namespace Mxrx.NetHost;

public partial class FrameworkResolver
{
	/// <summary>
	/// Created hosts.
	/// </summary>
	private readonly ConcurrentBag<HostContext> _contexts = [];
	/// <summary>
	/// Library handle.
	/// </summary>
	private readonly IntPtr _handle;
	/// <summary>
	/// Indicates whether CLR in current resolver is initialized.
	/// </summary>
	private Boolean _clrInitialized;
	/// <summary>
	/// Indicates whether current instance is disposed.
	/// </summary>
	private Boolean _isDisposed;

	/// <summary>
	/// Private constructor.
	/// </summary>
	/// <param name="handle">Library handle.</param>
	private FrameworkResolver(IntPtr handle)
	{
		this._handle = handle;
		this._isDisposed = handle != default;
	}
}