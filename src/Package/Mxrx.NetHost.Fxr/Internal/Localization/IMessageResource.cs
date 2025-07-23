namespace Mxrx.NetHost.Internal.Localization;

/// <summary>
/// This interface exposes a message resource type.
/// </summary>
internal partial interface IMessageResource
{
	/// <summary>
	/// Current instance.
	/// </summary>
	private protected static abstract IMessageResource Instance { get; }

	/// <summary>
	/// Message for non-Native AOT runtime.
	/// </summary>
	String AotRequired { get; }
	/// <summary>
	/// Message for non-Windows operating system.
	/// </summary>
	String WindowsRequired { get; }
	/// <summary>
	/// Message for non-Unix-like operating system.
	/// </summary>
	String UnixRequired { get; }
	/// <summary>
	/// Message for inactive framework resolver.
	/// </summary>
	String ActiveFrameworkResolver { get; }
	/// <summary>
	/// Message for closed host context.
	/// </summary>
	String HostClosed { get; }
	/// <summary>
	/// Message for missing assembly information.
	/// </summary>
	String MissingAssembly { get; }
	/// <summary>
	/// Message for missing method name.
	/// </summary>
	String MissingMethodName { get; }
	/// <summary>
	/// Message for reflection-free mode.
	/// </summary>
	String ReflectionRequired { get; }
	/// <summary>
	/// Message for empty text.
	/// </summary>
	String EmptyText { get; }

	/// <summary>
	/// Message for invalid hostfxr library.
	/// </summary>
	/// <param name="methodName">Not found method name.</param>
	String InvalidLibrary(String methodName);
	/// <summary>
	/// Message for invalid result.
	/// </summary>
	/// <param name="callResult">Runtime call result.</param>
	String InvalidResult(RuntimeCallResult callResult);
	/// <summary>
	/// Message for invalid delegate type.
	/// </summary>
	/// <param name="type">Invalid type.</param>
	String InvalidDelegateType(Type type);
	/// <summary>
	/// Message for invalid delegate name.
	/// </summary>
	/// <param name="type">Managed type.</param>
	String InvalidTypeName(Type type);
}