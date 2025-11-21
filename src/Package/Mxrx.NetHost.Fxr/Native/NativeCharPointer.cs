namespace Mxrx.NetHost.Native;

/// <summary>
/// Native character type.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
#if !PACKAGE
[SuppressMessage(Constants.CSharpSquid, Constants.CheckIdS6640,
                 Justification = Constants.SecureUnsafeCodeJustification)]
#endif
public readonly unsafe struct NativeCharPointer
{
	/// <summary>
	/// Size of native host char.
	/// </summary>
	public static Int32 CharSize => SystemInfo.IsWindows ? sizeof(Char) : sizeof(Byte);

	/// <summary>
	/// Internal value.
	/// </summary>
	internal void* Pointer { get; init; }
}