namespace Mxrx.NetHost;

/// <summary>
/// Stores a volatile text parameter.
/// </summary>
#if !PACKAGE
[ExcludeFromCodeCoverage]
#endif
public ref partial struct VolatileText
{
	/// <summary>
	/// Internal value.
	/// </summary>
	internal readonly TextParameter Text;
	/// <summary>
	/// Indicates whether current instance is disposed.
	/// </summary>
	internal Invalidator IsDisposed;

	/// <summary>
	/// Text value.
	/// </summary>
	private ReadOnlySpan<Byte> Value
	{
		get
		{
			if (this.IsValid) return this.Text.Value;
			IMessageResource resource = IMessageResource.GetInstance();
			throw new InvalidOperationException(resource.EmptyText);
		}
	}
	/// <summary>
	/// Indicates whether current instance is empty.
	/// </summary>
	internal Boolean IsEmpty => this.Text.IsEmpty;

	/// <summary>
	/// Binary length of current text.
	/// </summary>
	public Int32 BinaryLenght => this.Value.Length;
	/// <summary>
	/// Indicates whether the current text is null-terminated.
	/// </summary>
	public Boolean NullTerminated => this.Text.NullTerminated;
	/// <summary>
	/// Indicates whether the current text is UTF-8 encoded.
	/// </summary>
	public Boolean IsUtf8 => this.Text.IsUtf8;
	/// <summary>
	/// Indicates whether the current text is valid.
	/// </summary>
	public Boolean IsValid => !this.IsDisposed.Value;

	/// <summary>
	/// Inmutable UTF-16 property value.
	/// </summary>
	private String? _sValue;
	/// <summary>
	/// Inmutable UTF-8 property value.
	/// </summary>
	private CString? _cValue;

	/// <summary>
	/// Private constructor.
	/// </summary>
	/// <param name="text">Internal <see cref="TextParameter"/> instance.</param>
	internal VolatileText(TextParameter text)
	{
		this.Text = text;
		this.IsDisposed = default;
	}

	/// <summary>
	/// Retrieves text as <see cref="String"/>.
	/// </summary>
	/// <returns>A <see cref="String"/> instance.</returns>
	public String GetStringValue()
	{
		if (this._sValue is not null)
			return this._sValue;
		if (this.Value.IsEmpty)
			return this._sValue = String.Empty;

		return this._sValue = this.Text.IsUtf8 ?
			Encoding.UTF8.GetString(this.Text.Value) :
			new(this.Value.AsValues<Byte, Char>());
	}
	/// <summary>
	/// Retrieves text value as <see cref="CString"/>.
	/// </summary>
	/// <returns>A <see cref="CString"/> instance.</returns>
	public CString GetCStringValue()
	{
		if (this._cValue is not null)
			return this._cValue;
		if (this.Value.IsEmpty)
			return this._cValue = CString.Empty;

		if (this.Text.IsUtf8)
			return this._cValue = new(this.Value);

		ReadOnlySpan<Char> chars = this.Value.AsValues<Byte, Char>();
		Int32 length = Encoding.UTF8.GetByteCount(chars);
		Byte[] bytes = new Byte[length + 1];
		Encoding.UTF8.GetBytes(chars, bytes);
		return this._cValue = bytes;
	}
	/// <summary>
	/// Returns a raw managed reference that can be used for pinning.
	/// </summary>
	/// <returns>A raw managed reference to be used on pinning.</returns>
	public ref readonly Byte GetPinnableReference()
	{
		if (!this.IsValid)
			return ref Unsafe.NullRef<Byte>();
		return ref MemoryMarshal.GetReference(this.Value);
	}

	/// <summary>
	/// Creates a <see cref="VolatileText"/> from <paramref name="value"/>.
	/// </summary>
	/// <param name="value">Text value.</param>
	/// <returns>A <see cref="VolatileText"/> instance.</returns>
	public static VolatileText Create(String? value)
	{
		ReadOnlySpan<Char> span = value;
		VolatileText result = new(span) { _sValue = value, };
		return result;
	}
	/// <summary>
	/// Creates a <see cref="VolatileText"/> from <paramref name="value"/>.
	/// </summary>
	/// <param name="value">Text value.</param>
	/// <returns>A <see cref="VolatileText"/> instance.</returns>
	public static VolatileText Create(CString? value)
	{
		Boolean nullTerminated = value?.IsNullTerminated ?? false;
		VolatileText result = new(TextParameter.Create(value, nullTerminated)) { _cValue = value, };
		return result;
	}
	/// <summary>
	/// Creates a <see cref="VolatileText"/> from <paramref name="value"/>.
	/// </summary>
	/// <param name="value">Text span.</param>
	/// <returns>A <see cref="VolatileText"/> instance.</returns>
	public static VolatileText Create(ReadOnlySpan<Char> value) => new(value);
	/// <summary>
	/// Creates a <see cref="VolatileText"/> from <paramref name="value"/>.
	/// </summary>
	/// <param name="value">Text span.</param>
	/// <returns>A <see cref="VolatileText"/> instance.</returns>
	public static VolatileText Create(ReadOnlySpan<Byte> value)
		=> new(TextParameter.Create(value, TextParameter.IsLiteral(value)));

	/// <summary>
	/// Creates a literal <see cref="TextParameter"/> instance from <paramref name="value"/>.
	/// </summary>
	/// <param name="value">Text span.</param>
	/// <returns>Created <see cref="TextParameter"/> instance.</returns>
	internal static VolatileText CreateLiteral(ReadOnlySpan<Byte> value) => new(TextParameter.CreateLiteral(value));
	/// <summary>
	/// Creates a literal <see cref="TextParameter"/> instance from <paramref name="value"/>.
	/// </summary>
	/// <param name="value">Text span.</param>
	/// <returns>Created <see cref="TextParameter"/> instance.</returns>
	internal static VolatileText CreateLiteral(ReadOnlySpan<Char> value) => new(TextParameter.CreateLiteral(value));
}