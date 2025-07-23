namespace Mxrx.NetHost;

public abstract partial class FrameworkResolver
{
	/// <summary>
	/// Sets error writer for <paramref name="hostContext"/> instance.
	/// </summary>
	/// <param name="hostContext">A <see cref="HostHandle"/> instance.</param>
	/// <param name="clear">Indicates whether current error writer should be cleared.</param>
	[UnconditionalSuppressMessage("AOT", "IL3050", Justification = "Aot safe method.")]
	internal static void ConfigureErrorWriter(HostContext hostContext, Boolean clear)
	{
		IntPtr writeErrorPtr =
			clear ? IntPtr.Zero : Marshal.GetFunctionPointerForDelegate(hostContext.WriteErrorDelegate);
		hostContext.Resolver.ConfigureErrorWriter(hostContext, writeErrorPtr);
	}
	/// <summary>
	/// Runs application from <paramref name="hostContext"/> instance.
	/// </summary>
	/// <param name="hostContext">A <see cref="HostHandle"/> instance.</param>
	/// <param name="exitCode">Output. Execution exit code.</param>
	/// <returns>
	/// <see langword="true"/> if <paramref name="hostContext"/> was executed as application; otherwise,
	/// <see langword="false"/>.
	/// </returns>
	internal static Boolean Run(HostContext hostContext, out Int32 exitCode)
	{
		if (FrameworkResolver.applicationStarted)
		{
			exitCode = -1;
			return false;
		}
		exitCode = hostContext.Resolver.RunAsApplication(hostContext);
		FrameworkResolver.applicationStarted = true;
		return true;
	}
}