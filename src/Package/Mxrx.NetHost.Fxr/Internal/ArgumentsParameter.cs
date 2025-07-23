namespace Mxrx.NetHost.Internal;

/// <summary>
/// Builder struct for <see cref="InitializationParameters"/> type.
/// </summary>
#if !PACKAGE
[ExcludeFromCodeCoverage]
#endif
public readonly ref struct ArgumentsParameter
{
	/// <summary>
	/// Arguments span.
	/// </summary>
	public ReadOnlySpan<Object?> Values { get; init; }
	/// <summary>
	/// Type of <see cref="Values"/> items.
	/// </summary>
	public Type TextType { get; init; }
	/// <summary>
	/// Utf-8 arguments sequence.
	/// </summary>
	public CStringSequence? Sequence { get; init; }

	/// <summary>
	/// Indicates whether current instance is empty.
	/// </summary>
	public Boolean IsEmpty => this.Values.IsEmpty && (this.Sequence is null || this.Sequence.Count == 0);
	/// <summary>
	/// Arguments count.
	/// </summary>
	public Int32 Count => this.Sequence?.Count ?? this.Values.Length;

	/// <summary>
	/// Defines an implicit conversion of a given <see cref="ReadOnlySpan{Char}"/> to <see cref="TextParameter"/>.
	/// </summary>
	/// <param name="value">A <see cref="ReadOnlySpan{Char}"/> to implicitly convert.</param>
	public static implicit operator ArgumentsParameter(CStringSequence value)
		=> new() { Values = default, TextType = typeof(CString), Sequence = value, };

	/// <summary>
	/// Creates a <see cref="ArgumentsParameter"/> instance from <paramref name="values"/>.
	/// </summary>
	/// <param name="values">Arguments values.</param>
	/// <returns>Created <see cref="ArgumentsParameter"/> instance.</returns>
	public static ArgumentsParameter Create<TValue>(TValue?[] values)
		where TValue : class, ICloneable, IEquatable<String>, IEquatable<TValue>, IComparable<String>,
		IComparable<TValue>
		=> new() { Values = values, TextType = typeof(TValue), Sequence = null, };
	/// <summary>
	/// Creates a <see cref="ArgumentsParameter"/> instance from <paramref name="values"/>.
	/// </summary>
	/// <param name="values">Arguments values.</param>
	/// <returns>Created <see cref="ArgumentsParameter"/> instance.</returns>
	public static ArgumentsParameter Create<TValue>(ReadOnlySpan<TValue?> values)
		where TValue : class, ICloneable, IEquatable<String>, IEquatable<TValue>, IComparable<String>,
		IComparable<TValue>
	{
		if (values.IsEmpty)
			return default;
		ref TValue? refT0 = ref MemoryMarshal.GetReference(values);
		ref Object? ref0 = ref Unsafe.As<TValue?, Object?>(ref refT0);
		return new()
		{
			Values = MemoryMarshal.CreateReadOnlySpan(ref ref0, values.Length),
			TextType = typeof(TValue),
			Sequence = null,
		};
	}
}