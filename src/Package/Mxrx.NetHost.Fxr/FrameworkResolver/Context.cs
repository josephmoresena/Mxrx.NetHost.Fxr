namespace Mxrx.NetHost;

public partial class FrameworkResolver
{
	/// <summary>
	/// <see cref="HostContext"/> implementation class.
	/// </summary>
	private sealed class ContextImpl : HostContext
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="resolver">A <see cref="FrameworkResolver"/> instance.</param>
		/// <param name="handle">A <see cref="HostHandle"/> instance</param>
		/// <param name="isCommandLine">Indicates whether current host is for a command line.</param>
		public ContextImpl(FrameworkResolver resolver, HostHandle handle, Boolean isCommandLine) : base(
			resolver, handle, isCommandLine)
			=> resolver._contexts.Add(this);
	}
}