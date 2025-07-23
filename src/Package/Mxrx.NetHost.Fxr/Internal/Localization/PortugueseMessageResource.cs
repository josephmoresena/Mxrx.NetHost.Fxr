namespace Mxrx.NetHost.Internal.Localization;

/// <summary>
/// Portuguese message resource.
/// </summary>
#if !PACKAGE
[ExcludeFromCodeCoverage]
#endif
internal sealed class PortugueseMessageResource : IMessageResource
{
	/// <inheritdoc cref="IMessageResource.Instance"/>
	private static readonly PortugueseMessageResource instance = new();

	static IMessageResource IMessageResource.Instance => PortugueseMessageResource.instance;

	/// <summary>
	/// Private constructor.
	/// </summary>
	private PortugueseMessageResource() { }

	String IMessageResource.AotRequired => "Esta operação requer um runtime Native AOT.";
	String IMessageResource.WindowsRequired => "Esta operação requer o sistema operacional Microsoft Windows.";
	String IMessageResource.UnixRequired => "Esta operação requer um sistema operacional do tipo Unix.";
	String IMessageResource.ActiveFrameworkResolver => "Já existe um resolvedor de framework ativo.";
	String IMessageResource.HostClosed => "O contexto do host atual está fechado.";
	String IMessageResource.MissingAssembly => "Nome do assembly ou dados binários ausentes.";
	String IMessageResource.MissingMethodName => "Nome do método ausente.";
	String IMessageResource.ReflectionRequired => "Esta operação requer reflexão em tempo de execução.";
	String IMessageResource.EmptyText => "O texto fornecido está vazio ou inválido.";

	String IMessageResource.InvalidLibrary(String methodName)
		=> $"Biblioteca hostfxr inválida: símbolo '{methodName}' não encontrado.";
	String IMessageResource.InvalidResult(RuntimeCallResult callResult)
		=> $"Resultado inválido: {Enum.GetName(callResult) ?? $"0x{(UInt32)callResult:x8}"}.";
	String IMessageResource.InvalidDelegateType(Type type) => $"{type} não é um tipo de delegado válido.";
	String IMessageResource.InvalidTypeName(Type type) => $"{type} tem um nome inválido.";
}