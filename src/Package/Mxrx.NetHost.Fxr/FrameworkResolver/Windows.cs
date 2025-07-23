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
	private sealed class Windows(IntPtr handle) : Generic<WindowsFunctions>(handle)
	{
		/// <inheritdoc/>
		public override unsafe ContextImpl Initialize(InitializationParameters parameters)
		{
			if (parameters.InitializeCommand && parameters.IsEmpty)
				base.Initialize(parameters);

			Span<ArgHandle> handles = stackalloc ArgHandle[FrameworkResolver.GetHandlesCount(parameters.Arguments)];
			fixed (Char* hostPathPtr = &Windows.GetRef(parameters.HostPath, out Char[]? hostPathArray))
			fixed (Char* rootPathPtr = &Windows.GetRef(parameters.RootPath, out Char[]? rootPathArray))
			fixed (Char* configPathPtr = &Windows.GetRef(parameters.ConfigPath, out Char[]? configPathArray))
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
						Int32 argCount = Windows.LoadArgsAddr(parameters.Arguments, addresses, handles);
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
			fixed (Char* assemblyPathPtr = &Windows.GetRef(info.AssemblyPath, out Char[]? assemblyPathArray))
			fixed (Char* typeNamePtr = &Windows.GetRef(info.TypeName, out Char[]? typeNameArray))
			fixed (Char* methodNamePtr = &Windows.GetRef(info.MethodName, out Char[]? methodNameArray))
			fixed (Char* delegateTypeNamePtr =
				       &Windows.GetRef(info.DelegateTypeName, out Char[]? delegateTypeNameArray))
			{
				try
				{
					Char* delegateTypePtr = info.UseUnmanagedCallersOnly ?
						(Char*)UIntPtr.MaxValue.ToPointer() :
						delegateTypeNamePtr;

					value = info.AssemblyPath.IsEmpty ?
						Windows.GetFunctionPointer(hostContext.GetFunctionPointerPtr.ToPointer(), typeNamePtr,
						                           methodNamePtr, delegateTypePtr, out result) :
						Windows.LoadAssemblyAnGetFunctionPointer(
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
			fixed (Char* assemblyPathPtr = &Windows.GetRef(parameters.AssemblyPath, out Char[]? assemblyPathArray))
			{
				try
				{
					delegate* <Char*, void*, void*, RuntimeCallResult> loadAssembly =
						(delegate* <Char*, void*, void*, RuntimeCallResult>)hostContext.LoadAssemblyPtr;

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
			fixed (Char* propertyNamePtr = &Windows.GetRef(propertyName.Text, out Char[]? propertyNameArray))
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
		protected internal override unsafe void SetProperty(HostContext hostContext, VolatileText propertyName,
			VolatileText propertyValue)
		{
			fixed (Char* propertyNamePtr = &Windows.GetRef(propertyName.Text, out Char[]? propertyNameArray))
			fixed (Char* propertyValuePtr = &Windows.GetRef(propertyValue.Text, out Char[]? propertyValueArray))
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
		private static unsafe RuntimeCallResult LoadAssemblyAnGetFunctionPointer(void* funcPtr, Char* assemblyPathPtr,
			Char* typeNamePtr, Char* methodNamePtr, Char* delegateTypePtr, out IntPtr resultPtr)
		{
			delegate* <Char*, Char*, Char*, Char*, void*, out IntPtr, RuntimeCallResult> loadAndGetFunctionPtr =
				(delegate* <Char*, Char*, Char*, Char*, void*, out IntPtr, RuntimeCallResult>)funcPtr;

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
		private static unsafe RuntimeCallResult GetFunctionPointer(void* funcPtr, Char* typeNamePtr,
			Char* methodNamePtr, Char* delegateTypePtr, out IntPtr resultPtr)
		{
			delegate*<Char*, Char*, Char*, void*, void*, out IntPtr, RuntimeCallResult> getFunctionPointerPtr =
				(delegate* <Char*, Char*, Char*, void*, void*, out IntPtr, RuntimeCallResult>)funcPtr;

			return getFunctionPointerPtr(typeNamePtr, methodNamePtr, delegateTypePtr, default, default, out resultPtr);
		}
		/// <summary>
		/// Retrieves a managed <see cref="Char"/> reference from <paramref name="text"/>.
		/// </summary>
		/// <param name="text">A <see cref="TextParameter"/> instance.</param>
		/// <param name="chars">Output. Created <see cref="Char"/> array.</param>
		/// <returns>A managed <see cref="Char"/> reference from <paramref name="text"/>.</returns>
#if !PACKAGE
		[SuppressMessage(Constants.CSharpSquid, Constants.CheckIdS3358,
		                 Justification = Constants.OptimizedJustification)]
#endif
		private static ref readonly Char GetRef(TextParameter text, out Char[]? chars)
		{
			chars = null;
			if (text.IsEmpty)
				return ref Unsafe.NullRef<Char>();
			if (!text.IsUtf8 && text.NullTerminated)
				return ref MemoryMarshal.GetReference(text.Value.AsValues<Byte, Char>());

			Int32 utf16Length = !text.IsUtf8 ?
				text.Value.Length / sizeof(Char) + 1 :
				Encoding.UTF8.GetCharCount(text.Value) + (!text.NullTerminated ? 1 : 0);
			chars = ArrayPool<Char>.Shared.Rent(utf16Length);
			Span<Char> charSpan = chars.AsSpan()[..utf16Length];

			if (!text.IsUtf8)
				text.Value.CopyTo(charSpan.AsBytes());
			else
				Encoding.UTF8.GetChars(text.Value, charSpan);
			charSpan[^1] = '\0';

			return ref MemoryMarshal.GetReference(charSpan);
		}
		/// <summary>
		/// Loads addresses to arguments values from <paramref name="args"/> at <paramref name="addr"/>
		/// </summary>
		/// <param name="args">A <see cref="ArgumentsParameter"/> instance.</param>
		/// <param name="addr">Destination addresses span.</param>
		/// <param name="handles">Destination handles span.</param>
		private static Int32 LoadArgsAddr(ArgumentsParameter args, Span<ReadOnlyValPtr<Char>> addr,
			Span<ArgHandle> handles)
		{
			if (args.IsEmpty) return 0;

			if (args.Sequence is null) return Windows.LoadFromSpan(args.Values, addr, handles);

			Windows.LoadFromSequence(args.Sequence, addr, handles);
			return args.Sequence.NonEmptyCount;
		}
		/// <summary>
		/// Loads addresses to arguments values from <paramref name="args"/> at <paramref name="addr"/>
		/// </summary>
		/// <param name="args">A <see cref="CStringSequence"/> instance.</param>
		/// <param name="addr">Destination addresses span.</param>
		/// <param name="handles">Destination handles span.</param>
		private static void LoadFromSequence(CStringSequence args, Span<ReadOnlyValPtr<Char>> addr,
			Span<ArgHandle> handles)
		{
			ReadOnlySpan<Byte> utfChars = args.ToString().AsSpan().AsBytes();
			Int32 charsCount = Encoding.UTF8.GetCharCount(utfChars);
			Span<Int32> offsets = stackalloc Int32[args.NonEmptyCount];
			Char[] chars = ArrayPool<Char>.Shared.Rent(charsCount);

			Encoding.UTF8.GetChars(utfChars, chars);
			chars[charsCount] = '\0';
			handles[0] = GCHandle.Alloc(chars, GCHandleType.Pinned);

			addr[0] = chars.AsSpan().GetUnsafeValPtr();
			for (Int32 i = 1; i < offsets.Length; i++)
			{
				Int32 offset = Encoding.UTF8.GetCharCount(utfChars[offsets[i - 1]..offsets[i]]);
				addr[i] = addr[i - 1] + offset;
			}
		}
		/// <summary>
		/// Loads addresses to arguments values from <paramref name="args"/> at <paramref name="addr"/>
		/// </summary>
		/// <param name="args">A <see cref="CStringSequence"/> instance.</param>
		/// <param name="addr">Destination addresses span.</param>
		/// <param name="handles">Destination handles span.</param>
		private static Int32 LoadFromSpan(ReadOnlySpan<Object?> args, Span<ReadOnlyValPtr<Char>> addr,
			Span<ArgHandle> handles)
		{
			Int32 ixHandle = 0;
			Int32 ixAddress = 0;
			for (Int32 i = 0; i < args.Length; i++)
			{
				String? value = args[i] is CString { Length: > 0, } c ? $"{c}\0" : args[i] as String;
				if (String.IsNullOrWhiteSpace(value))
				{
					addr[ixAddress] = default;
					ixAddress++;
					continue;
				}

				if (value.AsSpan()[^1] != '\0')
					value = $"{value}\0";

				handles[ixHandle] = value;
				addr[ixAddress] = (ReadOnlyValPtr<Char>)handles[ixHandle].Handle.AddrOfPinnedObject();
				ixHandle++;
				ixAddress++;
			}
			return ixAddress;
		}
	}
}