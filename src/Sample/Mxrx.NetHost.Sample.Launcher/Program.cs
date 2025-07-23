﻿using System.Runtime.InteropServices;

using Mxrx.NetHost;

using Rxmxnx.PInvoke;

if (args.Length < 2)
{
	Console.WriteLine("Missing arguments.");
	Console.WriteLine("0: Native hostfxr library path.");
	Console.WriteLine("1: .NET assembly path. (Library or Application)");
	Console.WriteLine("2 (Optional): Use UTF-8 encoding.");
	return;
}

using FrameworkResolver fxr = FrameworkResolver.LoadResolver(args[0]);
Boolean useUftString = args.Length > 2 && args[2].ToLower() switch
{
	"1" => true,
	"true" => true,
	_ => false,
};

if (args[1].Contains("Application"))
{
	await RunApplication(fxr, args[1], useUftString);
}
else
{
	await Task.Yield();
	UseLibrary(fxr, args[1], useUftString);
}

return;

static async Task RunApplication(FrameworkResolver fxr, String assemblyPath, Boolean useUftString)
{
	using HostContext context = fxr.Initialize(CreateApplicationInitParams(assemblyPath));
	IsWaitingDelegate isWaitingPtr = context.GetFunctionPointer<IsWaitingDelegate>(GetIsWaitingFunctionInfo()).Invoke;
	IntPtr helloPtr = context.GetFunctionPointer(GetHelloAppFunctionInfo());

	Task task = !useUftString ?
		InvokeHelloAsync(isWaitingPtr, helloPtr.GetUnsafeDelegate<HelloDelegate>()!) :
		InvokeHelloUftAsync(isWaitingPtr, helloPtr.GetUnsafeDelegate<HelloUtfDelegate>()!);

	context.RunApp(out Int32 exitCode);
	Console.WriteLine("Exit Code: " + exitCode);

	await task;
}
static void UseLibrary(FrameworkResolver fxr, String assemblyPath, Boolean useUftString)
{
	InitializationParameters initParams = InitializationParameters.CreateBuilder()
	                                                              .WithRuntimeConfigPath(
		                                                              assemblyPath.Replace(
			                                                              ".dll", ".runtimeconfig.json")).Build();
	using HostContext context = fxr.Initialize(initParams);
	DefaultDelegate hello = context.GetFunctionPointer<DefaultDelegate>(GetHelloLibFunctionInfo(assemblyPath)).Invoke;
	IntPtr customUnmanagedHelloPtr = context.GetFunctionPointer(GetCustomHelloLibUnmanagedFunctionInfo(assemblyPath));
	IntPtr customHelloPtr = context.GetFunctionPointer(GetCustomHelloLibFunctionInfo(assemblyPath));

	if (!useUftString)
	{
		LibArgs<Char> libArgs = new() { Message = "from host!".AsSpan().GetUnsafeValPtr(), };
		CustomLibHello(customHelloPtr.GetUnsafeDelegate<HelloLibraryDelegate>()!.Invoke, ref libArgs);
		LibHello(hello, ref libArgs);
		CustomLibHello(customUnmanagedHelloPtr.GetUnsafeDelegate<HelloLibraryDelegate>()!.Invoke, ref libArgs);
	}
	else
	{
		LibArgs<Byte> libArgs = new() { Message = "from host!"u8.GetUnsafeValPtr(), };
		CustomLibHello(customHelloPtr.GetUnsafeDelegate<HelloUtfLibraryDelegate>()!.Invoke, ref libArgs);
		LibHello(hello, ref libArgs);
		CustomLibHello(customUnmanagedHelloPtr.GetUnsafeDelegate<HelloUtfLibraryDelegate>()!.Invoke, ref libArgs);
	}
}
static InitializationParameters CreateApplicationInitParams(String assemblyPath)
{
	InitializationParameters.Builder builder = InitializationParameters.CreateBuilder();
	if (OperatingSystem.IsWindows())
		builder.WithArguments(assemblyPath, "app_arg_1", "app_arg_2");
	else
		builder.WithArguments((CString)assemblyPath, new(() => "app_arg_1"u8), new(() => "app_arg_2"u8));
	return builder.Build();
}
static NetFunctionInfo GetHelloAppFunctionInfo()
{
	NetFunctionInfo.Builder builder = NetFunctionInfo.CreateBuilder().WithUnmanagedCallerOnly(true);
	builder = OperatingSystem.IsWindows() ?
		builder.WithTypeName("Mxrx.NetHost.Sample.Application.Program, Mxrx.NetHost.Sample.Application\0")
		       .WithMethodName("Hello\0") :
		builder.WithTypeName("Mxrx.NetHost.Sample.Application.Program, Mxrx.NetHost.Sample.Application"u8)
		       .WithMethodName("Hello"u8);
	return builder.Build();
}
static NetFunctionInfo GetIsWaitingFunctionInfo()
{
	NetFunctionInfo.Builder builder = NetFunctionInfo.CreateBuilder().WithUnmanagedCallerOnly(true);
	builder = OperatingSystem.IsWindows() ?
		builder.WithTypeName("Mxrx.NetHost.Sample.Application.Program, Mxrx.NetHost.Sample.Application\0")
		       .WithMethodName("IsWaiting\0") :
		builder.WithTypeName("Mxrx.NetHost.Sample.Application.Program, Mxrx.NetHost.Sample.Application"u8)
		       .WithMethodName("IsWaiting"u8);
	return builder.Build();
}
static NetFunctionInfo GetHelloLibFunctionInfo(String assemblyPath)
{
	NetFunctionInfo.Builder builder = NetFunctionInfo.CreateBuilder().WithAssemblyPathPath(assemblyPath);
	builder = OperatingSystem.IsWindows() ?
		builder.WithTypeName("Mxrx.NetHost.Sample.Library.Utilities, Mxrx.NetHost.Sample.Library\0")
		       .WithMethodName("Hello\0") :
		builder.WithTypeName("Mxrx.NetHost.Sample.Library.Utilities, Mxrx.NetHost.Sample.Library"u8)
		       .WithMethodName("Hello"u8);
	return builder.Build();
}
static NetFunctionInfo GetCustomHelloLibUnmanagedFunctionInfo(String assemblyPath)
{
	NetFunctionInfo.Builder builder = NetFunctionInfo.CreateBuilder().WithAssemblyPathPath(assemblyPath)
	                                                 .WithUnmanagedCallerOnly(true);
	builder = OperatingSystem.IsWindows() ?
		builder.WithTypeName("Mxrx.NetHost.Sample.Library.Utilities, Mxrx.NetHost.Sample.Library\0")
		       .WithMethodName("CustomEntryPointUnmanagedCallersOnly\0") :
		builder.WithTypeName("Mxrx.NetHost.Sample.Library.Utilities, Mxrx.NetHost.Sample.Library"u8)
		       .WithMethodName("CustomEntryPointUnmanagedCallersOnly"u8);
	return builder.Build();
}
static NetFunctionInfo GetCustomHelloLibFunctionInfo(String assemblyPath)
{
	NetFunctionInfo.Builder builder = NetFunctionInfo.CreateBuilder().WithAssemblyPathPath(assemblyPath);
	builder = OperatingSystem.IsWindows() ?
		builder.WithTypeName("Mxrx.NetHost.Sample.Library.Utilities, Mxrx.NetHost.Sample.Library\0")
		       .WithMethodName("CustomEntryPoint\0")
		       .WithDelegateTypeName(
			       "Mxrx.NetHost.Sample.Library.CustomEntryPointDelegate, Mxrx.NetHost.Sample.Library\0") :
		builder.WithTypeName("Mxrx.NetHost.Sample.Library.Utilities, Mxrx.NetHost.Sample.Library"u8)
		       .WithMethodName("CustomEntryPoint"u8)
		       .WithDelegateTypeName(
			       "Mxrx.NetHost.Sample.Library.CustomEntryPointDelegate, Mxrx.NetHost.Sample.Library"u8);
	return builder.Build();
}
static async Task InvokeHelloAsync(IsWaitingDelegate isWaiting, HelloDelegate hello)
{
	while (isWaiting() != 1)
		await Task.Delay(100);
	for (Int32 i = 0; i < 3; i++)
		hello("Hello".AsSpan().GetUnsafeValPtr(), 0);
}
static async Task InvokeHelloUftAsync(IsWaitingDelegate isWaiting, HelloUtfDelegate hello)
{
	if (isWaiting() != 1)
		await Task.Delay(100);
	for (Int32 i = 0; i < 3; i++)
		hello("Hello"u8.GetUnsafeValPtr(), 1);
}
static void LibHello<TChar>(DefaultDelegate hello, ref LibArgs<TChar> libArgs) where TChar : unmanaged
{
	for (Int32 i = 0; i < 3; i++)
	{
		libArgs.Number = i;
		hello(libArgs.GetUnsafeValPtr(), LibArgs<TChar>.Size);
	}
}
static void CustomLibHello<TChar>(Action<LibArgs<TChar>> hello, ref LibArgs<TChar> libArgs) where TChar : unmanaged
{
	libArgs.Number = -1;
	hello(libArgs);
}

internal delegate void HelloDelegate(ReadOnlyValPtr<Char> message, Byte utf8);
internal delegate void HelloUtfDelegate(ReadOnlyValPtr<Byte> message, Byte utf8);
internal delegate void DefaultDelegate(IntPtr argPtr, Int32 argSize);
internal delegate void HelloLibraryDelegate(LibArgs<Char> args);
internal delegate void HelloUtfLibraryDelegate(LibArgs<Byte> args);
internal delegate Byte IsWaitingDelegate();

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct LibArgs<TChar>() where TChar : unmanaged
{
	public static readonly Int32 Size = sizeof(LibArgs<TChar>);

	public ReadOnlyValPtr<TChar> Message;
	public Int32 Number;
	public Byte Utf8 = (Byte)(sizeof(TChar) == 1 ? 1 : 0);
}