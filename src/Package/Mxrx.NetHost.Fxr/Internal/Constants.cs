namespace Mxrx.NetHost.Internal;

/// <summary>
/// Constants.
/// </summary>
internal static class Constants
{
	/// <summary>
	/// Name of <c>hostfxr_close</c>.
	/// </summary>
	public const String CloseHandle = "hostfxr_close";
	/// <summary>
	/// Name of <c>hostfxr_get_runtime_delegate</c>.
	/// </summary>
	public const String GetDelegate = "hostfxr_get_runtime_delegate";
	/// <summary>
	/// Name of <c>hostfxr_get_runtime_properties</c>.
	/// </summary>
	public const String GetRuntimeProperties = "hostfxr_get_runtime_properties";
	/// <summary>
	/// Name of <c>hostfxr_get_runtime_property_value</c>.
	/// </summary>
	public const String GetRuntimePropertyValue = "hostfxr_get_runtime_property_value";
	/// <summary>
	/// Name of <c>hostfxr_set_runtime_property_value</c>.
	/// </summary>
	public const String SetRuntimePropertyValue = "hostfxr_set_runtime_property_value";
	/// <summary>
	/// Name of <c>hostfxr_initialize_for_dotnet_command_line</c>.
	/// </summary>
	public const String InitializeForCommand = "hostfxr_initialize_for_dotnet_command_line";
	/// <summary>
	/// Name of <c>hostfxr_initialize_for_runtime_config</c>.
	/// </summary>
	public const String InitializeForConfig = "hostfxr_initialize_for_runtime_config";
	/// <summary>
	/// Name of <c>hostfxr_main</c>.
	/// </summary>
	public const String Main = "hostfxr_main";
	/// <summary>
	/// Name of <c>hostfxr_main_startupinfo</c>.
	/// </summary>
	public const String MainStartupInfo = "hostfxr_main_startupinfo";
	/// <summary>
	/// Name of <c>hostfxr_run_app</c>.
	/// </summary>
	public const String RunApp = "hostfxr_run_app";
	/// <summary>
	/// Name of <c>hostfxr_set_error_writer</c>.
	/// </summary>
	public const String SetErrorWriter = "hostfxr_set_error_writer";
#if !PACKAGE
	public const String CSharpSquid = "csharpsquid";
	public const String CheckIdS6640 = "S6640";
	public const String CheckIdS3881 = "S3881";
	public const String CheckIdS3776 = "S3776";
	public const String CheckIdS3459 = "S3459";
	public const String CheckIdS3358 = "S3358";
	public const String CheckIdS1144 = "S1144";
	public const String SecureUnsafeCodeJustification = "Interop code is secure to use.";
	public const String BinaryStructJustification = "This struct is created only by binary operations.";
	public const String OptimizedJustification = "The method code is optimized.";
#endif
}