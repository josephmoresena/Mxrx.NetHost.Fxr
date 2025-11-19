namespace Mxrx.NetHost;

public partial class FrameworkResolver
{
	/// <summary>
	/// Unix-like OS <see cref="FrameworkResolver"/> class.
	/// </summary>
#if !PACKAGE
	[SuppressMessage(Constants.CSharpSquid, Constants.CheckIdS6640,
	                 Justification = Constants.SecureUnsafeCodeJustification)]
#endif
	private sealed unsafe class Unix(IntPtr handle) : Generic<UnixFunctions>(handle)
	{
		/// <inheritdoc/>
		public override ContextImpl Initialize(InitializationParameters parameters)
		{
			if (parameters.InitializeCommand && parameters.IsEmpty)
				base.Initialize(parameters);

			Span<ArgHandle> handles = stackalloc ArgHandle[FrameworkResolver.GetHandlesCount(parameters.Arguments)];
			fixed (Byte* hostPathPtr = &FrameworkResolver.GetRef(parameters.HostPath, out Byte[]? hostPathArray))
			fixed (Byte* rootPathPtr = &FrameworkResolver.GetRef(parameters.RootPath, out Byte[]? rootPathArray))
			fixed (Byte* configPathPtr = &FrameworkResolver.GetRef(parameters.ConfigPath, out Byte[]? configPathArray))
			{
				try
				{
					Boolean isCommandLine = true;
					UnixFunctions.InitialParameters param = new()
					{
						HostPathPtr = hostPathPtr,
						RootPtr = rootPathPtr,
						Size = (UIntPtr)sizeof(UnixFunctions.InitialParameters),
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
						Span<ReadOnlyValPtr<Byte>> addresses =
							stackalloc ReadOnlyValPtr<Byte>[parameters.Arguments.Count];
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
			fixed (Byte* assemblyPathPtr = &FrameworkResolver.GetRef(info.AssemblyPath, out Byte[]? assemblyPathArray))
			fixed (Byte* typeNamePtr = &FrameworkResolver.GetRef(info.TypeName, out Byte[]? typeNameArray))
			fixed (Byte* methodNamePtr = &FrameworkResolver.GetRef(info.MethodName, out Byte[]? methodNameArray))
			fixed (Byte* delegateTypeNamePtr =
				       &FrameworkResolver.GetRef(info.DelegateTypeName, out Byte[]? delegateTypeNameArray))
			{
				try
				{
					Byte* delegateTypePtr = info.UseUnmanagedCallersOnly ?
						(Byte*)UIntPtr.MaxValue.ToPointer() :
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
			fixed (Byte* assemblyPathPtr =
				       &FrameworkResolver.GetRef(parameters.AssemblyPath, out Byte[]? assemblyPathArray))
			{
				try
				{
					delegate* unmanaged[Stdcall]<Byte*, void*, void*, RuntimeCallResult> loadAssembly =
						(delegate* unmanaged[Stdcall]<Byte*, void*, void*, RuntimeCallResult>)hostContext
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
			fixed (Byte* propertyNamePtr = &FrameworkResolver.GetRef(propertyName.Text, out Byte[]? propertyNameArray))
			{
				try
				{
					RuntimeCallResult callResult = this.Functions.GetRuntimePropertyValue(
						hostContext.Handle, propertyNamePtr, out ReadOnlyValPtr<Byte> value);
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
			fixed (Byte* propertyNamePtr = &FrameworkResolver.GetRef(propertyName.Text, out Byte[]? propertyNameArray))
			fixed (Byte* propertyValuePtr =
				       &FrameworkResolver.GetRef(propertyValue.Text, out Byte[]? propertyValueArray))
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
		public static Unix Create(GetHostPathParameters parameters)
		{
			String libraryPath = parameters.IsEmpty ? Unix.GetLibraryPath() : Unix.GetLibraryPath(parameters);
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
			Span<Byte> bytes = stackalloc Byte[(Int32)pathLength];
			fixed (void* bytesPtr = &MemoryMarshal.GetReference(bytes))
			{
				RuntimeCallResult callResult =
					FrameworkResolver.GetHostPath(bytesPtr, ref pathLength, in Unsafe.NullRef<HostPathParameters>());
				FrameworkResolver.ThrowIfInvalidResult(callResult);
			}
			return Encoding.UTF8.GetString(bytes);
		}
		/// <summary>
		/// Retrieves the library path compatible with <paramref name="parameters"/> instance.
		/// </summary>
		/// <returns>A host library path.</returns>
		private static String GetLibraryPath(GetHostPathParameters parameters)
		{
			fixed (Byte* assemblyPathPtr =
				       &FrameworkResolver.GetRef(parameters.AssemblyPath, out Byte[]? assemblyPathArray))
			fixed (Byte* rootPathPtr = &FrameworkResolver.GetRef(parameters.RootPath, out Byte[]? rootPathArray))
			{
				try
				{
					HostPathParameters hostPathParameters = new(assemblyPathPtr, rootPathPtr);
					UIntPtr pathLength = FrameworkResolver.GetHostPathLength(in hostPathParameters);
					Span<Byte> bytes = stackalloc Byte[(Int32)pathLength];
					fixed (void* bytesPtr = &MemoryMarshal.GetReference(bytes))
					{
						RuntimeCallResult callResult =
							FrameworkResolver.GetHostPath(bytesPtr, ref pathLength, in hostPathParameters);
						FrameworkResolver.ThrowIfInvalidResult(callResult);
					}
					return Encoding.UTF8.GetString(bytes);
				}
				finally
				{
					FrameworkResolver.Clean([assemblyPathArray, rootPathArray,]);
				}
			}
		}
	}
}