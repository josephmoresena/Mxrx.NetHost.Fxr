namespace Mxrx.NetHost;

public abstract partial class HostContext
{
	/// <summary>
	/// Indicates whether current instance is disposed.
	/// </summary>
	private readonly IMutableWrapper<Boolean> _isDisposed = IMutableWrapper<Boolean>.Create();

	/// <summary>
	/// Pointer to <see cref="RuntimeDelegateType.ComActivation"/> function.
	/// </summary>
	private IntPtr? _comActivationPtr;
	/// <summary>
	/// Pointer to <see cref="RuntimeDelegateType.ComRegister"/> function.
	/// </summary>
	private IntPtr? _comRegisterPtr;
	/// <summary>
	/// Pointer to <see cref="RuntimeDelegateType.ComUnregister"/> function.
	/// </summary>
	private IntPtr? _comUnregister;

	/// <summary>
	/// Control for pointer invalidation.
	/// </summary>
	private IMutableWrapper<Boolean> _control = IMutableWrapper.Create(false);
	/// <summary>
	/// Pointer to <see cref="RuntimeDelegateType.GetFunction"/> function.
	/// </summary>
	private IntPtr? _getFunctionPointerPtr;
	/// <summary>
	/// Pointer to <see cref="RuntimeDelegateType.LoadAssemblyAndGetFunction"/> function.
	/// </summary>
	private IntPtr? _loadAssemblyAndGetFunctionPointerPtr;
	/// <summary>
	/// Pointer to <see cref="RuntimeDelegateType.LoadAssemblyBytes"/> function.
	/// </summary>
	private IntPtr? _loadAssemblyFromBytesPtr;
	/// <summary>
	/// Pointer to <see cref="RuntimeDelegateType.LoadAssembly"/> function.
	/// </summary>
	private IntPtr? _loadAssemblyPtr;
	/// <summary>
	/// Pointer to <see cref="RuntimeDelegateType.LoadInMemoryAssembly"/> function.
	/// </summary>
	private IntPtr? _loadInMemoryAssemblyPtr;
	/// <summary>
	/// Pointer to <see cref="RuntimeDelegateType.WinRtActivation"/> function.
	/// </summary>
	private IntPtr? _winrtActivationPtr;
	/// <summary>
	/// Write error delegate.
	/// </summary>
	private WriteErrorDelegate? _writeError;
	/// <summary>
	/// Write UTF-8 error delegate;
	/// </summary>
	private WriteUtfErrorDelegate? _writeUtfError;

	/// <summary>
	/// Invalidates all attached pointers.
	/// </summary>
	private void InvalidatePointers()
	{
		this._control.Value = true;
		this._control = IMutableWrapper.Create(false);
	}
	/// <summary>
	/// Throws an exception if current instance is disposed.
	/// </summary>
	/// <exception cref="InvalidOperationException">Throws if current instance is disposed.</exception>
	private void ThrowIfDisposed()
	{
		if (!this.Closed) return;
		IMessageResource resource = IMessageResource.GetInstance();
		throw new InvalidOperationException(resource.HostClosed);
	}
	/// <summary>
	/// Sets error writer function from pointer.
	/// </summary>
	/// <param name="writeErrorPtr"></param>
	private void SetErrorWriter(IntPtr writeErrorPtr)
	{
		this._writeError = default;
		this._writeUtfError = default;
		this.Resolver.SetErrorWriter(this, writeErrorPtr);
	}
	/// <summary>
	/// Error writer function.
	/// </summary>
	/// <param name="error0">First error character 0 read-only pointer.</param>
	private unsafe void WriteError(ReadOnlyValPtr<Char> error0)
	{
		ReadOnlySpan<Char> errorSpan = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(error0);
		if (this._writeError is not null)
		{
			this._writeError(errorSpan);
		}
		else if (this._writeUtfError is not null)
		{
			Int32 utf8Length = Encoding.UTF8.GetByteCount(errorSpan);
			Byte[]? arr = utf8Length < 250 ? ArrayPool<Byte>.Shared.Rent(utf8Length) : default;
			try
			{
				Span<Byte> utf8Span = arr is null ? stackalloc Byte[utf8Length] : arr.AsSpan(0, utf8Length);
				Encoding.UTF8.GetBytes(errorSpan, utf8Span);
				this._writeUtfError(utf8Span);
			}
			finally
			{
				if (arr is not null)
					ArrayPool<Byte>.Shared.Return(arr, true);
			}
		}
	}
	/// <summary>
	/// UTF-8 error writer function.
	/// </summary>
	/// <param name="error0">First UTF-8 error character 0 read-only pointer.</param>
	private unsafe void WriteUtfError(ReadOnlyValPtr<Byte> error0)
	{
		ReadOnlySpan<Byte> utf8Span = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(error0);
		if (this._writeUtfError is not null)
		{
			this._writeUtfError(utf8Span);
		}
		else if (this._writeError is not null)
		{
			Int32 length = Encoding.UTF8.GetCharCount(utf8Span);
			Char[]? arr = length < 250 ? ArrayPool<Char>.Shared.Rent(length) : default;
			try
			{
				Span<Char> errorSpan = arr is null ? stackalloc Char[length] : arr.AsSpan(0, length);
				Encoding.UTF8.GetChars(utf8Span, errorSpan);
				this._writeError(errorSpan);
			}
			finally
			{
				if (arr is not null)
					ArrayPool<Char>.Shared.Return(arr, true);
			}
		}
	}
}