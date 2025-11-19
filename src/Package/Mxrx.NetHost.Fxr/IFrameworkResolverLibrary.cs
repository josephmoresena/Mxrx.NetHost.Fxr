namespace Mxrx.NetHost;

/// <summary>
/// Exposes a type which implements via P/Invoke the <c>Hostfxr</c> library header.
/// </summary>
public interface IFrameworkResolverLibrary
{
	/// <summary>
	/// Name of <c>hostfxr_close</c>.
	/// </summary>
	public const String CloseHandleSymbol = "hostfxr_close";
	/// <summary>
	/// Name of <c>hostfxr_get_runtime_delegate</c>.
	/// </summary>
	public const String GetDelegateSymbol = "hostfxr_get_runtime_delegate";
#if !DISABLE_RUNTIME_PROPERTIES
	/// <summary>
	/// Name of <c>hostfxr_get_runtime_properties</c>.
	/// </summary>
	public const String GetRuntimePropertiesSymbol = "hostfxr_get_runtime_properties";
#endif
	/// <summary>
	/// Name of <c>hostfxr_get_runtime_property_value</c>.
	/// </summary>
	public const String GetRuntimePropertyValueSymbol = "hostfxr_get_runtime_property_value";
	/// <summary>
	/// Name of <c>hostfxr_set_runtime_property_value</c>.
	/// </summary>
	public const String SetRuntimePropertyValueSymbol = "hostfxr_set_runtime_property_value";
	/// <summary>
	/// Name of <c>hostfxr_initialize_for_dotnet_command_line</c>.
	/// </summary>
	public const String InitializeForCommandSymbol = "hostfxr_initialize_for_dotnet_command_line";
	/// <summary>
	/// Name of <c>hostfxr_initialize_for_runtime_config</c>.
	/// </summary>
	public const String InitializeForConfigSymbol = "hostfxr_initialize_for_runtime_config";
#if !DISABLE_MAIN_CALLS
	/// <summary>
	/// Name of <c>hostfxr_main</c>.
	/// </summary>
	public const String MainSymbol = "hostfxr_main";
	/// <summary>
	/// Name of <c>hostfxr_main_startupinfo</c>.
	/// </summary>
	public const String MainStartupInfoSymbol = "hostfxr_main_startupinfo";
#endif
	/// <summary>
	/// Name of <c>hostfxr_run_app</c>.
	/// </summary>
	public const String RunAppSymbol = "hostfxr_run_app";
	/// <summary>
	/// Name of <c>hostfxr_set_error_writer</c>.
	/// </summary>
	public const String SetErrorWriterSymbol = "hostfxr_set_error_writer";

	/// <summary>
	/// Closes host context.
	/// </summary>
	/// <param name="handle">Host context handle.</param>
	/// <returns>Operation result.</returns>
	static abstract RuntimeCallResult CloseContext(IntPtr handle);
	/// <summary>
	/// Retrieves function pointer.
	/// </summary>
	/// <param name="handle">Host context handle.</param>
	/// <param name="delegateType">Function type.</param>
	/// <param name="funcPtr">Output. Function pointer.</param>
	/// <returns>Operation result.</returns>
	static abstract RuntimeCallResult GetFunctionPointer(IntPtr handle, RuntimeDelegateType delegateType,
		out IntPtr funcPtr);
	/// <summary>
	/// Retrieves the runtime property of <paramref name="keyPtr"/> value.
	/// </summary>
	/// <param name="handle">Host context handle.</param>
	/// <param name="keyPtr">Pointer to property name.</param>
	/// <param name="valuePtr">Output. Pointer to property value.</param>
	/// <returns>Operation result.</returns>
	static abstract RuntimeCallResult GetRuntimePropertyValue(IntPtr handle, IntPtr keyPtr, out IntPtr valuePtr);
	/// <summary>
	/// Sets the runtime property of <paramref name="keyPtr"/> value to <paramref name="valuePtr"/>.
	/// </summary>
	/// <param name="handle">Host context handle.</param>
	/// <param name="keyPtr">Pointer to property name.</param>
	/// <param name="valuePtr">Pointer to property new value.</param>
	/// <returns>Operation result.</returns>
	static abstract RuntimeCallResult SetRuntimePropertyValue(IntPtr handle, IntPtr keyPtr, IntPtr valuePtr);
	/// <summary>
	/// Initializes a new context handle for a command line execution.
	/// </summary>
	/// <param name="argCount">Count of command args in the buffer.</param>
	/// <param name="argPtr">Pointer to the commands args buffer.</param>
	/// <param name="paramsPtr">Pointer to initialization parameters.</param>
	/// <param name="handle">Output. Host context handle.</param>
	/// <returns>Operation result.</returns>
	static abstract RuntimeCallResult Initialize(Int32 argCount, IntPtr argPtr, IntPtr paramsPtr, out IntPtr handle);
	/// <summary>
	/// Initializes a new context handle with a runtime config file.
	/// </summary>
	/// <param name="configPathPtr">Pointer to the config file path.</param>
	/// <param name="paramsPtr">Pointer to initialization parameters.</param>
	/// <param name="handle">Output. Host context handle.</param>
	/// <returns>Operation result.</returns>
	static abstract RuntimeCallResult Initialize(IntPtr configPathPtr, IntPtr paramsPtr, out IntPtr handle);
	/// <summary>
	/// Runs application from current context.
	/// </summary>
	/// <param name="handle">Host context handle.</param>
	/// <returns>Exit code.</returns>
	static abstract Int32 RunApp(IntPtr handle);
	/// <summary>
	/// Set error ptr.
	/// </summary>
	/// <param name="handle">Host context handle.</param>
	/// <param name="writeErrPtr">Write error delegate pointer.</param>
	static abstract void SetError(IntPtr handle, IntPtr writeErrPtr);
}