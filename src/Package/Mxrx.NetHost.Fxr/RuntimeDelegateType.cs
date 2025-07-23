namespace Mxrx.NetHost;

/// <summary>
/// Type of CLR delegate.
/// </summary>
public enum RuntimeDelegateType
{
	/// <summary>
	/// Creates and default-initializes a single object of the class associated with a specified CLSID.
	/// </summary>
	/// <remarks>
	///     <c>hdt_com_activation</c>
	/// </remarks>
	ComActivation,
	/// <summary>
	/// IJW entry-point .
	/// </summary>
	/// <remarks>
	///     <c>hdt_load_in_memory_assembly</c>
	/// </remarks>
	LoadInMemoryAssembly,
	/// <summary>
	/// WinRT activation entry-point. This only works with .Net Core.
	/// </summary>
	WinRtActivation,
	/// <summary>
	/// Instructs an in-process server to create its registry entries for all classes supported in this server module.
	/// </summary>
	/// <remarks>
	///     <c>hdt_com_register</c>
	/// </remarks>
	ComRegister,
	/// <summary>
	/// Instructs an in-process server to remove only those entries created through DllRegisterServer.
	/// </summary>
	/// <remarks>
	///     <c>hdt_com_unregister</c>
	/// </remarks>
	ComUnregister,
	/// <summary>
	/// Loads an assembly (with dependencies) and returns function pointer for a specified static method.
	/// </summary>
	/// <remarks>
	///     <c>hdt_load_assembly_and_get_function_pointer</c>
	/// </remarks>
	LoadAssemblyAndGetFunction,
	/// <summary>
	/// Finds a managed method and returns a function pointer to it. This only works with .NET 5+.
	/// </summary>
	/// <remarks>
	///     <c>hdt_get_function_pointer</c>
	/// </remarks>
	GetFunction,
	/// <summary>
	/// Loads an assembly by its path. This only works with .NET 8+.
	/// </summary>
	/// <remarks>
	///     <c>hdt_load_assembly</c>
	/// </remarks>
	LoadAssembly,
	/// <summary>
	/// Loads an assembly from a byte array. This only works with .NET 8+.
	/// </summary>
	/// <remarks>
	///     <c>hdt_load_assembly_bytes</c>
	/// </remarks>
	LoadAssemblyBytes,
}