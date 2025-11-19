namespace Mxrx.NetHost;

public partial class FrameworkResolver
{
	/// <summary>
	/// Windows OS <see cref="FrameworkResolver"/> class.
	/// </summary>
#if !PACKAGE
	[SuppressMessage(Constants.CSharpSquid, Constants.CheckIdS6640,
	                 Justification = Constants.SecureUnsafeCodeJustification)]
#endif
	private sealed unsafe class Windows(IntPtr handle) : Generic<WindowsFunctions>(handle)
	{
		/// <inheritdoc/>
		public override ContextImpl Initialize(InitializationParameters parameters)
		{
			if (parameters.InitializeCommand && parameters.IsEmpty)
				base.Initialize(parameters);

			Span<ArgHandle> handles = stackalloc ArgHandle[FrameworkResolver.GetHandlesCount(parameters.Arguments)];
			fixed (Char* hostPathPtr = &FrameworkResolver.GetRef(parameters.HostPath, out Char[]? hostPathArray))
			fixed (Char* rootPathPtr = &FrameworkResolver.GetRef(parameters.RootPath, out Char[]? rootPathArray))
			fixed (Char* configPathPtr = &FrameworkResolver.GetRef(parameters.ConfigPath, out Char[]? configPathArray))
			{
				try
				{
					Boolean isCommandLine = true;
					WindowsFunctions.InitialParameters param = new()
					{
						HostPathPtr = hostPathPtr,
						RootPtr = rootPathPtr,
						Size = (UIntPtr)sizeof(WindowsFunctions.InitialParameters),
					};
					RuntimeCallResult callResult;
					HostHandle hostHandle;
					if (!parameters.InitializeCommand)
					{
						isCommandLine = false;
						callResult = this.Functions.InitializeForConfig(configPathPtr, in param, out hostHandle);
					}
					else
					{
						Span<ReadOnlyValPtr<Char>> addresses =
							stackalloc ReadOnlyValPtr<Char>[parameters.Arguments.Count];
						Int32 argCount = FrameworkResolver.LoadArgsAddr(parameters.Arguments, addresses, handles);
						callResult = this.Functions.InitializeForCommand(argCount, addresses.GetUnsafeValPtr(),
						                                                 in param, out hostHandle);
					}

					FrameworkResolver.ThrowIfInvalidResult(callResult);
					return new(this, hostHandle, isCommandLine);
				}
				finally
				{
					FrameworkResolver.Clean([hostPathArray, rootPathArray, configPathArray,], handles);
				}
			}
		}
		/// <inheritdoc/>
#if !PACKAGE
		[SuppressMessage(Constants.CSharpSquid, Constants.CheckIdS3218)]
#endif
		protected internal override IntPtr GetFunctionPointer(HostContext hostContext, NetFunctionInfo info)
		{
			IntPtr result;
			RuntimeCallResult value;
			fixed (Char* assemblyPathPtr = &FrameworkResolver.GetRef(info.AssemblyPath, out Char[]? assemblyPathArray))
			fixed (Char* typeNamePtr = &FrameworkResolver.GetRef(info.TypeName, out Char[]? typeNameArray))
			fixed (Char* methodNamePtr = &FrameworkResolver.GetRef(info.MethodName, out Char[]? methodNameArray))
			fixed (Char* delegateTypeNamePtr =
				       &FrameworkResolver.GetRef(info.DelegateTypeName, out Char[]? delegateTypeNameArray))
			{
				try
				{
					Char* delegateTypePtr = info.UseUnmanagedCallersOnly ?
						(Char*)UIntPtr.MaxValue.ToPointer() :
						delegateTypeNamePtr;

					value = info.AssemblyPath.IsEmpty ?
						FrameworkResolver.GetFunctionPointer(hostContext.GetFunctionPointerPtr.ToPointer(), typeNamePtr,
						                                     methodNamePtr, delegateTypePtr, out result) :
						FrameworkResolver.LoadAssemblyAnGetFunctionPointer(
							hostContext.LoadAssemblyAndGetFunctionPointerPtr.ToPointer(), assemblyPathPtr, typeNamePtr,
							methodNamePtr, delegateTypePtr, out result);
				}
				finally
				{
					FrameworkResolver.Clean([
						assemblyPathArray, typeNameArray, methodNameArray, delegateTypeNameArray,
					]);
				}
			}

			FrameworkResolver.ThrowIfInvalidResult(value);
			return result;
		}
		/// <inheritdoc/>
		protected internal override void LoadAssembly(HostContext hostContext, LoadAssemblyParameters parameters)
		{
			if (parameters.AssemblyPath.IsEmpty)
				base.LoadAssembly(hostContext, parameters);
			fixed (Char* assemblyPathPtr =
				       &FrameworkResolver.GetRef(parameters.AssemblyPath, out Char[]? assemblyPathArray))
			{
				try
				{
					delegate* unmanaged[Stdcall]<Char*, void*, void*, RuntimeCallResult> loadAssembly =
						(delegate* unmanaged[Stdcall]<Char*, void*, void*, RuntimeCallResult>)hostContext
							.LoadAssemblyPtr;

					RuntimeCallResult value = loadAssembly(assemblyPathPtr, default, default);
					FrameworkResolver.ThrowIfInvalidResult(value);
				}
				finally
				{
					FrameworkResolver.Clean([assemblyPathArray,]);
				}
			}
		}
		/// <inheritdoc/>
		protected internal override VolatileText GetProperty(HostContext hostContext, VolatileText propertyName)
		{
			fixed (Char* propertyNamePtr = &FrameworkResolver.GetRef(propertyName.Text, out Char[]? propertyNameArray))
			{
				try
				{
					RuntimeCallResult callResult = this.Functions.GetRuntimePropertyValue(
						hostContext.Handle, propertyNamePtr, out ReadOnlyValPtr<Char> value);
					this._clrInitialized = true;
					FrameworkResolver.ThrowIfInvalidResult(callResult);
					VolatileText result =
						VolatileText.CreateLiteral(MemoryMarshal.CreateReadOnlySpanFromNullTerminated(value));
					hostContext.Attach(ref result);
					return result;
				}
				finally
				{
					FrameworkResolver.Clean([propertyNameArray,]);
				}
			}
		}
		/// <inheritdoc/>
		protected internal override void SetProperty(HostContext hostContext, VolatileText propertyName,
			VolatileText propertyValue)
		{
			fixed (Char* propertyNamePtr = &FrameworkResolver.GetRef(propertyName.Text, out Char[]? propertyNameArray))
			fixed (Char* propertyValuePtr =
				       &FrameworkResolver.GetRef(propertyValue.Text, out Char[]? propertyValueArray))
			{
				try
				{
					RuntimeCallResult callResult = this.Functions.SetRuntimePropertyValue(
						hostContext.Handle, propertyNamePtr, propertyValuePtr);
					this._clrInitialized = true;
					FrameworkResolver.ThrowIfInvalidResult(callResult);
				}
				finally
				{
					FrameworkResolver.Clean([propertyNameArray, propertyValueArray,]);
				}
			}
		}

		/// <summary>
		/// Creates a <see cref="FrameworkResolver"/> from <see cref="GetHostPathParameters"/> instance.
		/// </summary>
		/// <param name="parameters">Host path parameters.</param>
		/// <returns>A <see cref="FrameworkResolver"/> instance from <paramref name="parameters"/>.</returns>
		public static Windows Create(GetHostPathParameters parameters)
		{
			String libraryPath = parameters.IsEmpty ? Windows.GetLibraryPath() : Windows.GetLibraryPath(parameters);
			IntPtr libHandle = NativeLibrary.Load(libraryPath);
			return new(libHandle);
		}

		/// <summary>
		/// Retrieves the default library path.
		/// </summary>
		/// <returns>The default host library path.</returns>
		private static String GetLibraryPath()
		{
			UIntPtr pathLength = FrameworkResolver.GetHostPathLength(in Unsafe.NullRef<HostPathParameters>());
			Span<Char> chars = stackalloc Char[(Int32)pathLength];
			fixed (void* charsPtr = &MemoryMarshal.GetReference(chars))
			{
				RuntimeCallResult callResult =
					FrameworkResolver.GetHostPath(charsPtr, ref pathLength, in Unsafe.NullRef<HostPathParameters>());
				FrameworkResolver.ThrowIfInvalidResult(callResult);
			}
			return new(chars);
		}
		/// <summary>
		/// Retrieves the library path compatible with <paramref name="parameters"/> instance.
		/// </summary>
		/// <returns>A host library path.</returns>
		private static String GetLibraryPath(GetHostPathParameters parameters)
		{
			fixed (Char* assemblyPathPtr =
				       &FrameworkResolver.GetRef(parameters.AssemblyPath, out Char[]? assemblyPathArray))
			fixed (Char* rootPathPtr = &FrameworkResolver.GetRef(parameters.RootPath, out Char[]? rootPathArray))
			{
				try
				{
					HostPathParameters hostPathParameters = new(assemblyPathPtr, rootPathPtr);
					UIntPtr pathLength = FrameworkResolver.GetHostPathLength(in hostPathParameters);
					Span<Char> chars = stackalloc Char[(Int32)pathLength];
					fixed (void* charsPtr = &MemoryMarshal.GetReference(chars))
					{
						RuntimeCallResult callResult =
							FrameworkResolver.GetHostPath(charsPtr, ref pathLength, in hostPathParameters);
						FrameworkResolver.ThrowIfInvalidResult(callResult);
					}
					return new(chars);
				}
				finally
				{
					FrameworkResolver.Clean([assemblyPathArray, rootPathArray,]);
				}
			}
		}
	}
}