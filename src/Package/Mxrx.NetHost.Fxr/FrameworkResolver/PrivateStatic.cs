namespace Mxrx.NetHost;

public abstract partial class FrameworkResolver
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
	/// <returns>A <see cref="FrameworkResolver"/> instance for <paramref name="libHandle"/>.</returns>
	private static FrameworkResolver CreateResolver(IntPtr libHandle)
	{
		FrameworkResolver.loadedResolver = OperatingSystem.IsWindows() ? new Windows(libHandle) : new Unix(libHandle);
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
	/// Cleans up the initialization memory.
	/// </summary>
	/// <typeparam name="TChar">Text char type</typeparam>
	/// <param name="arrays">Arrays.</param>
	/// <param name="handles">Arguments handles.</param>
	private static void Clean<TChar>(ReadOnlySpan<TChar[]?> arrays, Span<ArgHandle> handles = default)
		where TChar : unmanaged, IEquatable<TChar>
	{
		foreach (TChar[]? array in arrays)
		{
			if (array is not null)
				ArrayPool<TChar>.Shared.Return(array);
		}

		foreach (ArgHandle handle in handles)
		{
			if (!handle.Handle.IsAllocated) break;
			if (handle is { FromArrayPool: true, Handle.Target: TChar[] arr, })
				ArrayPool<TChar>.Shared.Return(arr);
			handle.Handle.Free();
		}
	}
}