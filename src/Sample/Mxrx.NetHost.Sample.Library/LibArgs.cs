using System.Runtime.InteropServices;

namespace Mxrx.NetHost.Sample.Library;

[StructLayout(LayoutKind.Sequential)]
public struct LibArgs
{
	public IntPtr Message;
	public Int32 Number;
	public Byte Utf8;
}