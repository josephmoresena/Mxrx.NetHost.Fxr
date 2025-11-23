namespace Mxrx.NetHost;

/// <summary>
/// Visual Basic .NET compatibility class.
/// </summary>
public static class VbCompat
{
	/// <inheritdoc cref="LoadAssemblyParameters.CreateBuilder()"/>
	public static LoadAssemblyParameters.Builder CreateLoadAssemblyParametersBuilder()
		=> LoadAssemblyParameters.CreateBuilder();
	/// <inheritdoc cref="InitializationParameters.CreateBuilder()"/>
	public static InitializationParameters.Builder CreateInitializationParametersBuilder()
		=> InitializationParameters.CreateBuilder();
	/// <inheritdoc cref="GetHostPathParameters.CreateBuilder()"/>
	public static GetHostPathParameters.Builder CreateGetHostPathParametersBuilder()
		=> GetHostPathParameters.CreateBuilder();
	/// <inheritdoc cref="NetFunctionInfo.CreateBuilder()"/>
	public static NetFunctionInfo.Builder CreateNetFunctionInfoBuilder() => NetFunctionInfo.CreateBuilder();
	/// <inheritdoc cref="VolatileText.Create(String?)"/>
	public static VolatileText CreateText(String? value) => VolatileText.Create(value);
	/// <inheritdoc cref="VolatileText.Create(CString?)"/>
	public static VolatileText CreateText(CString? value) => VolatileText.Create(value);
}