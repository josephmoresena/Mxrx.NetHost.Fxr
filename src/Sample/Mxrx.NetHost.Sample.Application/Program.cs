using System.Runtime.InteropServices;

namespace Mxrx.NetHost.Sample.Application;

public static class Program
{
	private static readonly ManualResetEvent mre = new(false);

	private static Byte isWaiting;
	private static Int32 callCount;

	public static void Main(String[] args)
	{
		Console.WriteLine($"{nameof(Program)} started - args = [ {String.Join(", ", args)} ]");
		Program.isWaiting = 1;
		Program.mre.WaitOne();
	}

#if NET5_0_OR_GREATER
	[UnmanagedCallersOnly]
#endif
	public static Byte IsWaiting() => Program.isWaiting;

#if NET5_0_OR_GREATER
	[UnmanagedCallersOnly]
#endif
	public static void Hello(IntPtr message, Byte utf8)
	{
		Console.WriteLine($"Hello, world! from {nameof(Program)} utf16: {utf8 == 0} [count: {++Program.callCount}]");
		Console.WriteLine(
			$"-- message: {(utf8 == 0 ? Marshal.PtrToStringUni(message) : Marshal.PtrToStringUTF8(message))}");
		if (Program.callCount < 3) return;

		Console.WriteLine("Signaling app to close");
		Program.mre.Set();
	}
}