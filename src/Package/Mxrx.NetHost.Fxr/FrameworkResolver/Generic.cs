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
				FrameworkResolver.GetAddress(handle, functions, Constants.CloseHandle, 0);
				FrameworkResolver.GetAddress(handle, functions, Constants.GetDelegate, 1);
				FrameworkResolver.GetAddress(handle, functions, Constants.GetRuntimeProperties, 2);
				FrameworkResolver.GetAddress(handle, functions, Constants.GetRuntimePropertyValue, 3);
				FrameworkResolver.GetAddress(handle, functions, Constants.SetRuntimePropertyValue, 4);
				FrameworkResolver.GetAddress(handle, functions, Constants.InitializeForCommand, 5);
				FrameworkResolver.GetAddress(handle, functions, Constants.InitializeForConfig, 6);
				FrameworkResolver.GetAddress(handle, functions, Constants.Main, 7);
				FrameworkResolver.GetAddress(handle, functions, Constants.MainStartupInfo, 8);
				FrameworkResolver.GetAddress(handle, functions, Constants.RunApp, 9);
				FrameworkResolver.GetAddress(handle, functions, Constants.SetErrorWriter, 10);
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
			delegate* <Byte*, UIntPtr, Byte*, UIntPtr, void*, void*, RuntimeCallResult> loadAssemblyBytes =
				(delegate* <Byte*, UIntPtr, Byte*, UIntPtr, void*, void*, RuntimeCallResult>)hostContext
					.LoadAssemblyFromBytesPtr.ToPointer();
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