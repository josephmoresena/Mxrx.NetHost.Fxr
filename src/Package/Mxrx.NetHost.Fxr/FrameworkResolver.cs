namespace Mxrx.NetHost;

/// <summary>
/// This class exposes the <c>hostfxr</c> library.
/// </summary>
#if !PACKAGE
[SuppressMessage(Constants.CSharpSquid, Constants.CheckIdS3881, Justification = Constants.OptimizedJustification)]
#endif
public abstract partial class FrameworkResolver : IDisposable
{
	/// <summary>
	/// Library handle.
	/// </summary>
	public IntPtr Handle => !this._isDisposed ? this._handle : default;
	/// <inheritdoc/>
	public void Dispose()
	{
		Boolean disposing = this._handle != default && !this._clrInitialized && this._contexts.All(c => c.Closed);
		this.Dispose(!this._isDisposed && disposing);
		if (!disposing) return;

		this._isDisposed = true;
		GC.SuppressFinalize(this);
	}

	/// <summary>
	/// Initializes a <see cref="HostContext"/> instance.
	/// </summary>
	/// <param name="parameters">A <see cref="InitializationParameters"/> instance.</param>
	/// <returns>Initialized <see cref="HostContext"/> instance.</returns>
	public abstract HostContext Initialize(InitializationParameters parameters);
}