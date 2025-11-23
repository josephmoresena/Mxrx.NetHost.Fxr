namespace Mxrx.NetHost.Native;

/// <summary>
/// .NET host handle.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public readonly struct HostHandle
{
	/// <summary>
	/// Internal handle.
	/// </summary>
	internal IntPtr Handle { get; }

	/// <summary>
	/// Internal constructor.
	/// </summary>
	/// <param name="handle">Handle value.</param>
	internal HostHandle(IntPtr handle) => this.Handle = handle;
}