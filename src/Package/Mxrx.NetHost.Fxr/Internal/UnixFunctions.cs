namespace Mxrx.NetHost.Internal;

/// <summary>
/// <c>Hostfxr</c> library exported methods.
/// </summary>
/// <remarks>This struct is only for Unix-like OS</remarks>
[StructLayout(LayoutKind.Sequential)]
#if !PACKAGE
[ExcludeFromCodeCoverage]
[SuppressMessage(Constants.CSharpSquid, Constants.CheckIdS6640,
                 Justification = Constants.SecureUnsafeCodeJustification)]
#endif
internal readonly unsafe struct UnixFunctions : IResolverFunctions
{
	/// <inheritdoc/>
	public static Int32 SizeOf => sizeof(UnixFunctions);
	/// <inheritdoc/>
	public static Type CharType => typeof(Byte);

	/// <summary>
	/// Address of <c>hostfxr_close</c>.
	/// </summary>
	public readonly delegate* unmanaged<HostHandle, RuntimeCallResult> CloseHandle;
	/// <summary>
	/// Address of <c>hostfxr_get_runtime_delegate</c>.
	/// </summary>
	public readonly delegate* unmanaged<HostHandle, RuntimeDelegateType, out void*, RuntimeCallResult> GetDelegate;
#if !DISABLE_RUNTIME_PROPERTIES
	/// <summary>
	/// Address of <c>hostfxr_get_runtime_properties</c>.
	/// </summary>
	public readonly delegate* unmanaged<HostHandle, ref UIntPtr, ref ReadOnlyValPtr<ReadOnlyValPtr<Byte>>, ref
		ReadOnlyValPtr<ReadOnlyValPtr<Byte>>, RuntimeCallResult> GetRuntimeProperties;
#endif
	/// <summary>
	/// Address of <c>hostfxr_get_runtime_property_value</c>.
	/// </summary>
	public readonly delegate* unmanaged<HostHandle, ReadOnlyValPtr<Byte>, out ReadOnlyValPtr<Byte>, RuntimeCallResult>
		GetRuntimePropertyValue;
	/// <summary>
	/// Address of <c>hostfxr_set_runtime_property_value</c>.
	/// </summary>
	public readonly delegate* unmanaged<HostHandle, ReadOnlyValPtr<Byte>, ReadOnlyValPtr<Byte>, RuntimeCallResult>
		SetRuntimePropertyValue;
	/// <summary>
	/// Address of <c>hostfxr_initialize_for_dotnet_command_line</c>.
	/// </summary>
	public readonly delegate* unmanaged<Int32, ReadOnlyValPtr<ReadOnlyValPtr<Byte>>, in InitialParameters, out
		HostHandle, RuntimeCallResult> InitializeForCommand;
	/// <summary>
	/// Address of <c>hostfxr_initialize_for_runtime_config</c>.
	/// </summary>
	public readonly delegate* unmanaged<ReadOnlyValPtr<Byte>, in InitialParameters, out HostHandle, RuntimeCallResult>
		InitializeForConfig;
#if !DISABLE_MAIN_CALLS
	/// <summary>
	/// Address of <c>hostfxr_main</c>.
	/// </summary>
	public readonly delegate* unmanaged<Int32, ReadOnlyValPtr<ReadOnlyValPtr<Byte>>, RuntimeCallResult> Main;
	/// <summary>
	/// Address of <c>hostfxr_main_startupinfo</c>.
	/// </summary>
	public readonly delegate* unmanaged<Int32, ReadOnlyValPtr<ReadOnlyValPtr<Byte>>, ReadOnlyValPtr<Byte>,
		ReadOnlyValPtr<Byte>, ReadOnlyValPtr<Byte>, RuntimeCallResult> MainStartupInfo;
#endif
	/// <summary>
	/// Address of <c>hostfxr_run_app</c>.
	/// </summary>
	public readonly delegate* unmanaged<HostHandle, Int32> RunApp;
	/// <summary>
	/// Address of <c>hostfxr_set_error_writer</c>.
	/// </summary>
	public readonly delegate* unmanaged<HostHandle, void*, void*> SetErrorWriter;

	RuntimeCallResult IResolverFunctions.CloseContext(HostHandle handle) => this.CloseHandle(handle);
	RuntimeCallResult IResolverFunctions.Initialize(out HostHandle handle)
		=> this.InitializeForCommand(0, default, in Unsafe.NullRef<InitialParameters>(), out handle);
	void IResolverFunctions.SetError(HostHandle handle, IntPtr writeErrPtr)
		=> this.SetErrorWriter(handle, writeErrPtr.ToPointer());
	Int32 IResolverFunctions.RunApp(HostHandle handle) => this.RunApp(handle);
	RuntimeCallResult IResolverFunctions.GetFunctionPointer(HostHandle handle, RuntimeDelegateType delegateType,
		out void* funcPtr)
		=> this.GetDelegate(handle, delegateType, out funcPtr);

	/// <summary>
	/// Initial Unix host parameters.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public readonly struct InitialParameters()
	{
		public UIntPtr Size { get; init; } = (UIntPtr)sizeof(InitialParameters);
		public ReadOnlyValPtr<Byte> HostPathPtr { get; init; }
		public ReadOnlyValPtr<Byte> RootPtr { get; init; }
	}
}