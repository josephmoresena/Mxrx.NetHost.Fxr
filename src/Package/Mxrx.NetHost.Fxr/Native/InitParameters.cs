namespace Mxrx.NetHost.Native;

/// <summary>
/// Initialization .NET host parameters.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public readonly struct InitParameters
{
	/// <summary>
	/// Current structure size.
	/// </summary>
	internal UIntPtr Size { get; init; }
	/// <summary>
	/// Pointer to host path.
	/// </summary>
	internal NativeCharPointer HostPathPtr { get; init; }
	/// <summary>
	/// Pointer to root path.
	/// </summary>
	internal NativeCharPointer RootPathPtr { get; init; }
}