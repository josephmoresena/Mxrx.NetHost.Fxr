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
	public readonly IntPtr Handle { get; private init; }

	/// <summary>
	/// Defines an implicit conversion of a given <see cref="HostHandle"/> to <see cref="IntPtr"/>.
	/// </summary>
	/// <param name="value">A <see cref="HostHandle"/> to implicitly convert.</param>
	public static implicit operator IntPtr(HostHandle value) => value.Handle;
	/// <summary>
	/// Defines an implicit conversion of a given <see cref="IntPtr"/> to <see cref="HostHandle"/>.
	/// </summary>
	/// <param name="value">A <see cref="IntPtr"/> to implicitly convert.</param>
	public static implicit operator HostHandle(IntPtr value) => new() { Handle = value, };
}