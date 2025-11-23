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
	/// <summary>
	/// Indicates whether the current key/value pair is valid.
	/// </summary>
	public Boolean IsValid => this.Key.IsValid && this.Value.IsValid;

	/// <inheritdoc/>
	public override String ToString()
	{
		try
		{
			return $"{{ {this.Key.GetStringValue()} : {this.Value.GetStringValue()} }}";
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}
}