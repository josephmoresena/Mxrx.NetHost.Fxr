namespace Mxrx.NetHost.Internal.Localization;

/// <summary>
/// Chinese message resource.
/// </summary>
internal sealed class ChineseMessageResource : IMessageResource
{
	/// <inheritdoc cref="IMessageResource.Instance"/>
	private static readonly ChineseMessageResource instance = new();

	static IMessageResource IMessageResource.Instance => ChineseMessageResource.instance;

	/// <summary>
	/// Private constructor.
	/// </summary>
	private ChineseMessageResource() { }

	String IMessageResource.AotRequired => "此操作需要 Native AOT 运行时。";
	String IMessageResource.WindowsRequired => "此操作需要 Microsoft Windows 操作系统。";
	String IMessageResource.UnixRequired => "此操作需要类 Unix 操作系统。";
	String IMessageResource.ActiveFrameworkResolver => "已经存在一个活动的框架解析器。";
	String IMessageResource.HostClosed => "当前的主机上下文已关闭。";
	String IMessageResource.MissingAssembly => "缺少程序集名称或二进制数据。";
	String IMessageResource.MissingMethodName => "缺少方法名称。";
	String IMessageResource.ReflectionRequired => "此操作需要运行时反射。";
	String IMessageResource.EmptyText => "提供的文本为空或无效。";

	String IMessageResource.InvalidLibrary(String methodName) => $"无效的 hostfxr 库：未找到符号 '{methodName}'。";
	String IMessageResource.InvalidResult(RuntimeCallResult callResult)
		=> $"无效的结果：{Enum.GetName(callResult) ?? $"0x{(UInt32)callResult:x8}"}。";
	String IMessageResource.InvalidDelegateType(Type type) => $"{type} 不是有效的委托类型。";
	String IMessageResource.InvalidTypeName(Type type) => $"{type} 的名称无效。";
}