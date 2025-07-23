namespace Mxrx.NetHost;

public abstract partial class FrameworkResolver
{
	/// <summary>
	/// Loads a <see cref="FrameworkResolver"/> from current native library.
	/// </summary>
	/// <param name="libraryPath">Path to native library.</param>
	/// <returns>A <see cref="FrameworkResolver"/> instance.</returns>
	public static FrameworkResolver LoadResolver(String libraryPath)
	{
		FrameworkResolver.ThrowIfNotNativeAot();
		FrameworkResolver.ThrowIfInitializedHost();
		IntPtr libHandle = NativeLibrary.Load(libraryPath);
		return FrameworkResolver.CreateResolver(libHandle);
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
		FrameworkResolver.ThrowIfInitializedHost();
		IntPtr libHandle = NativeLibrary.Load(libraryPath, Assembly.GetExecutingAssembly(), searchPath);
		return FrameworkResolver.CreateResolver(libHandle);
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