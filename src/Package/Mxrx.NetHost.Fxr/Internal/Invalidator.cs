namespace Mxrx.NetHost.Internal;

/// <summary>
/// Struct for text invalidation
/// </summary>
internal readonly struct TextInvalidator(IWrapper<Boolean>? isDisposable, IWrapper<Boolean>? contextControl)
	: IWrapper<Boolean>
{
	/// <inheritdoc cref="IWrapper.IBase{Boolean}.Value"/>
	public Boolean Value => (isDisposable?.Value ?? false) || (contextControl?.Value ?? false);
}