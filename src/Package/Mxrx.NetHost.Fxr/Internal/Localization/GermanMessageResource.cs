namespace Mxrx.NetHost.Internal.Localization;

/// <summary>
/// German message resource.
/// </summary>
internal sealed class GermanMessageResource : IMessageResource
{
	/// <inheritdoc cref="IMessageResource.Instance"/>
	private static readonly GermanMessageResource instance = new();

	static IMessageResource IMessageResource.Instance => GermanMessageResource.instance;

	/// <summary>
	/// Private constructor.
	/// </summary>
	private GermanMessageResource() { }

	String IMessageResource.AotRequired => "Für diesen Vorgang ist eine Native AOT-Laufzeit erforderlich.";
	String IMessageResource.WindowsRequired
		=> "Für diesen Vorgang ist das Microsoft Windows-Betriebssystem erforderlich.";
	String IMessageResource.UnixRequired => "Für diesen Vorgang ist ein Unix-ähnliches Betriebssystem erforderlich.";
	String IMessageResource.ActiveFrameworkResolver => "Es ist bereits ein aktiver Framework-Resolver vorhanden.";
	String IMessageResource.HostClosed => "Der aktuelle Hostkontext ist geschlossen.";
	String IMessageResource.MissingAssembly => "Assembly-Name oder Binärdaten fehlen.";
	String IMessageResource.MissingMethodName => "Methodenname fehlt.";
	String IMessageResource.ReflectionRequired => "Dieser Vorgang erfordert Laufzeit-Reflexion.";
	String IMessageResource.EmptyText => "Der bereitgestellte Text ist leer oder ungültig.";

	String IMessageResource.InvalidLibrary(String methodName)
		=> $"Ungültige hostfxr-Bibliothek: Symbol '{methodName}' nicht gefunden.";
	String IMessageResource.InvalidResult(RuntimeCallResult callResult)
		=> $"Ungültiges Ergebnis: {Enum.GetName(callResult) ?? $"0x{(UInt32)callResult:x8}"}.";
	String IMessageResource.InvalidDelegateType(Type type) => $"{type} ist kein gültiger Delegatentyp.";
	String IMessageResource.InvalidTypeName(Type type) => $"{type} hat einen ungültigen Namen.";
}