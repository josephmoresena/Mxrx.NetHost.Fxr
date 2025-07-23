namespace Mxrx.NetHost;

/// <summary>
/// CLR host context.
/// </summary>
#if !PACKAGE
[ExcludeFromCodeCoverage]
[SuppressMessage(Constants.CSharpSquid, Constants.CheckIdS6640,
                 Justification = Constants.SecureUnsafeCodeJustification)]
[SuppressMessage(Constants.CSharpSquid, Constants.CheckIdS3881, Justification = Constants.OptimizedJustification)]
#endif
public abstract partial class HostContext : IDisposable
{
	/// <summary>
	/// Framework resolver instance.
	/// </summary>
	public FrameworkResolver Resolver { get; }
	/// <summary>
	/// Indicates whether current host is for a command line.
	/// </summary>
	public Boolean IsCommandLine { get; }
	/// <summary>
	/// Indicates whether current instance is closed.
	/// </summary>
	public Boolean Closed => this._isDisposed.Value;

	/// <summary>
	/// Constructor.
	/// </summary>
	/// <param name="resolver">A <see cref="FrameworkResolver"/> instance.</param>
	/// <param name="isCommandLine">Indicates whether current host is for a command line.</param>
	protected HostContext(FrameworkResolver resolver, Boolean isCommandLine) : this(
		resolver, HostHandle.Zero, isCommandLine) { }
	/// <inheritdoc/>
	public void Dispose()
	{
		this.Dispose(!this._isDisposed.Value);
		this._isDisposed.Value = true;
		GC.SuppressFinalize(this);
	}

	/// <summary>
	/// Current host handle.
	/// </summary>
	/// <returns>Current host handle.</returns>
	public IntPtr GetHandle() => !this.Closed ? this.Handle.Handle : default;
	/// <summary>
	/// Clears error writer function.
	/// </summary>
	public void ClearErrorWriter()
	{
		this.ThrowIfDisposed();
		this._writeError = default;
		this._writeUtfError = default;
		FrameworkResolver.ConfigureErrorWriter(this, true);
	}
	/// <summary>
	/// Sets error writer function.
	/// </summary>
	/// <param name="writeError">A <see cref="WriteErrorDelegate"/> delegate.</param>
	public void SetErrorWriter(WriteErrorDelegate writeError)
	{
		this.ThrowIfDisposed();
		this._writeError = writeError;
		this._writeUtfError = default;
		FrameworkResolver.ConfigureErrorWriter(this, false);
	}
	/// <summary>
	/// Sets UTF-8 error writer function.
	/// </summary>
	/// <param name="writeUtfError">A <see cref="WriteErrorDelegate"/> delegate.</param>
	public void SetErrorWriter(WriteUtfErrorDelegate writeUtfError)
	{
		this.ThrowIfDisposed();
		this._writeError = default;
		this._writeUtfError = writeUtfError;
		FrameworkResolver.ConfigureErrorWriter(this, false);
	}
	/// <summary>
	/// Sets error writer function from pointer.
	/// </summary>
	/// <param name="writeErrorPtr">A <see cref="FuncPtr{WriteErrorFromPointerDelegate}"/> pointer.</param>
	/// <exception cref="PlatformNotSupportedException">
	/// Throws if current platform is not Windows OS.
	/// </exception>
	public void SetErrorWriter(FuncPtr<WriteErrorFromPointerDelegate> writeErrorPtr)
	{
		this.ThrowIfDisposed();
		if (OperatingSystem.IsWindows())
		{
			this.SetErrorWriter(writeErrorPtr.Pointer);
			return;
		}
		IMessageResource resource = IMessageResource.GetInstance();
		throw new PlatformNotSupportedException(resource.WindowsRequired);
	}
	/// <summary>
	/// Sets error writer function from pointer.
	/// </summary>
	/// <param name="writeErrorPtr">A <see cref="FuncPtr{WriteErrorFromPointerDelegate}"/> pointer.</param>
	/// <exception cref="PlatformNotSupportedException">
	/// Throws if current platform is Windows OS.
	/// </exception>
	public void SetErrorWriter(FuncPtr<WriteUtfErrorFromPointerDelegate> writeErrorPtr)
	{
		this.ThrowIfDisposed();
		if (!OperatingSystem.IsWindows())
		{
			this.SetErrorWriter(writeErrorPtr.Pointer);
			return;
		}
		IMessageResource resource = IMessageResource.GetInstance();
		throw new PlatformNotSupportedException(resource.UnixRequired);
	}
	/// <summary>
	/// Sets error writer function from pointer.
	/// </summary>
	/// <param name="writeErrorPtr">A native write error function pointer.</param>
	/// <exception cref="PlatformNotSupportedException">
	/// Throws if current platform is not Windows OS.
	/// </exception>
	public unsafe void SetErrorWriter(delegate* unmanaged[Cdecl]<ReadOnlyValPtr<Char>> writeErrorPtr)
	{
		this.ThrowIfDisposed();
		if (OperatingSystem.IsWindows())
		{
			this.SetErrorWriter((IntPtr)writeErrorPtr);
			return;
		}
		IMessageResource resource = IMessageResource.GetInstance();
		throw new PlatformNotSupportedException(resource.WindowsRequired);
	}
	/// <summary>
	/// Sets error writer function from pointer.
	/// </summary>
	/// <param name="writeErrorPtr">A native write UTF-8 error function pointer.</param>
	/// <exception cref="PlatformNotSupportedException">
	/// Throws if current platform is Windows OS.
	/// </exception>
	public unsafe void SetErrorWriter(delegate* unmanaged<ReadOnlyValPtr<Byte>> writeErrorPtr)
	{
		this.ThrowIfDisposed();
		if (!OperatingSystem.IsWindows()) this.SetErrorWriter((IntPtr)writeErrorPtr);
		IMessageResource resource = IMessageResource.GetInstance();
		throw new PlatformNotSupportedException(resource.UnixRequired);
	}
	/// <summary>
	/// Tries to run current context as .NET application.
	/// </summary>
	/// <param name="exitCode">.NET application exit code.</param>
	/// <returns>
	/// <see langword="true"/> if current context runs as application; otherwise, <see langword="false"/>.
	/// </returns>
	public Boolean RunApp(out Int32 exitCode)
	{
		this.ThrowIfDisposed();

		exitCode = -1;
		return this.IsCommandLine && FrameworkResolver.Run(this, out exitCode);
	}
	/// <summary>
	/// Retrieves function pointer to <paramref name="info"/> function.
	/// </summary>
	/// <param name="info">Function information.</param>
	/// <returns>Function pointer.</returns>
	public IntPtr GetFunctionPointer(NetFunctionInfo info)
	{
		this.ThrowIfDisposed();
		return this.Resolver.GetFunctionPointer(this, info);
	}
	/// <summary>
	/// Retrieves function pointer to <paramref name="info"/> function.
	/// </summary>
	/// <typeparam name="TDelegate">Type of the function delegate.</typeparam>
	/// <param name="info">Function information.</param>
	/// <returns>Function pointer.</returns>
	public FuncPtr<TDelegate> GetFunctionPointer<TDelegate>(NetFunctionInfo info) where TDelegate : Delegate
		=> (FuncPtr<TDelegate>)this.GetFunctionPointer(info);
	/// <summary>
	/// Loads assembly for <paramref name="parameters"/>.
	/// </summary>
	/// <param name="parameters">A <see cref="LoadAssemblyParameters"/> instance.</param>
	public void LoadAssembly(LoadAssemblyParameters parameters)
	{
		this.ThrowIfDisposed();
		this.Resolver.LoadAssembly(this, parameters);
	}
	/// <summary>
	/// Retrieves the value of the property named for <paramref name="propertyName"/>.
	/// </summary>
	/// <param name="propertyName">A <see cref="VolatileText"/> instance.</param>
	/// <returns>A <see cref="VolatileText"/> instance.</returns>
	public VolatileText GetRuntimeProperty(VolatileText propertyName)
	{
		this.ThrowIfDisposed();
		return this.Resolver.GetProperty(this, propertyName);
	}
	/// <summary>
	/// Sets <paramref name="propertyValue"/> as the value of the property named for <paramref name="propertyName"/>.
	/// </summary>
	/// <param name="propertyName">A <see cref="VolatileText"/> instance.</param>
	/// <param name="propertyValue">A <see cref="VolatileText"/> instance.</param>
	public void SetRuntimeProperty(VolatileText propertyName, VolatileText propertyValue)
	{
		this.ThrowIfDisposed();
		this.Resolver.SetProperty(this, propertyName, propertyValue);
	}
	/// <inheritdoc cref="IDisposable.Dispose()"/>
	protected abstract void Dispose(Boolean disposing);
}