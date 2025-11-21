namespace Mxrx.NetHost;

#if !PACKAGE
[SuppressMessage(Constants.CSharpSquid, Constants.CheckIdS6640,
                 Justification = Constants.SecureUnsafeCodeJustification)]
#endif
public abstract unsafe partial class FrameworkResolver
{
	/// <summary>
	/// Indicates whether an application was started.
	/// </summary>
	private static Boolean applicationStarted;
	/// <summary>
	/// Loaded resolver instance.
	/// </summary>
	private static FrameworkResolver? loadedResolver;

	/// <summary>
	/// Throws an exception if current platform is not Native AOT.
	/// </summary>
	/// <exception cref="PlatformNotSupportedException">
	/// Throws an exception if current platform is not Native AOT.
	/// </exception>
	private static void ThrowIfNotNativeAot()
	{
		if (AotInfo.IsNativeAot) return;
		IMessageResource resource = IMessageResource.GetInstance();
		throw new PlatformNotSupportedException(resource.AotRequired);
	}
	/// <summary>
	/// Throws an exception if exists an active framework resolver.
	/// </summary>
	/// <exception cref="InvalidOperationException">
	/// Throws an exception if exists an active framework resolver.
	/// </exception>
	private static void ThrowIfInitializedHost()
	{
		if (FrameworkResolver.loadedResolver is null || FrameworkResolver.loadedResolver._isDisposed) return;
		IMessageResource resource = IMessageResource.GetInstance();
		throw new InvalidOperationException(resource.ActiveFrameworkResolver);
	}
	/// <summary>
	/// Throws an exception if exists an active framework resolver.
	/// </summary>
	/// <param name="resolverKey">Resolver key.</param>
	/// <exception cref="InvalidOperationException">
	/// Throws an exception if exists an active framework resolver.
	/// </exception>
	private static void ThrowIfInitializedHost<TKey>(TKey resolverKey)
	{
		if (FrameworkResolver.loadedResolver is null || FrameworkResolver.loadedResolver._isDisposed) return;
		if (FrameworkResolver.loadedResolver.ResolverKey.Equals(resolverKey)) return;
		IMessageResource resource = IMessageResource.GetInstance();
		throw new InvalidOperationException(resource.ActiveFrameworkResolver);
	}
	/// <summary>
	/// Throws if invalid result.
	/// </summary>
	/// <param name="callResult">A <see cref="RuntimeCallResult"/> value.</param>
	/// <exception cref="InvalidOperationException">Throws if invalid result.</exception>
	private static void ThrowIfInvalidResult(RuntimeCallResult callResult)
	{
		if (callResult is RuntimeCallResult.Success) return;
		IMessageResource resource = IMessageResource.GetInstance();
		throw new InvalidOperationException(resource.InvalidResult(callResult));
	}
	/// <summary>
	/// Creates a <see cref="FrameworkResolver"/> instance from <paramref name="libHandle"/>.
	/// </summary>
	/// <param name="libHandle">Framework resolver library handle.</param>
	/// <param name="libPath">Framework resolver library path.</param>
	/// <returns>A <see cref="FrameworkResolver"/> instance from <paramref name="libHandle"/>.</returns>
	private static FrameworkResolver CreateResolver(IntPtr libHandle, String? libPath = default)
	{
		if (FrameworkResolver.loadedResolver is null || FrameworkResolver.loadedResolver._isDisposed)
			FrameworkResolver.loadedResolver =
				new Impl<NativeFunctionSet>(libHandle, NativeFunctionSet.GetExport, libPath);
		return FrameworkResolver.loadedResolver;
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
	/// <summary>
	/// Retrieves the number of handles required for <paramref name="args"/> values.
	/// </summary>
	/// <param name="args">A <see cref="ArgumentsParameter"/> instance.</param>
	/// <returns>The number of handles required for <paramref name="args"/> values.</returns>
	private static Int32 GetHandlesCount(ArgumentsParameter args)
	{
		if (args.IsEmpty)
			return 0;
		return args.Sequence is not null ? 1 : args.Values.Length;
	}
	/// <summary>
	/// Invokes load_assembly_and_get_function_pointer_fn.
	/// </summary>
	/// <param name="funcPtr">A pointer to get_function_pointer_fn.</param>
	/// <param name="assemblyPathPtr">Assembly path pointer.</param>
	/// <param name="typeNamePtr">Type name pointer.</param>
	/// <param name="methodNamePtr">Method name pointer.</param>
	/// <param name="delegateTypePtr">Delegate type name pointer.</param>
	/// <param name="resultPtr">Output. Resulting function pointer.</param>
	/// <returns>A <see cref="RuntimeCallResult"/> value.</returns>
	private static RuntimeCallResult LoadAssemblyAndGetFunctionPointer(IntPtr funcPtr, NativeChar* assemblyPathPtr,
		NativeChar* typeNamePtr, NativeChar* methodNamePtr, NativeChar* delegateTypePtr, out IntPtr resultPtr)
	{
		delegate* unmanaged[Stdcall]<NativeChar*, NativeChar*, NativeChar*, NativeChar*, void*, out IntPtr,
			RuntimeCallResult> loadAndGetFunctionPtr =
				(delegate* unmanaged[Stdcall]<NativeChar*, NativeChar*, NativeChar*, NativeChar*, void*, out IntPtr,
					RuntimeCallResult>)funcPtr;

		return loadAndGetFunctionPtr(assemblyPathPtr, typeNamePtr, methodNamePtr, delegateTypePtr, default,
		                             out resultPtr);
	}
	/// <summary>
	/// Invokes <c>get_function_pointer_fn</c>.
	/// </summary>
	/// <param name="funcPtr">A pointer to get_function_pointer_fn.</param>
	/// <param name="typeNamePtr">Type name pointer.</param>
	/// <param name="methodNamePtr">Method name pointer.</param>
	/// <param name="delegateTypePtr">Delegate type name pointer.</param>
	/// <param name="resultPtr">Output. Resulting function pointer.</param>
	/// <returns>A <see cref="RuntimeCallResult"/> value.</returns>
	private static RuntimeCallResult GetFunctionPointer(IntPtr funcPtr, NativeChar* typeNamePtr,
		NativeChar* methodNamePtr, NativeChar* delegateTypePtr, out IntPtr resultPtr)
	{
		delegate* unmanaged[Stdcall]<NativeChar*, NativeChar*, NativeChar*, void*, void*, out IntPtr, RuntimeCallResult>
			getFunctionPointerPtr =
				(delegate* unmanaged[Stdcall]<NativeChar*, NativeChar*, NativeChar*, void*, void*, out IntPtr,
					RuntimeCallResult>)funcPtr;

		return getFunctionPointerPtr(typeNamePtr, methodNamePtr, delegateTypePtr, default, default, out resultPtr);
	}
	/// <summary>
	/// Loads an assembly from <paramref name="parameters"/>.
	/// </summary>
	/// <param name="funcPtr">A pointer to <c>hdt_load_assembly_byte</c>.</param>
	/// <param name="parameters">A <see cref="LoadAssemblyParameters"/> instance.</param>
	/// <returns>A <see cref="RuntimeCallResult"/> value.</returns>
	private static RuntimeCallResult LoadAssemblyFromBytes(IntPtr funcPtr, LoadAssemblyParameters parameters)
	{
		fixed (Byte* assemblyPtr = &MemoryMarshal.GetReference(parameters.AssemblyBytes))
		fixed (Byte* symbolPtr = &MemoryMarshal.GetReference(parameters.SymbolsBytes))
		{
			delegate* unmanaged[Stdcall]<Byte*, UIntPtr, Byte*, UIntPtr, void*, void*, RuntimeCallResult>
				loadAssemblyFromBytes =
					(delegate* unmanaged[Stdcall]<Byte*, UIntPtr, Byte*, UIntPtr, void*, void*, RuntimeCallResult>)
					funcPtr;
			return loadAssemblyFromBytes(assemblyPtr, (UIntPtr)parameters.AssemblyBytes.Length,
			                             !parameters.SymbolsBytes.IsEmpty ? symbolPtr : default,
			                             (UIntPtr)parameters.SymbolsBytes.Length, default, default);
		}
	}
	/// <summary>
	/// Loads an assembly from <paramref name="assemblyPathPtr"/>.
	/// </summary>
	/// <param name="funcPtr">A pointer to <c>hdt_load_assembly</c>.</param>
	/// <param name="assemblyPathPtr">Pointer to assembly path.</param>
	/// <returns>A <see cref="RuntimeCallResult"/> value.</returns>
	private static RuntimeCallResult LoadAssemblyFromPath(IntPtr funcPtr, NativeChar* assemblyPathPtr)
	{
		delegate* unmanaged[Stdcall]<NativeChar*, void*, void*, RuntimeCallResult> loadAssembly =
			(delegate* unmanaged[Stdcall]<NativeChar*, void*, void*, RuntimeCallResult>)funcPtr;
		return loadAssembly(assemblyPathPtr, default, default);
	}
	/// <summary>
	/// Retrieves the default library path.
	/// </summary>
	/// <returns>The default host library path.</returns>
	private static String GetLibraryPath()
	{
		UIntPtr pathLength = FrameworkResolver.GetHostPathLength(in Unsafe.NullRef<HostPathParameters>());
		Span<NativeChar> chars = stackalloc NativeChar[(Int32)pathLength * NativeCharPointer.CharSize];
		fixed (void* charsPtr = &MemoryMarshal.GetReference(chars))
		{
			RuntimeCallResult callResult =
				FrameworkResolver.GetHostPath(charsPtr, ref pathLength, in Unsafe.NullRef<HostPathParameters>());
			FrameworkResolver.ThrowIfInvalidResult(callResult);
		}
		return TextHelper.Instance.GetString(chars);
	}
	/// <summary>
	/// Retrieves the library path compatible with <paramref name="parameters"/> instance.
	/// </summary>
	/// <returns>A host library path.</returns>
	private static String GetLibraryPath(GetHostPathParameters parameters)
	{
		fixed (NativeChar* assemblyPathPtr =
			       &TextHelper.Instance.GetRef(parameters.AssemblyPath, out Array? assemblyPathArray))
		fixed (NativeChar* rootPathPtr = &TextHelper.Instance.GetRef(parameters.RootPath, out Array? rootPathArray))
		{
			try
			{
				HostPathParameters hostPathParameters = new(assemblyPathPtr, rootPathPtr);
				UIntPtr pathLength = FrameworkResolver.GetHostPathLength(in hostPathParameters);
				Span<NativeChar> chars = stackalloc NativeChar[(Int32)pathLength * NativeCharPointer.CharSize];
				fixed (void* charsPtr = &MemoryMarshal.GetReference(chars))
				{
					RuntimeCallResult callResult =
						FrameworkResolver.GetHostPath(charsPtr, ref pathLength, in hostPathParameters);
					FrameworkResolver.ThrowIfInvalidResult(callResult);
				}
				return TextHelper.Instance.GetString(chars);
			}
			finally
			{
				TextHelper.Instance.Clean([assemblyPathArray, rootPathArray,]);
			}
		}
	}
}