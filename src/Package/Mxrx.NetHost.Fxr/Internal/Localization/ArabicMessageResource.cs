namespace Mxrx.NetHost.Internal.Localization;

/// <summary>
/// Arabic message resource.
/// </summary>
internal sealed class ArabicMessageResource : IMessageResource
{
	/// <inheritdoc cref="IMessageResource.Instance"/>
	private static readonly ArabicMessageResource instance = new();

	static IMessageResource IMessageResource.Instance => ArabicMessageResource.instance;

	/// <summary>
	/// Private constructor.
	/// </summary>
	private ArabicMessageResource() { }

	String IMessageResource.AotRequired => "تتطلب هذه العملية وقت تشغيل Native AOT.";
	String IMessageResource.WindowsRequired => "تتطلب هذه العملية نظام التشغيل Microsoft Windows.";
	String IMessageResource.UnixRequired => "تتطلب هذه العملية نظام تشغيل شبيه بـ Unix.";
	String IMessageResource.ActiveFrameworkResolver => "يوجد محلل إطار عمل نشط بالفعل.";
	String IMessageResource.HostClosed => "سياق المضيف الحالي مغلق.";
	String IMessageResource.MissingAssembly => "اسم التجميع أو البيانات الثنائية مفقودة.";
	String IMessageResource.MissingMethodName => "اسم الطريقة مفقود.";
	String IMessageResource.ReflectionRequired => "تتطلب هذه العملية الانعكاس في وقت التشغيل.";
	String IMessageResource.EmptyText => "النص المُقدم فارغ أو غير صالح.";

	String IMessageResource.InvalidLibrary(String methodName)
		=> $"مكتبة hostfxr غير صالحة: الرمز '{methodName}' غير موجود.";
	String IMessageResource.InvalidResult(RuntimeCallResult callResult)
		=> $"النتيجة غير صالحة: {Enum.GetName(callResult) ?? $"0x{(UInt32)callResult:x8}"}.";
	String IMessageResource.InvalidDelegateType(Type type) => $"{type} ليس نوع مفوض صالح.";
	String IMessageResource.InvalidTypeName(Type type) => $"{type} يحتوي على اسم غير صالح.";
}