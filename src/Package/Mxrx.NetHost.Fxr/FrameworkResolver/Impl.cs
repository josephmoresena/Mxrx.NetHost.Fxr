namespace Mxrx.NetHost;

public partial class FrameworkResolver
{
	/// <summary>
	/// P/Invoke generic <see cref="FrameworkResolver"/> class.
	/// </summary>
	/// <typeparam name="TFunctionSet">A <typeparamref name="TFunctionSet"/> type.</typeparam>
#if !PACKAGE
	[SuppressMessage(Constants.CSharpSquid, Constants.CheckIdS6640,
	                 Justification = Constants.SecureUnsafeCodeJustification)]
#endif
	private sealed unsafe class Impl<TFunctionSet> : FrameworkResolver where TFunctionSet : IFunctionSet
	{
		/// <summary>
		/// Library functions.
		/// </summary>
		private readonly TFunctionSet _func;

		/// <inheritdoc/>
		private protected override Object ResolverKey { get; }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="handle">Native library handle.</param>
		/// <param name="initializeFromHandle">Delegate for set initialization.</param>
		/// <param name="path">Native library path.</param>
		public Impl(IntPtr handle, InitializeFromHandle<TFunctionSet> initializeFromHandle,
			String? path = default) : base(handle)
		{
			this.ResolverKey = (Object?)path ?? handle;
			try
			{
				Int32 count = initializeFromHandle(handle, out this._func);
				if (count * IntPtr.Size == TFunctionSet.SizeOf) return;
				// This will never execute.
				throw new TypeInitializationException($"{typeof(TFunctionSet)}", default);
			}
			catch (Exception)
			{
				this.Dispose();
				throw;
			}
		}
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="functionSet">A <typeparamref name="TFunctionSet"/> instance.</param>
		/// <param name="libraryType">Managed P/Invoke type.</param>
		public Impl(TFunctionSet functionSet, Type libraryType)
		{
			this.ResolverKey = libraryType;
			this._func = functionSet;
			this._isDisposed = false;
		}

		/// <inheritdoc/>
#if !PACKAGE
		[SuppressMessage(Constants.CSharpSquid, Constants.CheckIdS3218)]
#endif
		public override ContextImpl Initialize(InitializationParameters parameters)
		{
			RuntimeCallResult callResult;
			HostHandle hostHandle;
			Boolean isCommandLine = true;
			if (parameters.InitializeCommand && parameters.IsEmpty)
			{
				callResult = this._func.Initialize(out hostHandle);
			}
			else
			{
				Span<ArgHandle> handles = stackalloc ArgHandle[FrameworkResolver.GetHandlesCount(parameters.Arguments)];
				fixed (NativeChar* hostPathPtr =
					       &TextHelper.Instance.GetRef(parameters.HostPath, out Array? hostPathArray))
				fixed (NativeChar* rootPathPtr =
					       &TextHelper.Instance.GetRef(parameters.RootPath, out Array? rootPathArray))
				fixed (NativeChar* configPathPtr =
					       &TextHelper.Instance.GetRef(parameters.ConfigPath, out Array? configPathArray))
				{
					try
					{
						InitParameters param = new()
						{
							HostPathPtr = new() { Pointer = hostPathPtr, },
							RootPathPtr = new() { Pointer = rootPathPtr, },
							Size = (UIntPtr)sizeof(InitParameters),
						};
						if (!parameters.InitializeCommand)
						{
							isCommandLine = false;
							callResult =
								this._func.Initialize(new() { Pointer = configPathPtr, }, in param, out hostHandle);
						}
						else
						{
							Span<NativeCharPointer> addresses =
								stackalloc NativeCharPointer[parameters.Arguments.Count];
							Int32 argCount = TextHelper.Instance.LoadArgsAddr(parameters.Arguments, addresses, handles);
							callResult = this._func.Initialize(argCount, addresses.GetUnsafeValPtr(),
							                                   in param, out hostHandle);
						}
					}
					finally
					{
						TextHelper.Instance.Clean([hostPathArray, rootPathArray, configPathArray,], handles);
					}
				}
			}

			FrameworkResolver.ThrowIfInvalidResult(callResult);
			return new(this, hostHandle, isCommandLine);
		}

		/// <inheritdoc/>
#if !PACKAGE
		[SuppressMessage(Constants.CSharpSquid, Constants.CheckIdS3218)]
#endif
		protected internal override IntPtr GetFunctionPointer(HostContext hostContext, RuntimeDelegateType delegateType)
		{
			this._clrInitialized = true;

			RuntimeCallResult value =
				this._func.GetFunctionPointer(hostContext.Handle, delegateType, out IntPtr funcPtr);
			FrameworkResolver.ThrowIfInvalidResult(value);
			return funcPtr;
		}
		/// <inheritdoc/>
#if !PACKAGE
		[SuppressMessage(Constants.CSharpSquid, Constants.CheckIdS3218)]
#endif
		protected internal override IntPtr GetFunctionPointer(HostContext hostContext, NetFunctionInfo info)
		{
			this._clrInitialized = true;

			IntPtr result;
			RuntimeCallResult value;
			fixed (NativeChar* assemblyPathPtr =
				       &TextHelper.Instance.GetRef(info.AssemblyPath, out Array? assemblyPathArray))
			fixed (NativeChar* typeNamePtr = &TextHelper.Instance.GetRef(info.TypeName, out Array? typeNameArray))
			fixed (NativeChar* methodNamePtr = &TextHelper.Instance.GetRef(info.MethodName, out Array? methodNameArray))
			fixed (NativeChar* delegateTypeNamePtr =
				       &TextHelper.Instance.GetRef(info.DelegateTypeName, out Array? delegateTypeNameArray))
			{
				try
				{
					NativeChar* delegateTypePtr = info.UseUnmanagedCallersOnly ?
						(NativeChar*)UIntPtr.MaxValue.ToPointer() :
						delegateTypeNamePtr;

					value = info.AssemblyPath.IsEmpty ?
						FrameworkResolver.GetFunctionPointer(hostContext.GetFunctionPointerPtr, typeNamePtr,
						                                     methodNamePtr, delegateTypePtr, out result) :
						FrameworkResolver.LoadAssemblyAndGetFunctionPointer(
							hostContext.LoadAssemblyAndGetFunctionPointerPtr, assemblyPathPtr, typeNamePtr,
							methodNamePtr, delegateTypePtr, out result);
				}
				finally
				{
					TextHelper.Instance.Clean([
						assemblyPathArray, typeNameArray, methodNameArray, delegateTypeNameArray,
					]);
				}
			}

			FrameworkResolver.ThrowIfInvalidResult(value);
			return result;
		}
		/// <inheritdoc/>
#if !PACKAGE
		[SuppressMessage(Constants.CSharpSquid, Constants.CheckIdS3218)]
#endif
		protected internal override void LoadAssembly(HostContext hostContext, LoadAssemblyParameters parameters)
		{
			RuntimeCallResult value = RuntimeCallResult.HostInvalidState;
			Array? assemblyPathArray = default;
			try
			{
				if (parameters.AssemblyPath.IsEmpty)
					value = FrameworkResolver.LoadAssemblyFromBytes(hostContext.LoadAssemblyFromBytesPtr, parameters);
				else
					fixed (NativeChar* assemblyPathPtr =
						       &TextHelper.Instance.GetRef(parameters.AssemblyPath, out assemblyPathArray))
						value = FrameworkResolver.LoadAssemblyFromPath(hostContext.LoadAssemblyPtr, assemblyPathPtr);
			}
			finally
			{
				FrameworkResolver.ThrowIfInvalidResult(value);
				TextHelper.Instance.Clean([assemblyPathArray,]);
			}
		}
		/// <inheritdoc/>
		protected internal override RuntimePropertyCollection GetRuntimeProperties(HostContext hostContext)
		{
			this._clrInitialized = true;

			RuntimeCallResult callResult = this._func.CountRuntimeProperties(hostContext.Handle, out UIntPtr count);

			if (callResult is not RuntimeCallResult.HostApiBufferTooSmall)
				FrameworkResolver.ThrowIfInvalidResult(callResult);
			else if (count == default)
				return default;

			PropertiesBuffer buffer = new(count);
			fixed (NativeCharPointer* keysPtr = buffer.Keys)
			fixed (NativeCharPointer* valuesPtr = buffer.Values)
				callResult = this._func.GetRuntimeProperties(hostContext.Handle, count, keysPtr, valuesPtr);

			FrameworkResolver.ThrowIfInvalidResult(callResult);
			return new(hostContext.TextInvalidator, buffer);
		}
		/// <inheritdoc/>
		protected internal override VolatileText GetProperty(HostContext hostContext, VolatileText propertyName)
		{
			this._clrInitialized = true;

			fixed (NativeChar* propertyNamePtr =
				       &TextHelper.Instance.GetRef(propertyName.Text, out Array? propertyNameArray))
			{
				try
				{
					RuntimeCallResult callResult = this._func.GetRuntimePropertyValue(
						hostContext.Handle, new() { Pointer = propertyNamePtr, }, out NativeCharPointer value);
					FrameworkResolver.ThrowIfInvalidResult(callResult);

					VolatileText result = TextHelper.Instance.CreateLiteral(value);
					result.IsDisposed = hostContext.TextInvalidator;
					return result;
				}
				finally
				{
					TextHelper.Instance.Clean([propertyNameArray,]);
				}
			}
		}
		/// <inheritdoc/>
		protected internal override void SetProperty(HostContext hostContext, VolatileText propertyName,
			VolatileText propertyValue)
		{
			this._clrInitialized = true;

			fixed (NativeChar* propertyNamePtr =
				       &TextHelper.Instance.GetRef(propertyName.Text, out Array? propertyNameArray))
			fixed (NativeChar* propertyValuePtr =
				       &TextHelper.Instance.GetRef(propertyValue.Text, out Array? propertyValueArray))
			{
				try
				{
					RuntimeCallResult callResult = this._func.SetRuntimePropertyValue(
						hostContext.Handle, new() { Pointer = propertyNamePtr, },
						new() { Pointer = propertyValuePtr, });
					FrameworkResolver.ThrowIfInvalidResult(callResult);
				}
				finally
				{
					TextHelper.Instance.Clean([propertyNameArray, propertyValueArray,]);
				}
			}
		}
		/// <inheritdoc/>
		protected internal override void SetErrorWriter(HostContext hostContext, IntPtr writeErrorPtr)
			=> this._func.SetError(hostContext.Handle, (void*)writeErrorPtr);

		/// <inheritdoc/>
		protected override void Dispose(Boolean disposing)
		{
			if (disposing)
				NativeLibrary.Free(this._handle);
		}
		/// <inheritdoc/>
		protected override Int32 RunAsApplication(HostContext hostContext)
		{
			this._clrInitialized = true;
			return this._func.RunApp(hostContext.Handle);
		}
		/// <inheritdoc/>
		protected override void CloseHandle(HostContext hostContext) => this._func.CloseContext(hostContext.Handle);
	}
}