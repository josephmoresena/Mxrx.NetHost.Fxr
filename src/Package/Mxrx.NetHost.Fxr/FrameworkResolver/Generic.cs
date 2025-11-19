namespace Mxrx.NetHost;

public partial class FrameworkResolver
{
	/// <summary>
	/// Generic <see cref="FrameworkResolver"/> class.
	/// </summary>
	/// <typeparam name="TFunction">A <see cref="IResolverFunctions"/> type.</typeparam>
#if !PACKAGE
	[SuppressMessage(Constants.CSharpSquid, Constants.CheckIdS6640,
	                 Justification = Constants.SecureUnsafeCodeJustification)]
#endif
	private abstract class Generic<TFunction> : FrameworkResolver where TFunction : unmanaged, IResolverFunctions
	{
		/// <summary>
		/// Library functions.
		/// </summary>
		protected TFunction Functions;

		/// <inheritdoc/>
		protected Generic(IntPtr handle) : base(handle)
		{
			Span<IntPtr> functions = MemoryMarshal.CreateSpan(ref this.Functions, 1).AsValues<TFunction, IntPtr>();
			try
			{
				Int32 index = 0;
				FrameworkResolver.GetAddress(handle, functions, IFrameworkResolverLibrary.CloseHandleSymbol, index++);
				FrameworkResolver.GetAddress(handle, functions, IFrameworkResolverLibrary.GetDelegateSymbol, index++);
#if !DISABLE_RUNTIME_PROPERTIES
				FrameworkResolver.GetAddress(handle, functions, IFrameworkResolverLibrary.GetRuntimePropertiesSymbol,
				                             index++);
#endif
				FrameworkResolver.GetAddress(handle, functions, IFrameworkResolverLibrary.GetRuntimePropertyValueSymbol,
				                             index++);
				FrameworkResolver.GetAddress(handle, functions, IFrameworkResolverLibrary.SetRuntimePropertyValueSymbol,
				                             index++);
				FrameworkResolver.GetAddress(handle, functions, IFrameworkResolverLibrary.InitializeForCommandSymbol,
				                             index++);
				FrameworkResolver.GetAddress(handle, functions, IFrameworkResolverLibrary.InitializeForConfigSymbol,
				                             index++);
#if !DISABLE_MAIN_CALLS
				FrameworkResolver.GetAddress(handle, functions, IFrameworkResolverLibrary.MainSymbol, index++);
				FrameworkResolver.GetAddress(handle, functions, IFrameworkResolverLibrary.MainStartupInfoSymbol, index++);
#endif
				FrameworkResolver.GetAddress(handle, functions, IFrameworkResolverLibrary.RunAppSymbol, index++);
				FrameworkResolver.GetAddress(handle, functions, IFrameworkResolverLibrary.SetErrorWriterSymbol,
				                             index++);
				if (index * IntPtr.Size == TFunction.SizeOf) return;
				// This will never execute.
				throw new TypeInitializationException($"{typeof(TFunction)}", default);
			}
			catch (Exception)
			{
				this.Dispose();
				throw;
			}
		}

		/// <inheritdoc/>
		public override ContextImpl Initialize(InitializationParameters parameters)
		{
			RuntimeCallResult value = this.Functions.Initialize(out HostHandle handle);
			FrameworkResolver.ThrowIfInvalidResult(value);
			return new(this, handle, true);
		}

		/// <inheritdoc/>
		protected sealed override void Dispose(Boolean disposing)
		{
			if (disposing)
				NativeLibrary.Free(this._handle);
		}

		/// <inheritdoc/>
		protected internal sealed override void SetErrorWriter(HostContext hostContext, IntPtr writeErrorPtr)
			=> this.Functions.SetError(hostContext.Handle, writeErrorPtr);
		/// <inheritdoc/>
		protected sealed override void CloseHandle(HostContext hostContext)
			=> this.Functions.CloseContext(hostContext.Handle);
		/// <inheritdoc/>
		protected sealed override Int32 RunAsApplication(HostContext hostContext)
		{
			this._clrInitialized = true;
			return this.Functions.RunApp(hostContext.Handle);
		}
		/// <inheritdoc/>
#if !PACKAGE
		[SuppressMessage(Constants.CSharpSquid, Constants.CheckIdS3218)]
#endif
		protected internal sealed override unsafe IntPtr GetFunctionPointer(HostContext hostContext,
			RuntimeDelegateType delegateType)
		{
			this._clrInitialized = true;
			RuntimeCallResult value =
				this.Functions.GetFunctionPointer(hostContext.Handle, delegateType, out void* funcPtr);
			FrameworkResolver.ThrowIfInvalidResult(value);
			return new(funcPtr);
		}
		/// <inheritdoc/>
		protected internal override unsafe void LoadAssembly(HostContext hostContext, LoadAssemblyParameters parameters)
		{
			delegate* unmanaged[Stdcall]<Byte*, UIntPtr, Byte*, UIntPtr, void*, void*, RuntimeCallResult>
				loadAssemblyBytes =
					(delegate* unmanaged[Stdcall]<Byte*, UIntPtr, Byte*, UIntPtr, void*, void*, RuntimeCallResult>)
					hostContext.LoadAssemblyFromBytesPtr.ToPointer();
			this._clrInitialized = true;

			fixed (Byte* assemblyPtr = &MemoryMarshal.GetReference(parameters.AssemblyBytes))
			fixed (Byte* symbolPtr = &MemoryMarshal.GetReference(parameters.SymbolsBytes))
			{
				RuntimeCallResult value = loadAssemblyBytes(assemblyPtr, (UIntPtr)parameters.AssemblyBytes.Length,
				                                            !parameters.SymbolsBytes.IsEmpty ? symbolPtr : default,
				                                            (UIntPtr)parameters.SymbolsBytes.Length, default, default);
				FrameworkResolver.ThrowIfInvalidResult(value);
			}
		}
	}
}