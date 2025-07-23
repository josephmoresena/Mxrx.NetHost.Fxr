namespace Mxrx.NetHost.Internal.Localization;

/// <summary>
/// Japanese message resource.
/// </summary>
#if !PACKAGE
[ExcludeFromCodeCoverage]
#endif
internal sealed class JapaneseMessageResource : IMessageResource
{
	/// <inheritdoc cref="IMessageResource.Instance"/>
	private static readonly JapaneseMessageResource instance = new();

	static IMessageResource IMessageResource.Instance => JapaneseMessageResource.instance;

	/// <summary>
	/// Private constructor.
	/// </summary>
	private JapaneseMessageResource() { }

	String IMessageResource.AotRequired => "この操作には Native AOT ランタイムが必要です。";
	String IMessageResource.WindowsRequired => "この操作には Microsoft Windows オペレーティングシステムが必要です。";
	String IMessageResource.UnixRequired => "この操作には Unix系オペレーティングシステムが必要です。";
	String IMessageResource.ActiveFrameworkResolver => "すでにアクティブなフレームワークリゾルバーがあります。";
	String IMessageResource.HostClosed => "現在のホストコンテキストは閉じられています。";
	String IMessageResource.MissingAssembly => "アセンブリ名またはバイナリデータがありません。";
	String IMessageResource.MissingMethodName => "メソッド名がありません。";
	String IMessageResource.ReflectionRequired => "この操作にはランタイムリフレクションが必要です。";
	String IMessageResource.EmptyText => "指定されたテキストが空または無効です。";

	String IMessageResource.InvalidLibrary(String methodName) => $"無効な hostfxr ライブラリ: シンボル '{methodName}' が見つかりません。";
	String IMessageResource.InvalidResult(RuntimeCallResult callResult)
		=> $"無効な結果: {Enum.GetName(callResult) ?? $"0x{(UInt32)callResult:x8}"}。";
	String IMessageResource.InvalidDelegateType(Type type) => $"{type} は有効なデリゲート型ではありません。";
	String IMessageResource.InvalidTypeName(Type type) => $"{type} の名前が無効です。";
}