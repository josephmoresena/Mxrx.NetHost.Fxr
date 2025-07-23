# Description

`Mxrx.NetHost.Fxr` provides a flexible and powerful API for building custom .NET hosts using Native AOT technology.

## Key Features

* Custom .NET host creation
* Assembly loading and runtime management
* Native invocation of .NET methods

## Installation

Install via NuGet:

```bash
dotnet add package Mxrx.NetHost.Fxr
```

**Supported Frameworks:**

* .NET 8 and later

Ensure your project targets a compatible version.

## Quick Start

1. Load the native `hostfxr` library from the .NET SDK.
2. Create a custom host context using the loaded resolver.
3. Initialize and configure the host using the provided APIs.
4. Load assemblies and invoke desired functions.
5. Compile your host using Native AOT.

---

## API Overview

### FrameworkResolver

Represents an abstraction over the native `hostfxr` library.

#### Properties

* **Handle**: The native handle of the loaded `hostfxr` instance.

#### Methods

* **Initialize(InitializationParameters)**: Initializes a new host context.
* **Dispose()**: Unloads the resolver if no active host context is present.

#### Static Methods

* **LoadResolver(String)**: Loads the native `hostfxr` and returns a new `FrameworkResolver` instance.
* **GetActiveOrLoad(String, out Boolean)**: Returns the current `FrameworkResolver` or loads a new one.

### HostContext

Represents a .NET runtime host context.

#### Properties

* **Resolver**: The `FrameworkResolver` used to create this context.
* **IsCommandLine**: Indicates if the context was initialized for a command-line app.
* **IsClosed**: Indicates if the context has been closed.

#### Methods

* **GetHandle()**: Returns the internal handle for the current context.
* **SetErrorWriter(...)**: Sets error writers. Multiple overloads for different platforms and function signatures.
* **RunApp(out int)**: Executes the main method in the loaded application and returns its exit code.
* **GetFunctionPointer(...)**: Retrieves a pointer to a .NET function.
* **LoadAssembly(...)**: Loads an assembly into the context.
* **GetRuntimeProperty(...) / SetRuntimeProperty(...)**: Gets or sets runtime properties.
* **Dispose()**: Closes the current context.

### VolatileText

A `ref struct` representing a transient text value for interop with native host contexts.

#### Properties

* **BinaryLength**: Length in bytes.
* **NullTerminated**: Indicates if the string ends with a null character.
* **IsUtf8**: Indicates if the string uses UTF-8 encoding.
* **IsValid**: Indicates if the string is valid.

#### Methods

* **GetStringValue()**: Converts to `System.String`.
* **GetCStringValue()**: Converts to `Rxmxnx.PInvoke.CString`.
* **GetPinnableReference()**: Returns a managed reference to the text data.

### LoadAssemblyParameters & Builder

Used to define how assemblies are loaded into a host context.

* `LoadAssemblyParameters.CreateBuilder()` returns a `Builder`.

#### Builder Methods

* **WithAssemblyPath(...)**: Specifies the path to the assembly.
* **WithBytes(...)**: Loads from raw byte and (optionally) PDB data.

### InitializationParameters & Builder

Used to configure a host context at startup.

* `InitializationParameters.CreateBuilder()` returns a `Builder`.

#### Builder Methods

* **WithHostPath(...)**: Specifies the host path.
* **WithRootPath(...)**: Specifies the root application path.
* **WithArguments(...)**: Sets command-line arguments.

### NetFunctionInfo & Builder

Used to describe a managed method to be invoked from the host.

* `NetFunctionInfo.CreateBuilder()` returns a `Builder`.

#### Builder Methods

* **WithAssemblyPath(...)**: Specifies the assembly path.
* **WithTypeName(...) / WithType(...) / WithType<T>()**: Specifies the type.
* **WithMethodName(...)**: Specifies the method name.
* **WithDelegateTypeName(...) / WithDelegateType(...) / WithDelegateType<T>()**: Constrains by delegate type.
* **WithUnmanagedCallerOnly(Boolean)**: Marks as `UnmanagedCallersOnly` if true.

---

## Notes

* Strings passed to the CLR should be null-terminated.
* UTF-16 is expected on Windows; UTF-8 on Unix-like systems.
* Methods using `Type` or generics rely on reflection—ensure the required types are referenced in the host.
* This package is intended for Native AOT hosts and should not be used in regular .NET applications.
* `VBCompat` provides helpers for Visual Basic .NET compatibility.

---

## License

See [LICENSE](LICENSE) for details.