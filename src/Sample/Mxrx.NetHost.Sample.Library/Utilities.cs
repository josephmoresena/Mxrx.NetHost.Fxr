using System.Runtime.InteropServices;

namespace Mxrx.NetHost.Sample.Library;

public static class Utilities
{
	private static Int32 callCount = 1;

	public static Int32 Hello(IntPtr arg, Int32 argLength)
	{
		if (argLength < Marshal.SizeOf(typeof(LibArgs)))
			return 1;

		LibArgs libArgs = Marshal.PtrToStructure<LibArgs>(arg);
		Console.WriteLine($"Hello, world! from {nameof(Utilities)} [count: {Utilities.callCount++}]");
		Utilities.PrintLibArgs(libArgs);
		return 0;
	}
	public static void CustomEntryPoint(LibArgs libArgs)
	{
		Console.WriteLine($"Hello, world! from {nameof(Utilities.CustomEntryPoint)} in {nameof(Utilities)}");
		Utilities.PrintLibArgs(libArgs);
	}

#if NET5_0_OR_GREATER
	[UnmanagedCallersOnly]
	public static void CustomEntryPointUnmanagedCallersOnly(LibArgs libArgs)
	{
		Console.WriteLine(
			$"Hello, world! from {nameof(Utilities.CustomEntryPointUnmanagedCallersOnly)} in {nameof(Utilities)}");
		Utilities.PrintLibArgs(libArgs);
	}
#endif

	private static void PrintLibArgs(LibArgs libArgs)
	{
		String? message = libArgs.Utf8 == 0 ?
			Marshal.PtrToStringUni(libArgs.Message) :
			Marshal.PtrToStringUTF8(libArgs.Message);

		Console.WriteLine($"-- message: {message}");
		Console.WriteLine($"-- number: {libArgs.Number}");
		Console.WriteLine($"-- utf16: {libArgs.Utf8 == 0}");
	}
}