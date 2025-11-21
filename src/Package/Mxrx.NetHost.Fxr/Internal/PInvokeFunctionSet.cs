namespace Mxrx.NetHost.Internal;

/// <summary>
/// P/Invoke <c>Hostfxr</c> library exported methods.
/// </summary>
#if !PACKAGE
[ExcludeFromCodeCoverage]
[SuppressMessage(Constants.CSharpSquid, Constants.CheckIdS6640,
                 Justification = Constants.SecureUnsafeCodeJustification)]
#endif
internal abstract unsafe partial class PInvokeFunctionSet : IFunctionSet
{
	/// <inheritdoc/>
	public static Int32 SizeOf => -1;

	RuntimeCallResult IFunctionSet.CloseContext(HostHandle handle) => this.CloseContextImpl(handle);
	RuntimeCallResult IFunctionSet.GetFunctionPointer(HostHandle handle, RuntimeDelegateType delegateType,
		out IntPtr funcPtr)
		=> this.GetFunctionPointerImpl(handle, delegateType, out funcPtr);
	RuntimeCallResult IFunctionSet.GetRuntimeProperties(HostHandle handle, ref UIntPtr propCount,
		out ReadOnlyValPtr<NativeCharPointer> propKeysPtr, out ReadOnlyValPtr<NativeCharPointer> propValuesPtr)
		=> this.GetRuntimePropertiesImpl(handle, ref propCount, out propKeysPtr, out propValuesPtr);
	RuntimeCallResult IFunctionSet.CountRuntimeProperties(HostHandle handle, out UIntPtr propCount)
		=> this.CountRuntimePropertiesImpl(handle, out propCount);
	RuntimeCallResult IFunctionSet.GetRuntimePropertyValue(HostHandle handle, NativeCharPointer keyPtr,
		out NativeCharPointer valuePtr)
		=> this.GetRuntimePropertyValueImpl(handle, keyPtr, out valuePtr);
	RuntimeCallResult IFunctionSet.SetRuntimePropertyValue(HostHandle handle, NativeCharPointer keyPtr,
		NativeCharPointer valuePtr)
		=> this.SetRuntimePropertyValueImpl(handle, keyPtr, valuePtr);
	RuntimeCallResult IFunctionSet.Initialize(Int32 argsCount, NativeCharPointer* argsPtr, in InitParameters initParams,
		out HostHandle handle)
		=> this.InitializeImpl(argsCount, argsPtr, initParams, out handle);
	RuntimeCallResult IFunctionSet.Initialize(NativeCharPointer configPathPtr, in InitParameters initParams,
		out HostHandle handle)
		=> this.InitializeImpl(configPathPtr, in initParams, out handle);
	Int32 IFunctionSet.RunApp(HostHandle handle) => this.RunAppImpl(handle);
	void* IFunctionSet.SetError(HostHandle handle, void* writeErrPtr) => this.SetErrorImpl(handle, writeErrPtr);

	/// <inheritdoc cref="IFunctionSet.CloseContext(HostHandle)"/>
	private protected abstract RuntimeCallResult CloseContextImpl(HostHandle handle);
	/// <inheritdoc cref="IFunctionSet.GetFunctionPointer(HostHandle, RuntimeDelegateType, out IntPtr)"/>
	private protected abstract RuntimeCallResult GetFunctionPointerImpl(HostHandle handle,
		RuntimeDelegateType delegateType, out IntPtr funcPtr);
	/// <inheritdoc
	///     cref="IFunctionSet.GetRuntimeProperties(HostHandle, ref UIntPtr, out ReadOnlyValPtr{NativeCharPointer}, out ReadOnlyValPtr{NativeCharPointer})"/>
	private protected abstract RuntimeCallResult GetRuntimePropertiesImpl(HostHandle handle, ref UIntPtr propCount,
		out ReadOnlyValPtr<NativeCharPointer> propKeysPtr, out ReadOnlyValPtr<NativeCharPointer> propValuesPtr);
	/// <inheritdoc cref="IFunctionSet.CountRuntimeProperties(HostHandle, out UIntPtr)"/>
	private protected abstract RuntimeCallResult CountRuntimePropertiesImpl(HostHandle handle, out UIntPtr propCount);
	/// <inheritdoc
	///     cref="IFunctionSet.GetRuntimePropertyValue(HostHandle, NativeCharPointer, out NativeCharPointer)"/>
	private protected abstract RuntimeCallResult GetRuntimePropertyValueImpl(HostHandle handle,
		NativeCharPointer keyPtr, out NativeCharPointer valuePtr);
	/// <inheritdoc
	///     cref="IFunctionSet.SetRuntimePropertyValue(HostHandle, NativeCharPointer, NativeCharPointer)"/>
	private protected abstract RuntimeCallResult SetRuntimePropertyValueImpl(HostHandle handle,
		NativeCharPointer keyPtr, NativeCharPointer valuePtr);
	/// <inheritdoc
	///     cref="IFunctionSet.Initialize(Int32, NativeCharPointer*, in InitParameters, out HostHandle)"/>
	private protected abstract RuntimeCallResult InitializeImpl(Int32 argsCount, NativeCharPointer* argsPtr,
		in InitParameters initParams, out HostHandle handle);
	/// <inheritdoc cref="IFunctionSet.Initialize(NativeCharPointer, in InitParameters, out HostHandle)"/>
	private protected abstract RuntimeCallResult InitializeImpl(NativeCharPointer configPathPtr,
		in InitParameters initParams, out HostHandle handle);
	/// <inheritdoc cref="IFunctionSet.RunApp(HostHandle)"/>
	private protected abstract Int32 RunAppImpl(HostHandle handle);
	/// <inheritdoc cref="IFunctionSet.SetError(HostHandle, void*)"/>
	private protected abstract void* SetErrorImpl(HostHandle handle, void* writeErrPtr);

	/// <summary>
	/// Retrieves the function set for <typeparamref name="TLibrary"/>.
	/// </summary>
	/// <typeparam name="TLibrary">A <typeparamref name="TLibrary"/> type.</typeparam>
	/// <returns>A <see cref="Mxrx.NetHost.Internal.PInvokeFunctionSet"/> instance.</returns>
	public static PInvokeFunctionSet GetFunctionSet<TLibrary>() where TLibrary : IFrameworkResolverLib.IPInvoke
		=> FunctionSet<TLibrary>.Instance;
}