namespace Mxrx.NetHost;

#if !PACKAGE
[SuppressMessage(Constants.CSharpSquid, Constants.CheckIdS3881, Justification = Constants.OptimizedJustification)]
#endif
public unsafe partial class FrameworkResolver
{
	/// <summary>
	/// Parameterless constructor.
	/// </summary>
	private protected FrameworkResolver() : this(IntPtr.Zero) { }

	/// <summary>
	/// Loads assembly for <paramref name="parameters"/>
	/// </summary>
	/// <param name="hostContext">A <see cref="HostContext"/> instance.</param>
	/// <param name="parameters">A <see cref="LoadAssemblyParameters"/> instance.</param>
	protected internal virtual void LoadAssembly(HostContext hostContext, LoadAssemblyParameters parameters)
	{
		delegate* unmanaged[Stdcall]<Byte*, UIntPtr, Byte*, UIntPtr, void*, void*, RuntimeCallResult>
			loadAssemblyBytes =
				(delegate* unmanaged[Stdcall]<Byte*, UIntPtr, Byte*, UIntPtr, void*, void*, RuntimeCallResult>)
				hostContext.LoadAssemblyFromBytesPtr.ToPointer();
		this._clrInitialized = true;

		fixed (Byte* assemblyPtr = &MemoryMarshal.GetReference(parameters.AssemblyBytes))
		fixed (Byte* symbolPtr = &MemoryMarshal.GetReference(parameters.SymbolsBytes))
		{
			RuntimeCallResult value = loadAssemblyBytes(assemblyPtr, (UIntPtr)parameters.AssemblyBytes.Length,
			                                            !parameters.SymbolsBytes.IsEmpty ? symbolPtr : default,
			                                            (UIntPtr)parameters.SymbolsBytes.Length, default, default);
			FrameworkResolver.ThrowIfInvalidResult(value);
		}
	}

	/// <summary>
	/// Closes given host.
	/// </summary>
	/// <param name="hostContext">A <see cref="HostHandle"/> instance.</param>
	protected abstract void CloseHandle(HostContext hostContext);
	/// <summary>
	/// Configures the error writer pointer for <paramref name="hostContext"/>.
	/// </summary>
	/// <param name="hostContext">A <see cref="HostHandle"/> instance.</param>
	/// <param name="writeErrorPtr">Pointer to error writer function.</param>
	protected internal abstract void SetErrorWriter(HostContext hostContext, IntPtr writeErrorPtr);
	/// <summary>
	/// Retrieves function pointer delegate for <paramref name="info"/>.
	/// </summary>
	/// <param name="hostContext">A <see cref="HostHandle"/> instance.</param>
	/// <param name="info">A <see cref="NetFunctionInfo"/> instance.</param>
	/// <returns>Pointer to delegate.</returns>
	protected internal abstract IntPtr GetFunctionPointer(HostContext hostContext, NetFunctionInfo info);
	/// <summary>
	/// Retrieves the function pointer of <paramref name="delegateType"/> for <paramref name="hostContext"/> instance.
	/// </summary>
	/// <param name="hostContext">A <see cref="HostContext"/> instance.</param>
	/// <param name="delegateType">A <see cref="RuntimeDelegateType"/> value.</param>
	/// <returns>
	/// Runtime function pointer of <paramref name="delegateType"/> for <paramref name="hostContext"/> instance.
	/// </returns>
	protected internal abstract IntPtr GetFunctionPointer(HostContext hostContext, RuntimeDelegateType delegateType);
	/// <summary>
	/// Retrieves the <see cref="VolatileText"/> from <paramref name="propertyName"/>.
	/// </summary>
	/// <param name="hostContext">A <see cref="HostContext"/> instance.</param>
	/// <param name="propertyName">A <see cref="VolatileText"/> instance.</param>
	/// <returns>A <see cref="VolatileText"/> instance.</returns>
	protected internal abstract VolatileText GetProperty(HostContext hostContext, VolatileText propertyName);
	/// <summary>
	/// Retrieves number of runtime properties initialized.
	/// </summary>
	/// <param name="hostContext">A <see cref="HostContext"/> instance.</param>
	/// <returns>Number of runtime properties.</returns>
	protected internal abstract Int32 CountProperties(HostContext hostContext);
	/// <summary>
	/// Sets <paramref name="propertyValue"/> as the value of the property named for <paramref name="propertyName"/>.
	/// </summary>
	/// <param name="hostContext">A <see cref="HostContext"/> instance.</param>
	/// <param name="propertyName">A <see cref="VolatileText"/> instance.</param>
	/// <param name="propertyValue">A <see cref="VolatileText"/> instance.</param>
	protected internal abstract void SetProperty(HostContext hostContext, VolatileText propertyName,
		VolatileText propertyValue);
	/// <summary>
	/// Runs application from <paramref name="hostContext"/> instance.
	/// </summary>
	/// <param name="hostContext">A <see cref="HostHandle"/> instance.</param>
	protected abstract Int32 RunAsApplication(HostContext hostContext);
	/// <inheritdoc cref="IDisposable.Dispose()"/>
	protected virtual void Dispose(Boolean disposing)
	{
		// NOP
	}

	/// <summary>
	/// Retrieves the length of the host path.
	/// </summary>
	/// <param name="parameters">Reference. Host path parameters.</param>
	/// <returns>Host path length.</returns>
	private protected static UIntPtr GetHostPathLength(in HostPathParameters parameters)
	{
		UIntPtr result = default;
		_ = FrameworkResolver.GetHostPath(default, ref result, in parameters);
		return result;
	}

#pragma warning disable SYSLIB1054
	/// <summary>
	/// Static entry point to <c>get_hostfxr_path</c> method.
	/// </summary>
	/// <param name="pathPtr">Pointer to buffer to copy the host path.</param>
	/// <param name="pathLength">Buffer length. When output, path length.</param>
	/// <param name="parameters">Reference. Host path parameters.</param>
	/// <returns>A <see cref="RuntimeCallResult"/> value.</returns>
	[DllImport("*", EntryPoint = "get_hostfxr_path")]
	private protected static extern RuntimeCallResult GetHostPath(void* pathPtr, ref UIntPtr pathLength,
		in HostPathParameters parameters);
#pragma warning restore SYSLIB1054
}