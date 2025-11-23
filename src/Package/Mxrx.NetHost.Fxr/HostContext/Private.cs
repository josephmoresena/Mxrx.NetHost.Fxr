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
		ErrorHelper.Clear();
		this.Resolver.SetErrorWriter(this, writeErrorPtr);
	}
}