namespace Mxrx.NetHost.Internal.Localization;

/// <summary>
/// Russian message resource.
/// </summary>
internal sealed class RussianMessageResource : IMessageResource
{
	/// <inheritdoc cref="IMessageResource.Instance"/>
	private static readonly RussianMessageResource instance = new();

	static IMessageResource IMessageResource.Instance => RussianMessageResource.instance;

	/// <summary>
	/// Private constructor.
	/// </summary>
	private RussianMessageResource() { }

	String IMessageResource.AotRequired => "Для этой операции требуется среда выполнения Native AOT.";
	String IMessageResource.WindowsRequired => "Для этой операции требуется операционная система Microsoft Windows.";
	String IMessageResource.UnixRequired => "Для этой операции требуется Unix-подобная операционная система.";
	String IMessageResource.ActiveFrameworkResolver => "Уже существует активный резолвер фреймворка.";
	String IMessageResource.HostClosed => "Текущий контекст хоста закрыт.";
	String IMessageResource.MissingAssembly => "Отсутствует имя сборки или двоичные данные.";
	String IMessageResource.MissingMethodName => "Отсутствует имя метода.";
	String IMessageResource.ReflectionRequired => "Для этой операции требуется рефлексия во время выполнения.";
	String IMessageResource.EmptyText => "Предоставленный текст пуст или недопустим.";

	String IMessageResource.InvalidLibrary(String methodName)
		=> $"Недопустимая библиотека hostfxr: символ '{methodName}' не найден.";
	String IMessageResource.InvalidResult(RuntimeCallResult callResult)
		=> $"Недопустимый результат: {Enum.GetName(callResult) ?? $"0x{(UInt32)callResult:x8}"}.";
	String IMessageResource.InvalidDelegateType(Type type) => $"{type} не является допустимым типом делегата.";
	String IMessageResource.InvalidTypeName(Type type) => $"{type} имеет недопустимое имя.";
}