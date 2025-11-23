namespace Mxrx.NetHost.Internal.Localization;

/// <summary>
/// Spanish message resource.
/// </summary>
internal sealed class SpanishMessageResource : IMessageResource
{
	/// <summary>
	/// Singleton instance of <see cref="SpanishMessageResource"/>.
	/// </summary>
	private static readonly SpanishMessageResource instance = new();

	static IMessageResource IMessageResource.Instance => SpanishMessageResource.instance;

	/// <summary>
	/// Private constructor.
	/// </summary>
	private SpanishMessageResource() { }

	String IMessageResource.AotRequired => "Esta operación requiere un entorno de ejecución Native AOT.";
	String IMessageResource.WindowsRequired => "Esta operación requiere el sistema operativo Microsoft Windows.";
	String IMessageResource.UnixRequired => "Esta operación requiere un sistema operativo tipo Unix.";
	String IMessageResource.ActiveFrameworkResolver => "Ya hay un resolvente de frameworks activo.";
	String IMessageResource.HostClosed => "El contexto de host actual está cerrado.";
	String IMessageResource.MissingAssembly => "Falta el nombre del ensamblado o los datos binarios.";
	String IMessageResource.MissingMethodName => "Falta el nombre del método.";
	String IMessageResource.ReflectionRequired => "Esta operación requiere reflexión en tiempo de ejecución.";
	String IMessageResource.EmptyText => "El texto proporcionado está vacío o es inválido.";

	String IMessageResource.InvalidLibrary(String methodName)
		=> $"Biblioteca hostfxr inválida: símbolo '{methodName}' no encontrado.";
	String IMessageResource.InvalidResult(RuntimeCallResult callResult)
		=> $"Resultado inválido: {Enum.GetName(callResult) ?? $"0x{(UInt32)callResult:x8}"}.";
	String IMessageResource.InvalidDelegateType(Type type) => $"{type} no es un tipo de delegado válido.";
	String IMessageResource.InvalidTypeName(Type type) => $"{type} tiene un nombre inválido.";
}