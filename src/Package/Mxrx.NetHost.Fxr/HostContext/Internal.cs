namespace Mxrx.NetHost;

public abstract partial class HostContext
{
	/// <summary>
	/// Context handle.
	/// </summary>
	internal HostHandle Handle { get; }
	/// <summary>
	/// Host context error delegate.
	/// </summary>
	internal Delegate WriteErrorDelegate { get; }
	/// <inheritdoc cref="HostContext._loadAssemblyAndGetFunctionPointerPtr"/>
	internal IntPtr LoadAssemblyAndGetFunctionPointerPtr
	{
		get
		{
			this.ThrowIfDisposed();
			return this._loadAssemblyAndGetFunctionPointerPtr ??=
				this.Resolver.GetFunctionPointer(this, RuntimeDelegateType.LoadAssemblyAndGetFunction);
		}
	}
	/// <inheritdoc cref="HostContext._getFunctionPointerPtr"/>
	internal IntPtr GetFunctionPointerPtr
	{
		get
		{
			this.ThrowIfDisposed();
			return this._getFunctionPointerPtr ??=
				this.Resolver.GetFunctionPointer(this, RuntimeDelegateType.GetFunction);
		}
	}
	/// <inheritdoc cref="HostContext._loadAssemblyPtr"/>
	internal IntPtr LoadAssemblyPtr
	{
		get
		{
			this.ThrowIfDisposed();
			return this._loadAssemblyPtr ??= this.Resolver.GetFunctionPointer(this, RuntimeDelegateType.LoadAssembly);
		}
	}
	/// <inheritdoc cref="HostContext._loadAssemblyFromBytesPtr"/>
	internal IntPtr LoadAssemblyFromBytesPtr
	{
		get
		{
			this.ThrowIfDisposed();
			return this._loadAssemblyFromBytesPtr ??=
				this.Resolver.GetFunctionPointer(this, RuntimeDelegateType.LoadAssemblyBytes);
		}
	}
	/// <inheritdoc cref="HostContext._comActivationPtr"/>
	internal IntPtr ComActivationPtr
	{
		get
		{
			this.ThrowIfDisposed();
			return this._comActivationPtr ??= this.Resolver.GetFunctionPointer(this, RuntimeDelegateType.ComActivation);
		}
	}
	/// <inheritdoc cref="HostContext._winrtActivationPtr"/>
	internal IntPtr WinRtActivationPtr
	{
		get
		{
			this.ThrowIfDisposed();
			return this._winrtActivationPtr ??=
				this.Resolver.GetFunctionPointer(this, RuntimeDelegateType.WinRtActivation);
		}
	}
	/// <inheritdoc cref="HostContext._comRegisterPtr"/>
	internal IntPtr ComRegisterPtr
	{
		get
		{
			this.ThrowIfDisposed();
			return this._comRegisterPtr ??= this.Resolver.GetFunctionPointer(this, RuntimeDelegateType.ComRegister);
		}
	}
	/// <inheritdoc cref="HostContext._comUnregister"/>
	internal IntPtr ComUnRegisterPtr
	{
		get
		{
			this.ThrowIfDisposed();
			return this._comUnregister ??= this.Resolver.GetFunctionPointer(this, RuntimeDelegateType.ComUnregister);
		}
	}
	/// <inheritdoc cref="_loadInMemoryAssemblyPtr"/>
	internal IntPtr LoadInMemoryAssemblyPtr
	{
		get
		{
			this.ThrowIfDisposed();
			return this._loadInMemoryAssemblyPtr ??=
				this.Resolver.GetFunctionPointer(this, RuntimeDelegateType.LoadInMemoryAssembly);
		}
	}

	/// <summary>
	/// Private constructor.
	/// </summary>
	/// <param name="resolver">A <see cref="FrameworkResolver"/> instance.</param>
	/// <param name="handle">A <see cref="HostHandle"/> instance.</param>
	/// <param name="isCommandLine">Indicates whether current host is for a command line.</param>
	private protected HostContext(FrameworkResolver resolver, HostHandle handle, Boolean isCommandLine)
	{
		this.Resolver = resolver;
		this.IsCommandLine = isCommandLine;
		this.Handle = handle;
		this.WriteErrorDelegate = OperatingSystem.IsWindows() ?
			(WriteErrorFromPointerDelegate)this.WriteError :
			(WriteUtfErrorFromPointerDelegate)this.WriteUtfError;
	}

	/// <summary>
	/// Attaches <paramref name="value"/> to current instance.
	/// </summary>
	/// <param name="value">A <see cref="VolatileText"/> value.</param>
	internal void Attach(ref VolatileText value) => value.IsDisposed = this._isDisposed;
}