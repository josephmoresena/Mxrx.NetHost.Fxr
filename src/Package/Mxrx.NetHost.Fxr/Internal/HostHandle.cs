namespace Mxrx.NetHost.Internal;

#if !PACKAGE
[ExcludeFromCodeCoverage]
#endif
[StructLayout(LayoutKind.Sequential)]
internal readonly struct HostHandle
{
	/// <summary>
	/// Zero host.
	/// </summary>
	public static readonly HostHandle Zero = new();

	/// <summary>
	/// Internal handle.
	/// </summary>
	public readonly IntPtr Handle;
}