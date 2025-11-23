namespace Mxrx.NetHost.Internal.Localization;

/// <summary>
/// Italian message resource.
/// </summary>
internal sealed class ItalianMessageResource : IMessageResource
{
	/// <inheritdoc cref="IMessageResource.Instance"/>
	private static readonly ItalianMessageResource instance = new();

	static IMessageResource IMessageResource.Instance => ItalianMessageResource.instance;

	/// <summary>
	/// Private constructor.
	/// </summary>
	private ItalianMessageResource() { }

	String IMessageResource.AotRequired => "Questa operazione richiede un runtime Native AOT.";
	String IMessageResource.WindowsRequired => "Questa operazione richiede il sistema operativo Microsoft Windows.";
	String IMessageResource.UnixRequired => "Questa operazione richiede un sistema operativo di tipo Unix.";
	String IMessageResource.ActiveFrameworkResolver => "Esiste già un resolver di framework attivo.";
	String IMessageResource.HostClosed => "Il contesto host corrente è chiuso.";
	String IMessageResource.MissingAssembly => "Nome dell'assembly o dati binari mancanti.";
	String IMessageResource.MissingMethodName => "Nome del metodo mancante.";
	String IMessageResource.ReflectionRequired => "Questa operazione richiede la reflection a runtime.";
	String IMessageResource.EmptyText => "Il testo fornito è vuoto o non valido.";

	String IMessageResource.InvalidLibrary(String methodName)
		=> $"Libreria hostfxr non valida: simbolo '{methodName}' non trovato.";
	String IMessageResource.InvalidResult(RuntimeCallResult callResult)
		=> $"Risultato non valido: {Enum.GetName(callResult) ?? $"0x{(UInt32)callResult:x8}"}.";
	String IMessageResource.InvalidDelegateType(Type type) => $"{type} non è un tipo di delegato valido.";
	String IMessageResource.InvalidTypeName(Type type) => $"{type} ha un nome non valido.";
}