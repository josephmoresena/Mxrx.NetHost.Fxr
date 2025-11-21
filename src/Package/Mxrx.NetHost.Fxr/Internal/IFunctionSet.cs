namespace Mxrx.NetHost.Internal;

/// <summary>
/// Function set.
/// </summary>
#if !PACKAGE
[SuppressMessage(Constants.CSharpSquid, Constants.CheckIdS6640,
                 Justification = Constants.SecureUnsafeCodeJustification)]
#endif
internal unsafe interface IFunctionSet : IFrameworkResolverLibrary
{
	/// <summary>
	/// Closes host context.
	/// </summary>
	/// <param name="handle">Host context handle.</param>
	/// <returns>Operation result.</returns>
	RuntimeCallResult CloseContext(HostHandle handle);
	/// <summary>
	/// Retrieves function pointer.
	/// </summary>
	/// <param name="handle">Host context handle.</param>
	/// <param name="delegateType">Function type.</param>
	/// <param name="funcPtr">Output. Function pointer.</param>
	/// <returns>Operation result.</returns>
	RuntimeCallResult GetFunctionPointer(HostHandle handle, RuntimeDelegateType delegateType, out IntPtr funcPtr);
	/// <summary>
	/// Retrieves the runtime properties.
	/// </summary>
	/// <param name="handle">Host context handle.</param>
	/// <param name="propCount">Output. Number of properties.</param>
	/// <param name="propKeysPtr">Output. Pointer to properties keys buffer.</param>
	/// <param name="propValuesPtr">Output. Pointer to properties values buffer.</param>
	/// <returns>Operation result.</returns>
	RuntimeCallResult GetRuntimeProperties(HostHandle handle, ref UIntPtr propCount,
		out ReadOnlyValPtr<NativeCharPointer> propKeysPtr, out ReadOnlyValPtr<NativeCharPointer> propValuesPtr);
	/// <summary>
	/// Retrieves the runtime properties count.
	/// </summary>
	/// <param name="handle">Host context handle.</param>
	/// <param name="propCount">Output. Number of properties.</param>
	/// <returns>Operation result.</returns>
	RuntimeCallResult CountRuntimeProperties(HostHandle handle, out UIntPtr propCount);
	/// <summary>
	/// Retrieves the runtime property of <paramref name="keyPtr"/> value.
	/// </summary>
	/// <param name="handle">Host context handle.</param>
	/// <param name="keyPtr">Pointer to property name.</param>
	/// <param name="valuePtr">Output. Pointer to property value.</param>
	/// <returns>Operation result.</returns>
	RuntimeCallResult GetRuntimePropertyValue(HostHandle handle, NativeCharPointer keyPtr,
		out NativeCharPointer valuePtr);
	/// <summary>
	/// Sets the runtime property of <paramref name="keyPtr"/> value to <paramref name="valuePtr"/>.
	/// </summary>
	/// <param name="handle">Host context handle.</param>
	/// <param name="keyPtr">Pointer to property name.</param>
	/// <param name="valuePtr">Pointer to property new value.</param>
	/// <returns>Operation result.</returns>
	RuntimeCallResult SetRuntimePropertyValue(HostHandle handle, NativeCharPointer keyPtr, NativeCharPointer valuePtr);
	/// <summary>
	/// Initializes a new context handle for an empty command line execution.
	/// </summary>
	/// <param name="handle">Output. Host context handle.</param>
	/// <returns>Operation result.</returns>
	RuntimeCallResult Initialize(out HostHandle handle)
		=> this.Initialize(0, default, in Unsafe.NullRef<InitParameters>(), out handle);
	/// <summary>
	/// Initializes a new context handle for a command line execution.
	/// </summary>
	/// <param name="argsCount">Count of command args in the buffer.</param>
	/// <param name="argsPtr">Pointer to the commands args buffer.</param>
	/// <param name="initParams">Input. Initialization parameters.</param>
	/// <param name="handle">Output. Host context handle.</param>
	/// <returns>Operation result.</returns>
	RuntimeCallResult Initialize(Int32 argsCount, NativeCharPointer* argsPtr, in InitParameters initParams,
		out HostHandle handle);
	/// <summary>
	/// Initializes a new context handle with a runtime config file.
	/// </summary>
	/// <param name="configPathPtr">Pointer to the config file path.</param>
	/// <param name="initParams">Input. Initialization parameters.</param>
	/// <param name="handle">Output. Host context handle.</param>
	/// <returns>Operation result.</returns>
	RuntimeCallResult Initialize(NativeCharPointer configPathPtr, in InitParameters initParams, out HostHandle handle);
	/// <summary>
	/// Runs application from current context.
	/// </summary>
	/// <param name="handle">Host context handle.</param>
	/// <returns>Exit code.</returns>
	Int32 RunApp(HostHandle handle);
	/// <summary>
	/// Set error callback function.
	/// </summary>
	/// <param name="handle">Host context handle.</param>
	/// <param name="writeErrPtr">Write error delegate pointer.</param>
	/// <returns>The previous error delegate pointer.</returns>
	void* SetError(HostHandle handle, void* writeErrPtr);
}