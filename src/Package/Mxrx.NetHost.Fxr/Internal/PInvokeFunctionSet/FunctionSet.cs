namespace Mxrx.NetHost.Internal;

internal abstract unsafe partial class PInvokeFunctionSet
{
	/// <summary>
	/// Function set.
	/// </summary>
#if !PACKAGE
	[SuppressMessage(Constants.CSharpSquid, Constants.CheckIdS6640,
	                 Justification = Constants.SecureUnsafeCodeJustification)]
#endif
	public sealed class FunctionSet<TLibrary> : PInvokeFunctionSet where TLibrary : IFrameworkResolverLibrary.IPInvoke
	{
		/// <summary>
		/// Singleton instance.
		/// </summary>
		public static readonly FunctionSet<TLibrary> Instance = new();

		/// <summary>
		/// Private constructor.
		/// </summary>
		private FunctionSet() { }

		/// <inheritdoc/>
		private protected override RuntimeCallResult CloseContextImpl(HostHandle handle)
			=> TLibrary.CloseContext(handle);
		/// <inheritdoc/>
		private protected override RuntimeCallResult GetFunctionPointerImpl(HostHandle handle,
			RuntimeDelegateType delegateType, out IntPtr funcPtr)
			=> TLibrary.GetFunctionPointer(handle, delegateType, out funcPtr);
		/// <inheritdoc/>
		private protected override RuntimeCallResult GetRuntimePropertiesImpl(HostHandle handle, ref UIntPtr propCount,
			out ReadOnlyValPtr<NativeCharPointer> propKeysPtr, out ReadOnlyValPtr<NativeCharPointer> propValuesPtr)
		{
			Unsafe.SkipInit(out propKeysPtr);
			Unsafe.SkipInit(out propValuesPtr);
			return TLibrary.GetRuntimeProperties(handle, ref propCount, ref propKeysPtr, ref propValuesPtr);
		}
		/// <inheritdoc/>
		private protected override RuntimeCallResult CountRuntimePropertiesImpl(HostHandle handle,
			out UIntPtr propCount)
		{
			Unsafe.SkipInit(out propCount);
			ref ReadOnlyValPtr<NativeCharPointer> nullRef = ref Unsafe.NullRef<ReadOnlyValPtr<NativeCharPointer>>();
			return TLibrary.GetRuntimeProperties(handle, ref propCount, ref nullRef, ref nullRef);
		}
		/// <inheritdoc/>
		private protected override RuntimeCallResult GetRuntimePropertyValueImpl(HostHandle handle,
			NativeCharPointer keyPtr, out NativeCharPointer valuePtr)
			=> TLibrary.GetRuntimePropertyValue(handle, keyPtr, out valuePtr);
		/// <inheritdoc/>
		private protected override RuntimeCallResult SetRuntimePropertyValueImpl(HostHandle handle,
			NativeCharPointer keyPtr, NativeCharPointer valuePtr)
			=> TLibrary.SetRuntimePropertyValue(handle, keyPtr, valuePtr);
		/// <inheritdoc/>
		private protected override RuntimeCallResult InitializeImpl(Int32 argsCount, NativeCharPointer* argsPtr,
			in InitParameters initParams, out HostHandle handle)
			=> TLibrary.InitializeForCommandLine(argsCount, argsPtr, in initParams, out handle);
		/// <inheritdoc/>
		private protected override RuntimeCallResult InitializeImpl(NativeCharPointer configPathPtr,
			in InitParameters initParams, out HostHandle handle)
			=> TLibrary.InitializeForConfigFile(configPathPtr, in initParams, out handle);
		/// <inheritdoc/>
		private protected override Int32 RunAppImpl(HostHandle handle) => TLibrary.RunApp(handle);
		/// <inheritdoc/>
		private protected override void* SetErrorImpl(HostHandle handle, void* writeErrPtr)
			=> (void*)TLibrary.SetError(handle, (IntPtr)writeErrPtr);
	}
}