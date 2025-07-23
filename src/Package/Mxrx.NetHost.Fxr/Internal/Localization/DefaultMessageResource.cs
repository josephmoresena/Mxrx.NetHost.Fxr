namespace Mxrx.NetHost.Internal.Localization;

/// <summary>
/// Default (English) message resource.
/// </summary>
internal sealed class DefaultMessageResource : IMessageResource
{
	/// <inheritdoc cref="IMessageResource.Instance"/>
	private static readonly DefaultMessageResource instance = new();

	static IMessageResource IMessageResource.Instance => DefaultMessageResource.instance;

	/// <summary>
	/// Private constructor.
	/// </summary>
	private DefaultMessageResource() { }

	String IMessageResource.AotRequired => "This operation requires a Native AOT runtime.";
	String IMessageResource.WindowsRequired => "This operation requires the Microsoft Windows operating system.";
	String IMessageResource.UnixRequired => "This operation requires a Unix-like operating system.";
	String IMessageResource.ActiveFrameworkResolver => "There is already an active framework resolver.";
	String IMessageResource.HostClosed => "The current host context is closed.";
	String IMessageResource.MissingAssembly => "Missing assembly name or binary data.";
	String IMessageResource.MissingMethodName => "Missing method name.";
	String IMessageResource.ReflectionRequired => "This operation requires runtime reflection.";
	String IMessageResource.EmptyText => "The provided text is empty or invalid.";

	String IMessageResource.InvalidLibrary(String methodName)
		=> $"Invalid hostfxr library: symbol '{methodName}' not found.";
	String IMessageResource.InvalidResult(RuntimeCallResult callResult)
		=> $"Invalid result: {Enum.GetName(callResult) ?? $"0x{(UInt32)callResult:x8}"}.";
	String IMessageResource.InvalidDelegateType(Type type) => $"{type} is not a valid delegate type.";
	String IMessageResource.InvalidTypeName(Type type) => $"{type} has an invalid name.";
}