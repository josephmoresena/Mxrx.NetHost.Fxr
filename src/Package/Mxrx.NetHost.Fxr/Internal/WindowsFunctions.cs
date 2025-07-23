namespace Mxrx.NetHost.Internal;

/// <summary>
/// <c>Hostfxr</c> library exported methods.
/// </summary>
/// <remarks>This struct is only for Windows OS</remarks>
[StructLayout(LayoutKind.Sequential)]
#if !PACKAGE
[ExcludeFromCodeCoverage]
[SuppressMessage(Constants.CSharpSquid, Constants.CheckIdS6640,
                 Justification = Constants.SecureUnsafeCodeJustification)]
#endif
internal readonly unsafe struct WindowsFunctions : IResolverFunctions
{
	/// <inheritdoc/>
	public static Type CharType => typeof(Byte);

	/// <summary>
	/// Address of <c>hostfxr_close</c>.
	/// </summary>
	public readonly delegate* unmanaged[Cdecl]<HostHandle, RuntimeCallResult> CloseHandle;
	/// <summary>
	/// Address of <c>hostfxr_get_runtime_delegate</c>.
	/// </summary>
	public readonly delegate* unmanaged[Cdecl]<HostHandle, RuntimeDelegateType, out void*, RuntimeCallResult>
		GetDelegate;
	/// <summary>
	/// Address of <c>hostfxr_get_runtime_properties</c>.
	/// </summary>
	public readonly delegate* unmanaged[Cdecl]<HostHandle, ref UIntPtr, ValPtr<ReadOnlyValPtr<Char>>,
		ValPtr<ReadOnlyValPtr<Char>>, RuntimeCallResult> GetRuntimeProperties;
	/// <summary>
	/// Address of <c>hostfxr_get_runtime_property_value</c>.
	/// </summary>
	public readonly delegate* unmanaged[Cdecl]<HostHandle, ReadOnlyValPtr<Char>, out ReadOnlyValPtr<Char>,
		RuntimeCallResult> GetRuntimePropertyValue;
	/// <summary>
	/// Address of <c>hostfxr_set_runtime_property_value</c>.
	/// </summary>
	public readonly delegate* unmanaged[Cdecl]<HostHandle, ReadOnlyValPtr<Char>, ReadOnlyValPtr<Char>, RuntimeCallResult
		> SetRuntimePropertyValue;
	/// <summary>
	/// Address of <c>hostfxr_initialize_for_dotnet_command_line</c>.
	/// </summary>
	public readonly delegate* unmanaged[Cdecl]<Int32, ReadOnlyValPtr<ReadOnlyValPtr<Char>>, in InitialParameters, out
		HostHandle, RuntimeCallResult> InitializeForCommand;
	/// <summary>
	/// Address of <c>hostfxr_initialize_for_runtime_config</c>.
	/// </summary>
	public readonly delegate* unmanaged[Cdecl]<ReadOnlyValPtr<Char>, in InitialParameters, out HostHandle,
		RuntimeCallResult> InitializeForConfig;
	/// <summary>
	/// Address of <c>hostfxr_main</c>.
	/// </summary>
	public readonly delegate* unmanaged[Cdecl]<Int32, ReadOnlyValPtr<ReadOnlyValPtr<Char>>, RuntimeCallResult> Main;
	/// <summary>
	/// Address of <c>hostfxr_main_startupinfo</c>.
	/// </summary>
	public readonly delegate* unmanaged[Cdecl]<Int32, ReadOnlyValPtr<ReadOnlyValPtr<Char>>, ReadOnlyValPtr<Char>,
		ReadOnlyValPtr<Char>, ReadOnlyValPtr<Char>, RuntimeCallResult> MainStartupInfo;
	/// <summary>
	/// Address of <c>hostfxr_run_app</c>.
	/// </summary>
	public readonly delegate* unmanaged[Cdecl]<HostHandle, Int32> RunApp;
	/// <summary>
	/// Address of <c>hostfxr_set_error_writer</c>.
	/// </summary>
	public readonly delegate* unmanaged[Cdecl]<HostHandle, void*, void*> SetErrorWriter;

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
	/// Initial windows host parameters.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public readonly struct InitialParameters()
	{
		public UIntPtr Size { get; init; } = (UIntPtr)sizeof(InitialParameters);
		public ReadOnlyValPtr<Char> HostPathPtr { get; init; }
		public ReadOnlyValPtr<Char> RootPtr { get; init; }
	}
}