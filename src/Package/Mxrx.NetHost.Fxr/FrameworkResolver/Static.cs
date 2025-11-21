namespace Mxrx.NetHost;

public abstract partial class FrameworkResolver
{
	/// <summary>
	/// Loads a <see cref="FrameworkResolver"/> from static linked library.
	/// </summary>
	/// <typeparam name="TLibrary">A <see cref="IFrameworkResolverLib"/> type.</typeparam>
	/// <returns>A <see cref="FrameworkResolver"/> instance.</returns>
	public static FrameworkResolver LoadResolver<TLibrary>() where TLibrary : IFrameworkResolverLib.IPInvoke
	{
		FrameworkResolver.ThrowIfNotNativeAot();
		FrameworkResolver.ThrowIfInitializedHost();
		FrameworkResolver.ThrowIfInitializedHost(typeof(TLibrary));
		FrameworkResolver.loadedResolver ??=
			new Impl<PInvokeFunctionSet>(PInvokeFunctionSet.GetFunctionSet<TLibrary>(), typeof(TLibrary));
		return FrameworkResolver.loadedResolver;
	}
	/// <summary>
	/// Loads a <see cref="FrameworkResolver"/> from static linked library.
	/// </summary>
	/// <returns>A <see cref="FrameworkResolver"/> instance.</returns>
	/// <remarks>When this method is used, the .NET Host must be statically linked.</remarks>
#if !PACKAGE
	[ExcludeFromCodeCoverage]
	[SuppressMessage(Constants.CSharpSquid, Constants.CheckIdS3881, Justification = Constants.OptimizedJustification)]
#endif
	public static FrameworkResolver LoadResolver(GetHostPathParameters parameters = default)
	{
		String libraryPath = parameters.IsEmpty ?
			FrameworkResolver.GetLibraryPath() :
			FrameworkResolver.GetLibraryPath(parameters);
		return FrameworkResolver.LoadResolver(libraryPath);
	}
	/// <summary>
	/// Loads a <see cref="FrameworkResolver"/> from current native library.
	/// </summary>
	/// <param name="libraryHandle">Native library handle.</param>
	/// <returns>A <see cref="FrameworkResolver"/> instance.</returns>
	public static FrameworkResolver LoadResolver(IntPtr libraryHandle)
	{
		FrameworkResolver.ThrowIfNotNativeAot();
		FrameworkResolver.ThrowIfInitializedHost(libraryHandle);
		return FrameworkResolver.CreateResolver(libraryHandle);
	}
	/// <summary>
	/// Loads a <see cref="FrameworkResolver"/> from current native library.
	/// </summary>
	/// <param name="libraryPath">Path to native library.</param>
	/// <returns>A <see cref="FrameworkResolver"/> instance.</returns>
	public static FrameworkResolver LoadResolver(String libraryPath)
	{
		FrameworkResolver.ThrowIfNotNativeAot();
		FrameworkResolver.ThrowIfInitializedHost(libraryPath);
		IntPtr libHandle = NativeLibrary.Load(libraryPath);
		return FrameworkResolver.CreateResolver(libHandle, libraryPath);
	}
	/// <summary>
	/// Loads a <see cref="FrameworkResolver"/> from current native library.
	/// </summary>
	/// <param name="libraryPath">Path to native library.</param>
	/// <param name="searchPath">The search path.</param>
	/// <returns>A <see cref="FrameworkResolver"/> instance.</returns>
	public static FrameworkResolver LoadResolver(String libraryPath, DllImportSearchPath searchPath)
	{
		FrameworkResolver.ThrowIfNotNativeAot();
		FrameworkResolver.ThrowIfInitializedHost(libraryPath);
		IntPtr libHandle = NativeLibrary.Load(libraryPath, Assembly.GetExecutingAssembly(), searchPath);
		return FrameworkResolver.CreateResolver(libHandle, libraryPath);
	}
	/// <summary>
	/// Retrieves active <see cref="FrameworkResolver"/> instance or loads a new one from native library.
	/// </summary>
	/// <param name="libraryPath">Path to native library.</param>
	/// <param name="loaded">Indicates if resulting framework is loaded from native library.</param>
	/// <returns>A <see cref="FrameworkResolver"/> instance.</returns>
	public static FrameworkResolver GetActiveOrLoad(String libraryPath, out Boolean loaded)
	{
		if (FrameworkResolver.loadedResolver is not null && !FrameworkResolver.loadedResolver._isDisposed)
		{
			loaded = false;
			return FrameworkResolver.loadedResolver;
		}
		loaded = true;
		return FrameworkResolver.LoadResolver(libraryPath);
	}
}