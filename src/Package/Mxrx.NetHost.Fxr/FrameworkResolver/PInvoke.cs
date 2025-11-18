namespace Mxrx.NetHost;

public partial class FrameworkResolver
{
	/// <summary>
	/// P/Invoke generic <see cref="FrameworkResolver"/> class.
	/// </summary>
	/// <typeparam name="TLibrary">A <see cref="IFrameworkResolverLibrary"/> type.</typeparam>
#if !PACKAGE
	[SuppressMessage(Constants.CSharpSquid, Constants.CheckIdS6640,
	                 Justification = Constants.SecureUnsafeCodeJustification)]
#endif
	private abstract partial class PInvoke<TLibrary> : FrameworkResolver where TLibrary : IFrameworkResolverLibrary
	{
		/// <summary>
		/// Parameterless constructor.
		/// </summary>
		protected PInvoke() => this._isDisposed = false; // P/Invoke resolved is unloadable.

		/// <inheritdoc/>
		public override ContextImpl Initialize(InitializationParameters parameters)
		{
			RuntimeCallResult value = TLibrary.Initialize(0, default, default, out IntPtr handle);
			FrameworkResolver.ThrowIfInvalidResult(value);
			return new(this, handle, true);
		}

		/// <inheritdoc/>
		protected internal sealed override void SetErrorWriter(HostContext hostContext, IntPtr writeErrorPtr)
			=> TLibrary.SetError(hostContext.Handle, writeErrorPtr);
		/// <inheritdoc/>
		protected internal sealed override Int32 CountProperties(HostContext hostContext)
		{
			FrameworkResolver.ThrowIfInvalidResult(
				IFrameworkResolverLibrary.CountProperties<TLibrary>(hostContext.Handle, out UIntPtr count));
			return (Int32)count;
		}
		/// <inheritdoc/>
		protected sealed override void CloseHandle(HostContext hostContext)
			=> TLibrary.CloseContext(hostContext.Handle);
		/// <inheritdoc/>
		protected sealed override Int32 RunAsApplication(HostContext hostContext)
		{
			this._clrInitialized = true;
			return TLibrary.RunApp(hostContext.Handle);
		}
		/// <inheritdoc/>
		protected internal sealed override IntPtr GetFunctionPointer(HostContext hostContext,
			RuntimeDelegateType delegateType)
		{
			this._clrInitialized = true;
			RuntimeCallResult value = TLibrary.GetFunctionPointer(hostContext.Handle, delegateType, out IntPtr funcPtr);
			FrameworkResolver.ThrowIfInvalidResult(value);
			return funcPtr;
		}

		/// <summary>
		/// Creates a P/Invoke <see cref="FrameworkResolver"/> instance.
		/// </summary>
		/// <returns>A P/Invoke <see cref="FrameworkResolver"/> instance.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FrameworkResolver CreateResolver()
			=> SystemInfo.IsWindows ? new WindowsPInvoke() : new UnixPInvoke();
	}
}