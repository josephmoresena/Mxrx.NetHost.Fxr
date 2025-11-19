namespace Mxrx.NetHost;

/// <summary>
/// Property key/value pair.
/// </summary>
public readonly ref struct RuntimePropertyPair
{
	/// <summary>
	/// Property Key.
	/// </summary>
	public VolatileText Key { get; internal init; }
	/// <summary>
	/// Property Value.
	/// </summary>
	public VolatileText Value { get; internal init; }

	/// <inheritdoc/>
	public override String ToString() => $"{{ {this.Key.GetStringValue()} : {this.Value.GetStringValue()} }}";
}