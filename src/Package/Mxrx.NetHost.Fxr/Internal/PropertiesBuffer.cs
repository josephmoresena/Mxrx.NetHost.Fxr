namespace Mxrx.NetHost.Internal;

/// <summary>
/// Properties keys/values buffer.
/// </summary>
internal sealed class PropertiesBuffer
{
	/// <summary>
	/// Internal buffer.
	/// </summary>
	private readonly NativeCharPointer[] _buffer;

	/// <summary>
	/// Properties keys buffer.
	/// </summary>
	public Span<NativeCharPointer> Keys => this._buffer.AsSpan()[..this.Count];
	/// <summary>
	/// Properties values buffer.
	/// </summary>
	public Span<NativeCharPointer> Values => this._buffer.AsSpan()[this.Count..];
	/// <summary>
	/// Number of properties.
	/// </summary>
	public Int32 Count { get; }

	/// <summary>
	/// Constructor.
	/// </summary>
	/// <param name="size">Required buffer size.</param>
	public PropertiesBuffer(UIntPtr size)
	{
		this.Count = (Int32)size;
		this._buffer = ArrayPool<NativeCharPointer>.Shared.Rent(this.Count * 2);
	}

	~PropertiesBuffer() => ArrayPool<NativeCharPointer>.Shared.Return(this._buffer, true);
}