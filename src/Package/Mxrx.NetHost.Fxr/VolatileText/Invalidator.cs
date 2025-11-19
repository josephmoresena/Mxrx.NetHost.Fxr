namespace Mxrx.NetHost;

public ref partial struct VolatileText
{
	/// <summary>
	/// Struct for text invalidation
	/// </summary>
	internal readonly struct Invalidator(IWrapper<Boolean>? isDisposable, IWrapper<Boolean>? contextControl)
		: IWrapper<Boolean>
	{
		/// <inheritdoc cref="IWrapper.IBase{Boolean}.Value"/>
		public Boolean Value => (isDisposable?.Value ?? false) || (contextControl?.Value ?? false);
	}
}