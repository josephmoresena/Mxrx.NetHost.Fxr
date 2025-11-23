namespace Mxrx.NetHost.Internal;

/// <summary>
/// Error class helper.
/// </summary>
#if !PACKAGE
[SuppressMessage(Constants.CSharpSquid, Constants.CheckIdS6640,
                 Justification = Constants.SecureUnsafeCodeJustification)]
#endif
internal abstract unsafe class ErrorHelper
{
	/// <summary>
	/// Singleton instance.
	/// </summary>
	private static readonly ErrorHelper instance = SystemInfo.IsWindows ? new Windows() : new Unix();

	/// <summary>
	/// Error write pointer.
	/// </summary>
	public static readonly IntPtr WriteErrorPtr =
		(IntPtr)(delegate* unmanaged<NativeCharPointer, void>)&ErrorHelper.NativeWriteError;

	/// <summary>
	/// Write error delegate.
	/// </summary>
	[ThreadStatic]
	private static WriteErrorDelegate? writeError;
	/// <summary>
	/// Write UTF-8 error delegate;
	/// </summary>
	[ThreadStatic]
	private static WriteUtfErrorDelegate? writeUtfError;

	/// <summary>
	/// Error writer function.
	/// </summary>
	/// <param name="error0">First error character 0 read-only pointer.</param>
	protected abstract void WriteErrorImpl(NativeCharPointer error0);

	/// <summary>
	/// Set error delegate.
	/// </summary>
	/// <param name="errorDelegate">Error delegate.</param>
	public static void SetErrorWriter(WriteErrorDelegate? errorDelegate)
	{
		ErrorHelper.writeError = errorDelegate;
		ErrorHelper.writeUtfError = default;
	}
	/// <summary>
	/// Set error delegate.
	/// </summary>
	/// <param name="errorDelegate">Error delegate.</param>
	public static void SetErrorWriter(WriteUtfErrorDelegate? errorDelegate)
	{
		ErrorHelper.writeError = default;
		ErrorHelper.writeUtfError = errorDelegate;
	}
	/// <summary>
	/// Clears current instance.
	/// </summary>
	public static void Clear()
	{
		ErrorHelper.writeError = default;
		ErrorHelper.writeUtfError = default;
	}

	[UnmanagedCallersOnly]
	private static void NativeWriteError(NativeCharPointer error0) => ErrorHelper.instance.WriteErrorImpl(error0);

	/// <summary>
	/// Error helper implementation for Windows OS.
	/// </summary>
	private sealed class Windows : ErrorHelper
	{
		/// <inheritdoc/>
		protected override void WriteErrorImpl(NativeCharPointer error0)
		{
			ReadOnlySpan<Char> errorSpan = MemoryMarshal.CreateReadOnlySpanFromNullTerminated((Char*)error0.Pointer);
			if (ErrorHelper.writeError is not null)
			{
				ErrorHelper.writeError(errorSpan);
			}
			else if (ErrorHelper.writeUtfError is not null)
			{
				Int32 utf8Length = Encoding.UTF8.GetByteCount(errorSpan);
				Byte[]? arr = utf8Length < 500 ? ArrayPool<Byte>.Shared.Rent(utf8Length) : default;
				try
				{
					Span<Byte> utf8Span = arr is null ? stackalloc Byte[utf8Length] : arr.AsSpan(0, utf8Length);
					Encoding.UTF8.GetBytes(errorSpan, utf8Span);
					ErrorHelper.writeUtfError(utf8Span);
				}
				finally
				{
					if (arr is not null)
						ArrayPool<Byte>.Shared.Return(arr, true);
				}
			}
		}
	}

	/// <summary>
	/// Error helper implementation for Unix-like OS.
	/// </summary>
	private sealed class Unix : ErrorHelper
	{
		/// <inheritdoc/>
		protected override void WriteErrorImpl(NativeCharPointer error0)
		{
			ReadOnlySpan<Byte> utf8Span = MemoryMarshal.CreateReadOnlySpanFromNullTerminated((Byte*)error0.Pointer);
			if (ErrorHelper.writeUtfError is not null)
			{
				ErrorHelper.writeUtfError(utf8Span);
			}
			else if (ErrorHelper.writeError is not null)
			{
				Int32 length = Encoding.UTF8.GetCharCount(utf8Span);
				Char[]? arr = length < 250 ? ArrayPool<Char>.Shared.Rent(length) : default;
				try
				{
					Span<Char> errorSpan = arr is null ? stackalloc Char[length] : arr.AsSpan(0, length);
					Encoding.UTF8.GetChars(utf8Span, errorSpan);
					ErrorHelper.writeError(errorSpan);
				}
				finally
				{
					if (arr is not null)
						ArrayPool<Char>.Shared.Return(arr, true);
				}
			}
		}
	}
}