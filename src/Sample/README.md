# Description

This document describes the process of creating a custom .NET host using `Mxrx.NetHost.Fxr`. For a deeper understanding
of some of the concepts, please refer to the official documentation:

* [Write a custom .NET host to control the .NET runtime from your native code](https://learn.microsoft.com/en-us/dotnet/core/tutorials/netcore-hosting)
* [DllImportAttribute Class](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.dllimportattribute)
* [NativeLibrary loading](https://learn.microsoft.com/en-us/dotnet/standard/native-interop/native-library-loading)
* [Unmanaged calling conventions](https://learn.microsoft.com/en-us/dotnet/standard/native-interop/calling-conventions)
* [Native code interop with Native AOT](https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot/interop)
* [UnmanagedCallersOnlyAttribute Class](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.unmanagedcallersonlyattribute)

The examples implemented here are heavily based on
the [official sample](https://github.com/dotnet/samples/blob/main/core/hosting/readme.md).

## Loading `FrameworkResolver`

The first step in creating a .NET host is loading the native `hostfxr` library. This library is part of the .NET/.NET
Core runtime environment.
`Mxrx.NetHost.Fxr` is compatible with the `hostfxr` library from .NET 5.0 or later.

The file is located at: `[dotnet_root]/host/fxr/[dotnet_version]`. Depending on the operating system, the file found
there will be:

* Windows: `hostfxr.dll`
* macOS: `libhostfxr.dylib`
* Linux and FreeBSD: `libhostfxr.so`

This library must be dynamically linked, so from managed code we need to access it through `P/Invoke` or the
`NativeLibrary` class.

`Mxrx.NetHost.Fxr` simplifies the loading process through the `FrameworkResolver` class by offering the following
alternatives:

* **Classic P/Invoke:**
  Useful when the library is located in a well-known location at process runtime and can be linked at compile time. Each
  call must be declared in a class implementing `IFrameworkResolverLibrary.IPInvoke` using the `___cdecl` convention.

```csharp
using FrameworkResolver fxr = FrameworkResolver.LoadResolver<HostFxrLibrary>();
...

class HostFxrLibrary : IFrameworkResolverLibrary.IPInvoke
{
    [DllImport("hostfxr", CallingConvention = CallingConvention.Cdecl,
	           EntryPoint = IFrameworkResolverLibrary.CloseHandleSymbol)]
	public static extern RuntimeCallResult CloseContext(HostHandle handle);
    ...
}
```

* **Library Handle:**
  Useful when the library is already loaded in the process runtime.

```csharp
IntPtr libHandle = NativeLibrary.LoadLibrary("path/to/hostfxr");
...
using FrameworkResolver fxr = FrameworkResolver.LoadResolver(libHandle);
```

* **Library Path:**
  Useful when the library is not in a known location and must be linked at runtime.

```csharp
using FrameworkResolver fxr = FrameworkResolver.LoadResolver("path/to/hostfxr");
```

* **Microsoft.NETCore.App.Host package:**
  Similar to the previous option, but uses a NuGet package that includes the `nethost` library (which must be linked
  statically when compiling the custom host). At runtime, it allows locating the physical path of `hostfxr`.

```csharp
using FrameworkResolver fxr = FrameworkResolver.LoadResolver();
```

There is also an optional parameter, `GetHostPathParameters`, which allows `nethost` to locate the native `hostfxr`
library.

```csharp
GetHostPathParameters getHost = GetHostPathParameters.CreateBuilder()
                                    .WithAssemblyPathPath(ASSEMBLY_DLL_PATH)
                                    .WithRootPath(RUNTIME_ROOT_PATH)
using FrameworkResolver fxr = FrameworkResolver.LoadResolver(getHost);
```

To reference the `Microsoft.NETCore.App.Host` package and link it statically,
review [this MSBuild target](StaticNetHostLink.targets).

## Interoperable Text Handling

The .NET/.NET Core runtimes are cross-platform and must adapt to each host machine where they run.

The following considerations should be taken into account to maximize the performance of the custom host:

* If the host is running on Windows systems, the encoding of interoperable text is `UTF-16`, compatible with `String`,
  `ReadOnlySpan<Char>`, and `Char`.
* If the host is running on Unix systems, the encoding of interoperable text is `UTF-8`, compatible with `CString`,
  `CStringSequence`, `Byte[]`, `ReadOnlySpan<Byte>`, and `Byte`.
* All text values must be null-terminated; for `UTF-16`, this means ending with `\0`.
* When using `String` literals, the null terminator must be manually appended at the end of the text.

## Initializing `HostContext`

Once you have a `FrameworkResolver` instance, you can begin loading the .NET assemblies for the current execution.
`Mxrx.NetHost.Fxr` simplifies this process using the `InitializationParameters`. After initialization, a `HostContext`
instance is produced (which encapsulates a `hostfxr_handle`) and will allow controlling the runtime.

### `InitializationParameters` Structure:

The `InitializationParameters` ref-struct configures the runtime environment and the loading of .NET assemblies.
It contains parameters to specify the target .NET runtime as well as two alternative loading modes:

* **Executable configuration:**
  Loads an executable assembly and allows the host to configure the command-line arguments executed when the application
  runs.

```csharp
InitializationParameters init = InitializationParameters.CreateBuilder()
                                    .WithArguments(EXECUTABLE_ASSEMBLY_PATH, COMMAND_ARG1, MAND_ARG2, ...)
                                    .Build();
```

* **Runtime environment configuration:**
  Loads the runtime configuration from an assembly’s `.runtimeconfig.json` file. This prepares the environment but does
  *not* load the assembly itself.

```csharp
InitializationParameters init = InitializationParameters.CreateBuilder()
                                    .WithRuntimeConfigPath(ASSEMBLY_CONFIGURATION_PATH)
                                    .Build();
```

Note that both options are mutually exclusive. `WithArguments` and `WithRuntimeConfigPath` accept different data types
to optimize interop performance depending on the operating system.

## Loading Assemblies via `HostContext`

`Mxrx.NetHost.Fxr` allows loading .NET assemblies into the initialized context via their physical location or binary
data.
This is done using the `LoadAssemblyParameters` structure:

### `LoadAssemblyParameters` Structure:

* **Physical location:**

```csharp
LoadAssemblyParameters load = LoadAssemblyParameters.CreateBuilder()
                                    .WithAssemblyPathPath(ASSEMBLY_DLL_PATH)
                                    .Build();
host.LoadAssembly(load);
```

* **Binary data:**

```csharp
LoadAssemblyParameters load = LoadAssemblyParameters.CreateBuilder()
                                    .WithAssemblyPathPath(ASSEMBLY_DLL_BYTES)
                                    .Build();
host.LoadAssembly(load);
```

Both options are mutually exclusive. `WithAssemblyPathPath` may receive additional PDB binary data to enable debugging
in the initialized context.

## Calling Managed Code through `HostContext`

To invoke/execute managed .NET code using `Mxrx.NetHost.Fxr`, the following options are available:

* **Command-line execution:**
  Requires initializing the context with an executable assembly. This enables the `RunApp()` method, which executes the
  assembly’s EntryPoint.

```csharp
using HostContext host = fxr.Intialize(init);
Boolean success = host.RunApp(out Int32 exitCode);
```

The method returns `true` only if the context was initialized with command-line arguments and `RunApp` has not been
called before.

* **Managed function pointer:**
  Uses reflection to locate a pointer to the managed function. `Mxrx.NetHost.Fxr` simplifies this using the
  `NetFunctionInfo` structure.

### `NetFunctionInfo` Structure

The `NetFunctionInfo` ref-struct assists `HostContext` in locating managed functions and even loading assemblies.

The available approaches:

* **Function marked with `UnmanagedCallersOnly`:**
  The runtime searches for a method in the specified type with the specified name and the `UnmanagedCallersOnly`
  attribute. No reflection is needed, which makes it more efficient. Available since .NET 5.0.

```csharp
using HostContext host = fxr.Intialize(init);
NetFunctionInfo funcInfo = NetFunctionInfo.CreateBuilder()
                                .WithUnmanagedCallerOnly(true)
                                .WithTypeName(METHOD_DECLARING_FULL_QUALIFIED_TYPE_NAME)
                                .WithMethodName(METHOD_NAME)
                                .Build();
IntPtr funcPtr = host.GetFunctionPointer(funcInfo);
```

* **Function with standard signature:**
  The runtime looks for a matching name/signature pair: `Int32(IntPtr, Int32)`.

```csharp
NetFunctionInfo funcInfo = NetFunctionInfo.CreateBuilder()
                                .WithTypeName(METHOD_DECLARING_FULL_QUALIFIED_TYPE_NAME)
                                .WithMethodName(METHOD_NAME)
                                .Build();
IntPtr funcPtr = host.GetFunctionPointer(funcInfo);
```

* **Function matching a managed delegate signature:**

```csharp
NetFunctionInfo funcInfo = NetFunctionInfo.CreateBuilder()
                                .WithTypeName(METHOD_DECLARING_FULL_QUALIFIED_TYPE_NAME)
                                .WithDelegateTypeName(METHOD_SIGNATURE_FULL_QUALIFIED_DELEGATE_NAME)
                                .WithMethodName(METHOD_NAME)
                                .Build();
IntPtr funcPtr = host.GetFunctionPointer(funcInfo);
```

* **Function matching a generic shared managed delegate signature:**
  Requires reflection.

```csharp
NetFunctionInfo funcInfo = NetFunctionInfo.CreateBuilder()
                                .WithTypeName(METHOD_DECLARING_FULL_QUALIFIED_TYPE_NAME)
                                .WithDelegateTypeName<TDelegate>()
                                .WithMethodName(METHOD_NAME)
                                .Build();
FuncPtr<TDelegate> funcPtr = host.GetFunctionPointer<TDelegate>(funcInfo);
```

* **Function matching a shared managed delegate signature:**
  Requires reflection.

```csharp
NetFunctionInfo funcInfo = NetFunctionInfo.CreateBuilder()
                                .WithTypeName(METHOD_DECLARING_FULL_QUALIFIED_TYPE_NAME)
                                .WithDelegateTypeName(typeof(TDelegate))
                                .WithMethodName(METHOD_NAME)
                                .Build();
IntPtr funcPtr = host.GetFunctionPointer(funcInfo);
```

All these options are mutually exclusive.
You may also use `WithAssemblyPathPath(ASSEMBLY_DLL_PATH)` to load a .NET assembly when requesting the managed function
pointer.

## Runtime Properties

`Mxrx.NetHost.Fxr` enables interaction with runtime properties through `HostContext`:

* **Get a property value:**

```csharp
VolatileText propName = VolatileText.Create(PROPERTY_NAME);
VolatileText propValue = host.GetRuntimeProperty(propName);
```

* **Set a property value:**

```csharp
VolatileText propName = VolatileText.Create(PROPERTY_NAME);
VolatileText propValue = VolatileText.Create(PROPERTY_NEW_VALUE);
host.SetRuntimeProperty(propName, propValue);
```

* **Enumerate all properties:**

```csharp
foreach (RuntimePropertyPair prop in host.GetRuntimeProperties()) 
{
    VolatileText propName = prop.Key;
    VolatileText propValue = prop.Value;
}
```

## Validity of `VolatileText` and Other `ref-struct` Parameters

* **If self-owned:**
  The value is only valid within the scope in which it was created. If the instance needs to outlive the initialization
  method, prefer using `String` or `CString`, even for UTF-8 literals.

* **If obtained from the context:**
  The value remains valid only while the originating context is still open (i.e., `Dispose()` has not been called).
  For runtime properties in particular, validity also depends on the property not being modified and on `RunApp` not
  being invoked.

The same rules apply to all ref-struct instances created as parameters, their validity depends on the underlying spans
provided during construction.

If any parameter needs to escape the builder method, it is mandatory to use `String`, `CString`, `String[]`,
`CString[]`, `CStringSequence`, or `Byte[]` in the relevant stages of the building process.