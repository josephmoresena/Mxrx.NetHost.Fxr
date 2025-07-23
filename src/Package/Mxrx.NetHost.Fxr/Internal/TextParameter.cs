namespace Mxrx.NetHost.Internal;

/// <summary>
/// Stores a text parameter.
/// </summary>
internal readonly ref struct TextParameter
{
	/// <summary>
	/// Text value.
	/// </summary>
	public ReadOnlySpan<Byte> Value { get; private init; }
	/// <summary>
	/// Indicates whether <see cref="Value"/> is null-terminated.
	/// </summary>
	public Boolean NullTerminated { get; private init; }
	/// <summary>
	/// Indicates whether <see cref="Value"/> is UTF-8 encoded.
	/// </summary>
	public Boolean IsUtf8 { get; private init; }
	/// <summary>
	/// Indicates whether current instance is empty.
	/// </summary>
	public Boolean IsEmpty => this.Value.IsEmpty;

	/// <summary>
	/// Defines an implicit conversion of a given <see cref="ReadOnlySpan{Char}"/> to <see cref="TextParameter"/>.
	/// </summary>
	/// <param name="value">A <see cref="ReadOnlySpan{Char}"/> to implicitly convert.</param>
	public static implicit operator TextParameter(ReadOnlySpan<Char> value)
	{
		if (value.IsEmpty)
			return default;
		return new()
		{
			IsUtf8 = false, Value = MemoryMarshal.Cast<Char, Byte>(value), NullTerminated = value[^1] == '\0',
		};
	}

	/// <summary>
	/// Creates a <see cref="TextParameter"/> instance from <paramref name="value"/>.
	/// </summary>
	/// <param name="value">Text span.</param>
	/// <param name="isLiteral">Indicates whether current instance is literal.</param>
	/// <returns>Created <see cref="TextParameter"/> instance.</returns>
	public static TextParameter Create(ReadOnlySpan<Byte> value, Boolean isLiteral)
	{
		if (value.IsEmpty)
			return default;

		Boolean nullTerminated = value[^1] == '\0';
		ReadOnlySpan<Byte> span = nullTerminated || !isLiteral ?
			value :
			MemoryMarshal.CreateReadOnlySpan(ref MemoryMarshal.GetReference(value), value.Length + 1);
		return new() { IsUtf8 = true, Value = span, NullTerminated = isLiteral || nullTerminated, };
	}
	/// <summary>
	/// Creates a literal <see cref="TextParameter"/> instance from <paramref name="value"/>.
	/// </summary>
	/// <typeparam name="TChar">Type of text character.</typeparam>
	/// <param name="value">Text span.</param>
	/// <returns>Created <see cref="TextParameter"/> instance.</returns>
	public static unsafe TextParameter CreateLiteral<TChar>(ReadOnlySpan<TChar> value)
		where TChar : unmanaged, IEquatable<TChar>
	{
		if (value.IsEmpty)
			return default;

		return new() { IsUtf8 = sizeof(TChar) == sizeof(Byte), Value = value.AsBytes(), NullTerminated = true, };
	}
	/// <summary>
	/// Indicates whether <paramref name="value"/> is a UTF-8 literal.
	/// </summary>
	/// <param name="value">A binary span.</param>
	/// <returns>
	/// <see langword="true"/> if <paramref name="value"/> is a UTF-8 literal; otherwise, <see langword="false"/>.
	/// </returns>
	public static Boolean IsLiteral(ReadOnlySpan<Byte> value)
	{
		ref Byte refValue = ref MemoryMarshal.GetReference(value);
		if (Unsafe.IsNullRef(ref refValue) || value.MayBeNonLiteral()) return false;
		return (Byte)'\0' == Unsafe.AddByteOffset(ref refValue, value.Length);
	}
}