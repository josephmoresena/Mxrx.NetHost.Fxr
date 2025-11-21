namespace Mxrx.NetHost;

/// <summary>
/// <c>Hostfxr</c> library interface.
/// </summary>
public interface IFrameworkResolverLibrary
{
	/// <summary>
	/// Name of <c>hostfxr_close</c>.
	/// </summary>
	protected internal const String CloseHandleSymbol = "hostfxr_close";
	/// <summary>
	/// Name of <c>hostfxr_get_runtime_delegate</c>.
	/// </summary>
	protected internal const String GetDelegateSymbol = "hostfxr_get_runtime_delegate";
	/// <summary>
	/// Name of <c>hostfxr_get_runtime_properties</c>.
	/// </summary>
	protected internal const String GetRuntimePropertiesSymbol = "hostfxr_get_runtime_properties";
	/// <summary>
	/// Name of <c>hostfxr_get_runtime_property_value</c>.
	/// </summary>
	protected internal const String GetRuntimePropertyValueSymbol = "hostfxr_get_runtime_property_value";
	/// <summary>
	/// Name of <c>hostfxr_set_runtime_property_value</c>.
	/// </summary>
	protected internal const String SetRuntimePropertyValueSymbol = "hostfxr_set_runtime_property_value";
	/// <summary>
	/// Name of <c>hostfxr_initialize_for_dotnet_command_line</c>.
	/// </summary>
	protected internal const String InitializeForCommandSymbol = "hostfxr_initialize_for_dotnet_command_line";
	/// <summary>
	/// Name of <c>hostfxr_initialize_for_runtime_config</c>.
	/// </summary>
	protected internal const String InitializeForConfigSymbol = "hostfxr_initialize_for_runtime_config";
	/// <summary>
	/// Name of <c>hostfxr_main</c>.
	/// </summary>
	protected internal const String MainSymbol = "hostfxr_main";
	/// <summary>
	/// Name of <c>hostfxr_main_startupinfo</c>.
	/// </summary>
	protected internal const String MainStartupInfoSymbol = "hostfxr_main_startupinfo";
	/// <summary>
	/// Name of <c>hostfxr_run_app</c>.
	/// </summary>
	protected internal const String RunAppSymbol = "hostfxr_run_app";
	/// <summary>
	/// Name of <c>hostfxr_set_error_writer</c>.
	/// </summary>
	protected internal const String SetErrorWriterSymbol = "hostfxr_set_error_writer";

	/// <summary>
	/// Size of implementing type.
	/// </summary>
	internal static abstract Int32 SizeOf { get; }

	/// <summary>
	/// Function set.
	/// </summary>
	public interface IPInvoke : IFrameworkResolverLibrary
	{
		static Int32 IFrameworkResolverLibrary.SizeOf => 0;

		/// <summary>
		/// Closes host context.
		/// </summary>
		/// <param name="handle">Host context handle.</param>
		/// <returns>Operation result.</returns>
		static abstract RuntimeCallResult CloseContext(HostHandle handle);
		/// <summary>
		/// Retrieves function pointer.
		/// </summary>
		/// <param name="handle">Host context handle.</param>
		/// <param name="delegateType">Function type.</param>
		/// <param name="funcPtr">Output. Function pointer.</param>
		/// <returns>Operation result.</returns>
		static abstract RuntimeCallResult GetFunctionPointer(HostHandle handle, RuntimeDelegateType delegateType,
			out IntPtr funcPtr);
		/// <summary>
		/// Retrieves the runtime properties.
		/// </summary>
		/// <param name="handle">Host context handle.</param>
		/// <param name="propCount">Output. Number of properties.</param>
		/// <param name="propKeysPtr">Output. Pointer to properties keys buffer.</param>
		/// <param name="propValuesPtr">Output. Pointer to properties values buffer.</param>
		/// <returns>Operation result.</returns>
		static abstract RuntimeCallResult GetRuntimeProperties(HostHandle handle, ref UIntPtr propCount,
			ref ReadOnlyValPtr<NativeCharPointer> propKeysPtr, ref ReadOnlyValPtr<NativeCharPointer> propValuesPtr);
		/// <summary>
		/// Retrieves the runtime property of <paramref name="keyPtr"/> value.
		/// </summary>
		/// <param name="handle">Host context handle.</param>
		/// <param name="keyPtr">Pointer to property name.</param>
		/// <param name="valuePtr">Output. Pointer to property value.</param>
		/// <returns>Operation result.</returns>
		static abstract RuntimeCallResult GetRuntimePropertyValue(HostHandle handle, NativeCharPointer keyPtr,
			out NativeCharPointer valuePtr);
		/// <summary>
		/// Sets the runtime property of <paramref name="keyPtr"/> value to <paramref name="valuePtr"/>.
		/// </summary>
		/// <param name="handle">Host context handle.</param>
		/// <param name="keyPtr">Pointer to property name.</param>
		/// <param name="valuePtr">Pointer to property new value.</param>
		/// <returns>Operation result.</returns>
		static abstract RuntimeCallResult SetRuntimePropertyValue(HostHandle handle, NativeCharPointer keyPtr,
			NativeCharPointer valuePtr);
		/// <summary>
		/// Initializes a new context handle for a command line execution.
		/// </summary>
		/// <param name="argsCount">Count of command args in the buffer.</param>
		/// <param name="argsPtr">Pointer to the commands args buffer.</param>
		/// <param name="initParams">Input. Initialization parameters.</param>
		/// <param name="handle">Output. Host context handle.</param>
		/// <returns>Operation result.</returns>
		static abstract RuntimeCallResult InitializeForCommandLine(Int32 argsCount,
			ReadOnlyValPtr<NativeCharPointer> argsPtr, in InitParameters initParams, out HostHandle handle);
		/// <summary>
		/// Initializes a new context handle with a runtime config file.
		/// </summary>
		/// <param name="configPathPtr">Pointer to the config file path.</param>
		/// <param name="initParams">Input. Initialization parameters.</param>
		/// <param name="handle">Output. Host context handle.</param>
		/// <returns>Operation result.</returns>
		static abstract RuntimeCallResult InitializeForConfigFile(NativeCharPointer configPathPtr,
			in InitParameters initParams, out HostHandle handle);
		/// <summary>
		/// Runs application from current context.
		/// </summary>
		/// <param name="handle">Host context handle.</param>
		/// <returns>Exit code.</returns>
		static abstract Int32 RunApp(HostHandle handle);
		/// <summary>
		/// Set error callback function.
		/// </summary>
		/// <param name="handle">Host context handle.</param>
		/// <param name="writeErrPtr">Write error delegate pointer.</param>
		/// <returns>The previous error delegate pointer.</returns>
		static abstract IntPtr SetError(HostHandle handle, IntPtr writeErrPtr);
	}
}