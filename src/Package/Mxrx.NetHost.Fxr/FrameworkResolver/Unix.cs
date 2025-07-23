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
	private sealed class Unix(IntPtr handle) : Generic<UnixFunctions>(handle)
	{
		/// <inheritdoc/>
		public override unsafe ContextImpl Initialize(InitializationParameters parameters)
		{
			if (parameters.InitializeCommand && parameters.IsEmpty)
				base.Initialize(parameters);

			Span<ArgHandle> handles = stackalloc ArgHandle[FrameworkResolver.GetHandlesCount(parameters.Arguments)];
			fixed (Byte* hostPathPtr = &Unix.GetRef(parameters.HostPath, out Byte[]? hostPathArray))
			fixed (Byte* rootPathPtr = &Unix.GetRef(parameters.RootPath, out Byte[]? rootPathArray))
			fixed (Byte* configPathPtr = &Unix.GetRef(parameters.ConfigPath, out Byte[]? configPathArray))
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
						Int32 argCount = Unix.LoadArgsAddr(parameters.Arguments, addresses, handles);
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
		protected internal override unsafe IntPtr GetFunctionPointer(HostContext hostContext, NetFunctionInfo info)
		{
			IntPtr result;
			RuntimeCallResult value;
			fixed (Byte* assemblyPathPtr = &Unix.GetRef(info.AssemblyPath, out Byte[]? assemblyPathArray))
			fixed (Byte* typeNamePtr = &Unix.GetRef(info.TypeName, out Byte[]? typeNameArray))
			fixed (Byte* methodNamePtr = &Unix.GetRef(info.MethodName, out Byte[]? methodNameArray))
			fixed (Byte* delegateTypeNamePtr = &Unix.GetRef(info.DelegateTypeName, out Byte[]? delegateTypeNameArray))
			{
				try
				{
					Byte* delegateTypePtr = info.UseUnmanagedCallersOnly ?
						(Byte*)UIntPtr.MaxValue.ToPointer() :
						delegateTypeNamePtr;

					value = info.AssemblyPath.IsEmpty ?
						Unix.GetFunctionPointer(hostContext.GetFunctionPointerPtr.ToPointer(), typeNamePtr,
						                        methodNamePtr, delegateTypePtr, out result) :
						Unix.LoadAssemblyAnGetFunctionPointer(
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
		protected internal override unsafe void LoadAssembly(HostContext hostContext, LoadAssemblyParameters parameters)
		{
			if (parameters.AssemblyPath.IsEmpty)
				base.LoadAssembly(hostContext, parameters);
			fixed (Byte* assemblyPathPtr = &Unix.GetRef(parameters.AssemblyPath, out Byte[]? assemblyPathArray))
			{
				try
				{
					delegate* <Byte*, void*, void*, RuntimeCallResult> loadAssembly =
						(delegate* <Byte*, void*, void*, RuntimeCallResult>)hostContext.LoadAssemblyPtr;

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
		protected internal override unsafe VolatileText GetProperty(HostContext hostContext, VolatileText propertyName)
		{
			fixed (Byte* propertyNamePtr = &Unix.GetRef(propertyName.Text, out Byte[]? propertyNameArray))
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
		protected internal override unsafe void SetProperty(HostContext hostContext, VolatileText propertyName,
			VolatileText propertyValue)
		{
			fixed (Byte* propertyNamePtr = &Unix.GetRef(propertyName.Text, out Byte[]? propertyNameArray))
			fixed (Byte* propertyValuePtr = &Unix.GetRef(propertyValue.Text, out Byte[]? propertyValueArray))
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
		/// Invokes load_assembly_and_get_function_pointer_fn.
		/// </summary>
		/// <param name="funcPtr">A pointer to get_function_pointer_fn.</param>
		/// <param name="assemblyPathPtr">Assembly path pointer.</param>
		/// <param name="typeNamePtr">Type name pointer.</param>
		/// <param name="methodNamePtr">Method name pointer.</param>
		/// <param name="delegateTypePtr">Delegate type name pointer.</param>
		/// <param name="resultPtr">Output. Resulting function pointer.</param>
		/// <returns>A <see cref="RuntimeCallResult"/> value.</returns>
		private static unsafe RuntimeCallResult LoadAssemblyAnGetFunctionPointer(void* funcPtr, Byte* assemblyPathPtr,
			Byte* typeNamePtr, Byte* methodNamePtr, Byte* delegateTypePtr, out IntPtr resultPtr)
		{
			delegate* <Byte*, Byte*, Byte*, Byte*, void*, out IntPtr, RuntimeCallResult> loadAndGetFunctionPtr =
				(delegate* <Byte*, Byte*, Byte*, Byte*, void*, out IntPtr, RuntimeCallResult>)funcPtr;

			return loadAndGetFunctionPtr(assemblyPathPtr, typeNamePtr, methodNamePtr, delegateTypePtr, default,
			                             out resultPtr);
		}
		/// <summary>
		/// Invokes get_function_pointer_fn.
		/// </summary>
		/// <param name="funcPtr">A pointer to get_function_pointer_fn.</param>
		/// <param name="typeNamePtr">Type name pointer.</param>
		/// <param name="methodNamePtr">Method name pointer.</param>
		/// <param name="delegateTypePtr">Delegate type name pointer.</param>
		/// <param name="resultPtr">Output. Resulting function pointer.</param>
		/// <returns>A <see cref="RuntimeCallResult"/> value.</returns>
		private static unsafe RuntimeCallResult GetFunctionPointer(void* funcPtr, Byte* typeNamePtr,
			Byte* methodNamePtr, Byte* delegateTypePtr, out IntPtr resultPtr)
		{
			delegate*<Byte*, Byte*, Byte*, void*, void*, out IntPtr, RuntimeCallResult> getFunctionPointerPtr =
				(delegate* <Byte*, Byte*, Byte*, void*, void*, out IntPtr, RuntimeCallResult>)funcPtr;

			return getFunctionPointerPtr(typeNamePtr, methodNamePtr, delegateTypePtr, default, default, out resultPtr);
		}
		/// <summary>
		/// Retrieves a managed <see cref="Byte"/> reference from <paramref name="text"/>.
		/// </summary>
		/// <param name="text">A <see cref="TextParameter"/> instance.</param>
		/// <param name="chars">Output. Created <see cref="Char"/> array.</param>
		/// <returns>A managed <see cref="Byte"/> reference from <paramref name="text"/>.</returns>
		private static ref Byte GetRef(TextParameter text, out Byte[]? chars)
		{
			chars = null;
			if (text.IsEmpty)
				return ref Unsafe.NullRef<Byte>();
			switch (text.IsUtf8)
			{
				case true when text.NullTerminated:
					return ref MemoryMarshal.GetReference(text.Value);
				case true:
					chars = ArrayPool<Byte>.Shared.Rent(text.Value.Length + 1);
					text.Value.CopyTo(chars.AsSpan());
					chars[text.Value.Length] = (Byte)'\0';
					break;
				default:
				{
					ReadOnlySpan<Char> utf16Char = text.Value.AsValues<Byte, Char>();
					Int32 utf8Length = Encoding.UTF8.GetByteCount(utf16Char);
					Int32 extraLength = utf16Char[^1] != '\0' ? 1 : 0;
					chars = ArrayPool<Byte>.Shared.Rent(utf8Length + extraLength);
					Encoding.UTF8.GetBytes(utf16Char, chars);
					chars[utf8Length] = default;
					break;
				}
			}

			chars[^1] = (Byte)'\0';
			return ref MemoryMarshal.GetReference(chars.AsSpan());
		}
		/// <summary>
		/// Loads addresses to arguments values from <paramref name="args"/> at <paramref name="addr"/>
		/// </summary>
		/// <param name="args">A <see cref="ArgumentsParameter"/> instance.</param>
		/// <param name="addr">Destination addresses span.</param>
		/// <param name="handles">Destination handles span.</param>
		private static Int32 LoadArgsAddr(ArgumentsParameter args, Span<ReadOnlyValPtr<Byte>> addr,
			Span<ArgHandle> handles)
		{
			if (args.IsEmpty) return 0;
			if (args.Sequence is null)
				return Unix.LoadFromSpan(args.Values, addr, handles);

			Unix.LoadFromSequence(args.Sequence, addr, handles);
			return args.Sequence.NonEmptyCount;
		}
		/// <summary>
		/// Loads addresses to arguments values from <paramref name="args"/> at <paramref name="addr"/>
		/// </summary>
		/// <param name="args">A <see cref="CStringSequence"/> instance.</param>
		/// <param name="addr">Destination addresses span.</param>
		/// <param name="handles">Destination handles span.</param>
		private static void LoadFromSequence(CStringSequence args, Span<ReadOnlyValPtr<Byte>> addr,
			Span<ArgHandle> handles)
		{
			String buffer = args.ToString();
			ReadOnlyValPtr<Byte> ptr = buffer.AsSpan().AsBytes().GetUnsafeValPtr();
			Span<Int32> offsets = stackalloc Int32[args.NonEmptyCount];

			args.GetOffsets(offsets);
			handles[0] = buffer;

			for (Int32 i = 0; i < offsets.Length; i++)
				addr[i] = ptr + offsets[i];
		}
		/// <summary>
		/// Loads addresses to arguments values from <paramref name="args"/> at <paramref name="addr"/>
		/// </summary>
		/// <param name="args">A <see cref="CStringSequence"/> instance.</param>
		/// <param name="addr">Destination addresses span.</param>
		/// <param name="handles">Destination handles span.</param>
#if !PACKAGE
		[SuppressMessage(Constants.CSharpSquid, Constants.CheckIdS3776,
		                 Justification = Constants.OptimizedJustification)]
#endif
		private static Int32 LoadFromSpan(ReadOnlySpan<Object?> args, Span<ReadOnlyValPtr<Byte>> addr,
			Span<ArgHandle> handles)
		{
			Int32 ixHandle = 0;
			Int32 ixAddress = 0;
			for (Int32 i = 0; i < args.Length; i++)
			{
				Byte[]? arr;

				switch (args[i])
				{
					case null:
						continue;
					case CString c:
					{
						if (CString.IsNullOrEmpty(c)) continue;
						if (c.IsNullTerminated)
						{
							ReadOnlySpan<Byte> span = c.AsSpan();
							ArgHandle argH = c;
							if (argH.Handle.IsAllocated || !span.MayBeNonLiteral() || c.IsReference)
							{
								addr[ixAddress] = c.AsSpan().GetUnsafeValPtr();
								ixAddress++;
								if (argH.Handle.IsAllocated)
								{
									handles[ixHandle] = argH;
									ixHandle++;
								}
								continue;
							}
						}

						arr = ArrayPool<Byte>.Shared.Rent(c.Length + 1);
						c.AsSpan().CopyTo(arr);
						arr[c.Length] = (Byte)'\0';
						break;
					}
					default:
					{
						String? value = args[i]?.ToString();
						if (String.IsNullOrEmpty(value)) continue;

						Int32 utf8Length = Encoding.UTF8.GetByteCount(value);
						Int32 extraLength = value[^1] != '\0' ? 1 : 0;
						arr = ArrayPool<Byte>.Shared.Rent(value.Length + extraLength);
						Encoding.UTF8.GetBytes(value, arr);
						arr[utf8Length] = (Byte)'\0';
						break;
					}
				}

				handles[ixHandle] = GCHandle.Alloc(arr, GCHandleType.Pinned);
				addr[ixHandle] = (ReadOnlyValPtr<Byte>)handles[ixHandle].Handle.AddrOfPinnedObject();
				ixHandle++;
				ixAddress++;
			}
			return ixAddress;
		}
	}
}