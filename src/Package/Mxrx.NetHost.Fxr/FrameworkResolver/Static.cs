namespace Mxrx.NetHost;

public abstract partial class FrameworkResolver
{
	/// <summary>
	/// Loads a <see cref="FrameworkResolver"/> from a static linked library.
	/// </summary>
	/// <typeparam name="TLibrary">A <see cref="IFrameworkResolverLibrary"/> type.</typeparam>
	/// <returns>A <see cref="FrameworkResolver"/> instance.</returns>
	public static FrameworkResolver LoadResolver<TLibrary>() where TLibrary : IFrameworkResolverLibrary.IPInvoke
	{
		FrameworkResolver.ThrowIfNotNativeAot();
		FrameworkResolver.ThrowIfInitializedHost();
		FrameworkResolver.ThrowIfInitializedHost(typeof(TLibrary));
		FrameworkResolver.loadedResolver ??=
			new Impl<PInvokeFunctionSet>(PInvokeFunctionSet.GetFunctionSet<TLibrary>(), typeof(TLibrary));
		return FrameworkResolver.loadedResolver;
	}
	/// <summary>
	/// Loads a <see cref="FrameworkResolver"/> from the default native library.
	/// </summary>
	/// <param name="parameters">Resolver location parameter.</param>
	/// <returns>A <see cref="FrameworkResolver"/> instance.</returns>
	/// <remarks>When this method is used, the .NET Host must be statically linked.</remarks>
	public static FrameworkResolver LoadResolver(GetHostPathParameters parameters = default)
	{
		String libraryPath = parameters.IsEmpty ?
			FrameworkResolver.GetLibraryPath() :
			FrameworkResolver.GetLibraryPath(parameters);
		return FrameworkResolver.LoadResolver(libraryPath);
	}
	/// <summary>
	/// Loads a <see cref="FrameworkResolver"/> from a native library handle.
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
	/// Loads a <see cref="FrameworkResolver"/> from given native library path.
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
	/// Loads a <see cref="FrameworkResolver"/> from given native library path.
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
	/// Retrieves active <see cref="FrameworkResolver"/> instance or loads a new one static linked library.
	/// </summary>
	/// <typeparam name="TLibrary">A <see cref="IFrameworkResolverLibrary"/> type.</typeparam>
	/// <param name="loaded">Indicates if resulting framework is loaded from static linked library.</param>
	/// <returns>A <see cref="FrameworkResolver"/> instance.</returns>
	public static FrameworkResolver GetActiveOrLoad<TLibrary>(out Boolean loaded)
		where TLibrary : IFrameworkResolverLibrary.IPInvoke
	{
		if (FrameworkResolver.loadedResolver is not null && !FrameworkResolver.loadedResolver._isDisposed)
		{
			loaded = false;
			return FrameworkResolver.loadedResolver;
		}
		loaded = true;
		return FrameworkResolver.LoadResolver<TLibrary>();
	}
	/// <summary>
	/// Retrieves active <see cref="FrameworkResolver"/> instance or loads a new one from the default native library.
	/// </summary>
	/// <param name="loaded">Indicates if resulting framework is loaded from native library.</param>
	/// <returns>A <see cref="FrameworkResolver"/> instance.</returns>
	public static FrameworkResolver GetActiveOrLoad(out Boolean loaded)
	{
		if (FrameworkResolver.loadedResolver is not null && !FrameworkResolver.loadedResolver._isDisposed)
		{
			loaded = false;
			return FrameworkResolver.loadedResolver;
		}
		loaded = true;
		return FrameworkResolver.LoadResolver();
	}
	/// <summary>
	/// Retrieves active <see cref="FrameworkResolver"/> instance or loads a new one rom the default native library.
	/// </summary>
	/// <param name="parameters">Resolver location parameter.</param>
	/// <param name="loaded">Indicates if resulting framework is loaded from native library.</param>
	/// <returns>A <see cref="FrameworkResolver"/> instance.</returns>
	public static FrameworkResolver GetActiveOrLoad(GetHostPathParameters parameters, out Boolean loaded)
	{
		if (FrameworkResolver.loadedResolver is not null && !FrameworkResolver.loadedResolver._isDisposed)
		{
			loaded = false;
			return FrameworkResolver.loadedResolver;
		}
		loaded = true;
		return FrameworkResolver.LoadResolver(parameters);
	}
	/// <summary>
	/// Retrieves active <see cref="FrameworkResolver"/> instance or loads a new one from given native library handle.
	/// </summary>
	/// <param name="libraryHandle">Native library handle.</param>
	/// <param name="loaded">Indicates if resulting framework is loaded from native library.</param>
	/// <returns>A <see cref="FrameworkResolver"/> instance.</returns>
	public static FrameworkResolver GetActiveOrLoad(IntPtr libraryHandle, out Boolean loaded)
	{
		if (FrameworkResolver.loadedResolver is not null && !FrameworkResolver.loadedResolver._isDisposed)
		{
			loaded = false;
			return FrameworkResolver.loadedResolver;
		}
		loaded = true;
		return FrameworkResolver.LoadResolver(libraryHandle);
	}
	/// <summary>
	/// Retrieves active <see cref="FrameworkResolver"/> instance or loads a new one from given native library path.
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
	/// <summary>
	/// Retrieves active <see cref="FrameworkResolver"/> instance or loads a new one from given native library path.
	/// </summary>
	/// <param name="libraryPath">Path to native library.</param>
	/// <param name="searchPath">The search path.</param>
	/// <param name="loaded">Indicates if resulting framework is loaded from native library.</param>
	/// <returns>A <see cref="FrameworkResolver"/> instance.</returns>
	public static FrameworkResolver GetActiveOrLoad(String libraryPath, DllImportSearchPath searchPath,
		out Boolean loaded)
	{
		if (FrameworkResolver.loadedResolver is not null && !FrameworkResolver.loadedResolver._isDisposed)
		{
			loaded = false;
			return FrameworkResolver.loadedResolver;
		}
		loaded = true;
		return FrameworkResolver.LoadResolver(libraryPath, searchPath);
	}
}