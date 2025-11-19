namespace Mxrx.NetHost;

#if !PACKAGE
[SuppressMessage(Constants.CSharpSquid, Constants.CheckIdS6640,
                 Justification = Constants.SecureUnsafeCodeJustification)]
#endif
public readonly unsafe ref partial struct RuntimePropertyCollection
{
	/// <summary>
	/// Delegate. Retrieves the <see cref="VolatileText"/> from <paramref name="textPtr"/> using
	/// <paramref name="hostContext"/>.
	/// </summary>
	/// <returns>A <see cref="VolatileText"/> instance.</returns>
	private delegate VolatileText GetText(HostContext hostContext, IntPtr textPtr);

	/// <summary>
	/// Creates a <see cref="VolatileText"/> using a <see cref="HostContext"/> instance and the text pointer.
	/// </summary>
	private static readonly GetText getText = SystemInfo.IsWindows ?
		RuntimePropertyCollection.GetUtf16Text :
		RuntimePropertyCollection.GetUtf8Text;

	/// <summary>
	/// Retrieves the <see cref="VolatileText"/> from <paramref name="textPtr"/> using
	/// <paramref name="hostContext"/>.
	/// </summary>
	/// <returns>A <see cref="VolatileText"/> instance.</returns>
	private static VolatileText GetUtf16Text(HostContext hostContext, IntPtr textPtr)
	{
		ReadOnlySpan<Char> chars = MemoryMarshal.CreateReadOnlySpanFromNullTerminated((Char*)textPtr);
		TextParameter text = TextParameter.CreateLiteral(chars);
		VolatileText result = new(text);
		hostContext.Attach(ref result);
		return result;
	}
	/// <summary>
	/// Retrieves the <see cref="VolatileText"/> from <paramref name="textPtr"/> using
	/// <paramref name="hostContext"/>.
	/// </summary>
	/// <returns>A <see cref="VolatileText"/> instance.</returns>
	private static VolatileText GetUtf8Text(HostContext hostContext, IntPtr textPtr)
	{
		ReadOnlySpan<Byte> bytes = MemoryMarshal.CreateReadOnlySpanFromNullTerminated((Byte*)textPtr);
		TextParameter text = TextParameter.CreateLiteral(bytes);
		VolatileText result = new(text);
		hostContext.Attach(ref result);
		return result;
	}
}