namespace Mxrx.NetHost.Internal;

/// <summary>
/// Native <c>Hostfxr</c> library exported methods.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
#if !PACKAGE
[SuppressMessage(Constants.CSharpSquid, Constants.CheckIdS6640,
                 Justification = Constants.SecureUnsafeCodeJustification)]
#endif
internal readonly unsafe struct NativeFunctionSet : IFunctionSet
{
	/// <inheritdoc/>
	public static Int32 SizeOf => sizeof(NativeFunctionSet);

	/// <summary>
	/// Address of <c>hostfxr_close</c>.
	/// </summary>
	public readonly delegate* unmanaged<HostHandle, RuntimeCallResult> CloseHandle;
	/// <summary>
	/// Address of <c>hostfxr_get_runtime_delegate</c>.
	/// </summary>
	public readonly delegate* unmanaged<HostHandle, RuntimeDelegateType, out IntPtr, RuntimeCallResult> GetDelegate;
	/// <summary>
	/// Address of <c>hostfxr_get_runtime_properties</c>.
	/// </summary>
	public readonly delegate* unmanaged<HostHandle, ref UIntPtr, ref ReadOnlyValPtr<NativeCharPointer>, ref
		ReadOnlyValPtr<NativeCharPointer>, RuntimeCallResult> GetRuntimeProperties;
	/// <summary>
	/// Address of <c>hostfxr_get_runtime_property_value</c>.
	/// </summary>
	public readonly delegate* unmanaged<HostHandle, NativeCharPointer, out NativeCharPointer, RuntimeCallResult>
		GetRuntimePropertyValue;
	/// <summary>
	/// Address of <c>hostfxr_set_runtime_property_value</c>.
	/// </summary>
	public readonly delegate* unmanaged<HostHandle, NativeCharPointer, NativeCharPointer, RuntimeCallResult>
		SetRuntimePropertyValue;
	/// <summary>
	/// Address of <c>hostfxr_initialize_for_dotnet_command_line</c>.
	/// </summary>
	public readonly delegate* unmanaged<Int32, NativeCharPointer*, in InitParameters, out HostHandle, RuntimeCallResult>
		InitializeForCommand;
	/// <summary>
	/// Address of <c>hostfxr_initialize_for_runtime_config</c>.
	/// </summary>
	public readonly delegate* unmanaged<NativeCharPointer, in InitParameters, out HostHandle, RuntimeCallResult>
		InitializeForConfig;
	/// <summary>
	/// Address of <c>hostfxr_run_app</c>.
	/// </summary>
	public readonly delegate* unmanaged<HostHandle, Int32> RunApp;
	/// <summary>
	/// Address of <c>hostfxr_set_error_writer</c>.
	/// </summary>
	public readonly delegate* unmanaged<HostHandle, void*, void*> SetErrorWriter;
#if !DISABLE_MAIN_CALLS
	/// <summary>
	/// Address of <c>hostfxr_main</c>.
	/// </summary>
	public readonly delegate* unmanaged<Int32, IntPtr*, RuntimeCallResult> Main;
	/// <summary>
	/// Address of <c>hostfxr_main_startupinfo</c>.
	/// </summary>
	public readonly delegate* unmanaged<Int32, IntPtr*, IntPtr, IntPtr, IntPtr, RuntimeCallResult> MainStartupInfo;
#endif

	RuntimeCallResult IFunctionSet.CloseContext(HostHandle handle) => this.CloseHandle(handle);
	RuntimeCallResult IFunctionSet.GetFunctionPointer(HostHandle handle, RuntimeDelegateType delegateType,
		out IntPtr funcPtr)
		=> this.GetDelegate(handle, delegateType, out funcPtr);
	RuntimeCallResult IFunctionSet.GetRuntimeProperties(HostHandle handle, ref UIntPtr propCount,
		out ReadOnlyValPtr<NativeCharPointer> propKeysPtr, out ReadOnlyValPtr<NativeCharPointer> propValuesPtr)
	{
		Unsafe.SkipInit(out propKeysPtr);
		Unsafe.SkipInit(out propValuesPtr);
		return this.GetRuntimeProperties(handle, ref propCount, ref propKeysPtr, ref propValuesPtr);
	}
	RuntimeCallResult IFunctionSet.CountRuntimeProperties(HostHandle handle, out UIntPtr propCount)
	{
		Unsafe.SkipInit(out propCount);
		ref ReadOnlyValPtr<NativeCharPointer> nullRef = ref Unsafe.NullRef<ReadOnlyValPtr<NativeCharPointer>>();
		return this.GetRuntimeProperties(handle, ref propCount, ref nullRef, ref nullRef);
	}
	RuntimeCallResult IFunctionSet.GetRuntimePropertyValue(HostHandle handle, NativeCharPointer keyPtr,
		out NativeCharPointer valuePtr)
		=> this.GetRuntimePropertyValue(handle, keyPtr, out valuePtr);
	RuntimeCallResult IFunctionSet.SetRuntimePropertyValue(HostHandle handle, NativeCharPointer keyPtr,
		NativeCharPointer valuePtr)
		=> this.SetRuntimePropertyValue(handle, keyPtr, valuePtr);
	RuntimeCallResult IFunctionSet.Initialize(Int32 argsCount, NativeCharPointer* argsPtr, in InitParameters initParams,
		out HostHandle handle)
		=> this.InitializeForCommand(argsCount, argsPtr, in initParams, out handle);
	RuntimeCallResult IFunctionSet.Initialize(NativeCharPointer configPathPtr, in InitParameters initParams,
		out HostHandle handle)
		=> this.InitializeForConfig(configPathPtr, in initParams, out handle);
	Int32 IFunctionSet.RunApp(HostHandle handle) => this.RunApp(handle);
	void* IFunctionSet.SetError(HostHandle handle, void* writeErrPtr) => this.SetErrorWriter(handle, writeErrPtr);

	/// <summary>
	/// Gets the address of the exported symbols using them to initialize <paramref name="functionSet"/> and
	/// returns a value that indicates the number of exported symbols used.
	/// </summary>
	/// <param name="libraryHandle">Handle to library.</param>
	/// <param name="functionSet">Output. Initialized <see cref="NativeFunctionSet"/> instance.</param>
	/// <returns>Number of native symbols used to initialize <paramref name="functionSet"/>.</returns>
	public static Int32 GetExport(IntPtr libraryHandle, out NativeFunctionSet functionSet)
	{
		functionSet = default;
		Span<IntPtr> functions = MemoryMarshal.CreateSpan(ref functionSet, 1).AsValues<NativeFunctionSet, IntPtr>();
		Int32 result = 0;

		NativeFunctionSet.GetAddress(libraryHandle, functions, IFrameworkResolverLib.CloseHandleSymbol, result++);
		NativeFunctionSet.GetAddress(libraryHandle, functions, IFrameworkResolverLib.GetDelegateSymbol, result++);
		NativeFunctionSet.GetAddress(libraryHandle, functions, IFrameworkResolverLib.GetRuntimePropertiesSymbol,
		                             result++);
		NativeFunctionSet.GetAddress(libraryHandle, functions, IFrameworkResolverLib.GetRuntimePropertyValueSymbol,
		                             result++);
		NativeFunctionSet.GetAddress(libraryHandle, functions, IFrameworkResolverLib.SetRuntimePropertyValueSymbol,
		                             result++);
		NativeFunctionSet.GetAddress(libraryHandle, functions, IFrameworkResolverLib.InitializeForCommandSymbol,
		                             result++);
		NativeFunctionSet.GetAddress(libraryHandle, functions, IFrameworkResolverLib.InitializeForConfigSymbol,
		                             result++);
#if !DISABLE_MAIN_CALLS
		NativeFunctionSet.GetAddress(libraryHandle, functions, IFrameworkResolverLib.MainSymbol, result++);
		NativeFunctionSet.GetAddress(libraryHandle, functions, IFrameworkResolverLib.MainStartupInfoSymbol, result++);
#endif
		NativeFunctionSet.GetAddress(libraryHandle, functions, IFrameworkResolverLib.RunAppSymbol, result++);
		NativeFunctionSet.GetAddress(libraryHandle, functions, IFrameworkResolverLib.SetErrorWriterSymbol, result++);
		return result;
	}

	/// <summary>
	/// Retrieves address of an exported method.
	/// </summary>
	/// <param name="handle">Handle to library.</param>
	/// <param name="functions">A <see cref="IntPtr"/> span.</param>
	/// <param name="methodName">Exported symbol name.</param>
	/// <param name="index">Span index destination.</param>
	/// <exception cref="ArgumentException">If <paramref name="methodName"/> symbol was not found.</exception>
	private static void GetAddress(IntPtr handle, Span<IntPtr> functions, String methodName, Int32 index)
	{
		if (NativeLibrary.TryGetExport(handle, methodName, out functions[index])) return;
		IMessageResource resource = IMessageResource.GetInstance();
		throw new ArgumentException(resource.InvalidLibrary(methodName));
	}
}