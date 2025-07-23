namespace Mxrx.NetHost.Internal.Localization;

/// <summary>
/// French message resource.
/// </summary>
#if !PACKAGE
[ExcludeFromCodeCoverage]
#endif
internal sealed class FrenchMessageResource : IMessageResource
{
	/// <inheritdoc cref="IMessageResource.Instance"/>
	private static readonly FrenchMessageResource instance = new();

	static IMessageResource IMessageResource.Instance => FrenchMessageResource.instance;

	/// <summary>
	/// Private constructor.
	/// </summary>
	private FrenchMessageResource() { }

	String IMessageResource.AotRequired => "Cette opération nécessite un runtime Native AOT.";
	String IMessageResource.WindowsRequired => "Cette opération nécessite le système d'exploitation Microsoft Windows.";
	String IMessageResource.UnixRequired => "Cette opération nécessite un système de type Unix.";
	String IMessageResource.ActiveFrameworkResolver => "Un résolveur de framework est déjà actif.";
	String IMessageResource.HostClosed => "Le contexte hôte actuel est fermé.";
	String IMessageResource.MissingAssembly => "Nom de l'assembly ou données binaires manquantes.";
	String IMessageResource.MissingMethodName => "Nom de méthode manquant.";
	String IMessageResource.ReflectionRequired => "Cette opération nécessite la réflexion à l'exécution.";
	String IMessageResource.EmptyText => "Le texte fourni est vide ou invalide.";

	String IMessageResource.InvalidLibrary(String methodName)
		=> $"Bibliothèque hostfxr invalide : symbole '{methodName}' introuvable.";
	String IMessageResource.InvalidResult(RuntimeCallResult callResult)
		=> $"Résultat invalide : {Enum.GetName(callResult) ?? $"0x{(UInt32)callResult:x8}"}.";
	String IMessageResource.InvalidDelegateType(Type type) => $"{type} n'est pas un type de délégué valide.";
	String IMessageResource.InvalidTypeName(Type type) => $"{type} a un nom invalide.";
}