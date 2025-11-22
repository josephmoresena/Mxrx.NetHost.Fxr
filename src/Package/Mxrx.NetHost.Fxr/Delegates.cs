namespace Mxrx.NetHost;

/// <summary>
/// Delegate for error writing.
/// </summary>
/// <remarks>This function is native on Windows OS.</remarks>
public delegate void WriteErrorDelegate(ReadOnlySpan<Char> error);

/// <summary>
/// Delegate for UTF-8 error writing.
/// </summary>
/// <remarks>This function is native on Unix-like OS.</remarks>
public delegate void WriteUtfErrorDelegate(ReadOnlySpan<Byte> error);

/// <summary>
/// Delegate for error writing.
/// </summary>
/// <remarks>This function is native on Windows OS.</remarks>
public delegate void WriteErrorFromPointerDelegate(ReadOnlyValPtr<Char> error0);

/// <summary>
/// Delegate for UTF-8 error writing.
/// </summary>
/// <remarks>This function is native on Unix-like OS.</remarks>
public delegate void WriteUtfErrorFromPointerDelegate(ReadOnlyValPtr<Byte> error0);

/// <summary>
/// Delegate for error native writing.
/// </summary>
internal delegate void WriteNativeErrorDelegate(NativeCharPointer error);

/// <summary>
/// Delegate for <typeparamref name="TFunction"/> initialization.
/// </summary>
/// <typeparam name="TFunction">A <see cref="IFunctionSet"/></typeparam>
internal delegate Int32 InitializeFromHandle<TFunction>(IntPtr libraryHandle, out TFunction functionSet)
	where TFunction : IFunctionSet;