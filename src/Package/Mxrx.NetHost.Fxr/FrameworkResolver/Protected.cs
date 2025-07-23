namespace Mxrx.NetHost;

public partial class FrameworkResolver
{
	/// <summary>
	/// Parameterless constructor.
	/// </summary>
	private protected FrameworkResolver() : this(IntPtr.Zero) { }

	/// <summary>
	/// Closes given host.
	/// </summary>
	/// <param name="hostContext">A <see cref="HostHandle"/> instance.</param>
	protected abstract void CloseHandle(HostContext hostContext);
	/// <summary>
	/// Configures the error writer pointer for <paramref name="hostContext"/>.
	/// </summary>
	/// <param name="hostContext">A <see cref="HostHandle"/> instance.</param>
	/// <param name="writeErrorPtr">Pointer to error writer function.</param>
	protected internal abstract void ConfigureErrorWriter(HostContext hostContext, IntPtr writeErrorPtr);
	/// <summary>
	/// Retrieves function pointer delegate for <paramref name="info"/>.
	/// </summary>
	/// <param name="hostContext">A <see cref="HostHandle"/> instance.</param>
	/// <param name="info">A <see cref="NetFunctionInfo"/> instance.</param>
	/// <returns>Pointer to delegate.</returns>
	protected internal abstract IntPtr GetFunctionPointer(HostContext hostContext, NetFunctionInfo info);
	/// <summary>
	/// Retrieves the function pointer of <paramref name="delegateType"/> for <paramref name="hostContext"/> instance.
	/// </summary>
	/// <param name="hostContext">A <see cref="HostContext"/> instance.</param>
	/// <param name="delegateType">A <see cref="RuntimeDelegateType"/> value.</param>
	/// <returns>
	/// Runtime function pointer of <paramref name="delegateType"/> for <paramref name="hostContext"/> instance.
	/// </returns>
	protected internal abstract IntPtr GetFunctionPointer(HostContext hostContext, RuntimeDelegateType delegateType);
	/// <summary>
	/// Loads assembly for <paramref name="parameters"/>
	/// </summary>
	/// <param name="hostContext">A <see cref="HostContext"/> instance.</param>
	/// <param name="parameters">A <see cref="LoadAssemblyParameters"/> instance.</param>
	protected internal abstract void LoadAssembly(HostContext hostContext, LoadAssemblyParameters parameters);
	/// <summary>
	/// Retrieves the <see cref="VolatileText"/> from <paramref name="propertyName"/>.
	/// </summary>
	/// <param name="hostContext">A <see cref="HostContext"/> instance.</param>
	/// <param name="propertyName">A <see cref="VolatileText"/> instance.</param>
	/// <returns>A <see cref="VolatileText"/> instance.</returns>
	protected internal abstract VolatileText GetProperty(HostContext hostContext, VolatileText propertyName);
	/// <summary>
	/// Sets <paramref name="propertyValue"/> as the value of the property named for <paramref name="propertyName"/>.
	/// </summary>
	/// <param name="hostContext">A <see cref="HostContext"/> instance.</param>
	/// <param name="propertyName">A <see cref="VolatileText"/> instance.</param>
	/// <param name="propertyValue">A <see cref="VolatileText"/> instance.</param>
	protected internal abstract void SetProperty(HostContext hostContext, VolatileText propertyName,
		VolatileText propertyValue);
	/// <summary>
	/// Runs application from <paramref name="hostContext"/> instance.
	/// </summary>
	/// <param name="hostContext">A <see cref="HostHandle"/> instance.</param>
	protected abstract Int32 RunAsApplication(HostContext hostContext);
	/// <inheritdoc cref="IDisposable.Dispose()"/>
	protected abstract void Dispose(Boolean disposing);
}