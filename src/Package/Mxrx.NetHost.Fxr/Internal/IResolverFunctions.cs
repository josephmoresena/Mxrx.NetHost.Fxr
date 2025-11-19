namespace Mxrx.NetHost.Internal;

/// <summary>
/// <c>Hostfxr</c> library exported interface.
/// </summary>
#if !PACKAGE
[SuppressMessage(Constants.CSharpSquid, Constants.CheckIdS6640,
                 Justification = Constants.SecureUnsafeCodeJustification)]
#endif
internal unsafe interface IResolverFunctions
{
	/// <summary>
	/// Closes host context.
	/// </summary>
	/// <param name="handle">Host context handle.</param>
	/// <returns>Operation result.</returns>
	RuntimeCallResult CloseContext(HostHandle handle);
	/// <summary>
	/// Initializes a new context handle.
	/// </summary>
	/// <param name="handle">Output. Host context handle.</param>
	/// <returns>Operation result.</returns>
	RuntimeCallResult Initialize(out HostHandle handle);
	/// <summary>
	/// Set error ptr.
	/// </summary>
	/// <param name="handle">Host context handle.</param>
	/// <param name="writeErrPtr">Write error delegate pointer.</param>
	void SetError(HostHandle handle, IntPtr writeErrPtr);
	/// <summary>
	/// Runs application from current context.
	/// </summary>
	/// <param name="handle">Host context handle.</param>
	/// <returns>Exit code.</returns>
	Int32 RunApp(HostHandle handle);
	/// <summary>
	/// Retrieves function pointer.
	/// </summary>
	/// <param name="handle">Host context handle.</param>
	/// <param name="delegateType">Function type.</param>
	/// <param name="funcPtr">Output. Function pointer.</param>
	/// <returns>Operation result.</returns>
	RuntimeCallResult GetFunctionPointer(HostHandle handle, RuntimeDelegateType delegateType, out void* funcPtr);
	/// <summary>
	/// Type of current char.
	/// </summary>
#pragma warning disable CA2252
	static abstract Int32 SizeOf { get; }
	static abstract Type CharType { get; }
#pragma warning restore CA2252
}