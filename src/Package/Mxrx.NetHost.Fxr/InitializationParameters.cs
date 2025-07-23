namespace Mxrx.NetHost;

/// <summary>
/// Parameters for runtime context initialization.
/// </summary>
#if !PACKAGE
[ExcludeFromCodeCoverage]
#endif
public ref partial struct InitializationParameters
{
	/// <summary>
	/// Host path <see cref="TextParameter"/> instance.
	/// </summary>
	internal TextParameter HostPath { get; private set; }
	/// <summary>
	/// Root path <see cref="TextParameter"/> instance.
	/// </summary>
	internal TextParameter RootPath { get; private set; }
	/// <summary>
	/// Command <see cref="Arguments"/> instance.
	/// </summary>
	internal ArgumentsParameter Arguments { get; private set; }
	/// <summary>
	/// <c>.runtimeconfig.json</c> path <see cref="TextParameter"/> instance.
	/// </summary>
	internal TextParameter ConfigPath { get; private set; }

	/// <summary>
	/// Indicates whether current instance is empty.
	/// </summary>
	public Boolean IsEmpty => this.ConfigPath.IsEmpty && this.Arguments.IsEmpty && this.RootPath.IsEmpty;
	/// <summary>
	/// Indicates whether initialization is from Command Line.
	/// </summary>
	public Boolean InitializeCommand => this.ConfigPath.IsEmpty;

	/// <summary>
	/// Initializes a builder for <see cref="InitializationParameters"/> instance.
	/// </summary>
	/// <returns>A <see cref="Builder"/> instance.</returns>
	public static Builder CreateBuilder() => new();

	/// <summary>
	/// Builder struct for <see cref="InitializationParameters"/> type.
	/// </summary>
	public ref partial struct Builder()
	{
		/// <summary>
		/// Initializes host path.
		/// </summary>
		/// <param name="hostPath">Host path.</param>
		/// <returns>Current builder instance.</returns>
		public Builder WithHostPath(String? hostPath)
		{
			ReadOnlySpan<Char> span = hostPath;
			return this.WithHostPath(span);
		}
		/// <summary>
		/// Initializes host path.
		/// </summary>
		/// <param name="hostPath">Host path.</param>
		/// <returns>Current builder instance.</returns>
		public Builder WithHostPath(CString? hostPath)
		{
			ReadOnlySpan<Byte> span = hostPath;
			Boolean? nullTerminated = hostPath?.IsNullTerminated;
			return this.WithHostPath(span, nullTerminated.GetValueOrDefault());
		}
		/// <summary>
		/// Initializes host path.
		/// </summary>
		/// <param name="hostPath">Host path.</param>
		/// <returns>Current builder instance.</returns>
		public Builder WithHostPath(ReadOnlySpan<Char> hostPath)
		{
			this._parameters.HostPath = hostPath;
			return this;
		}
		/// <summary>
		/// Initializes host path.
		/// </summary>
		/// <param name="hostPath">Host path.</param>
		/// <returns>Current builder instance.</returns>
		public Builder WithHostPath(ReadOnlySpan<Byte> hostPath)
			=> this.WithHostPath(hostPath, TextParameter.IsLiteral(hostPath));

		/// <summary>
		/// Initializes root path.
		/// </summary>
		/// <param name="rootPath">Root path.</param>
		/// <returns>Current builder instance.</returns>
		public Builder WithRootPath(String? rootPath)
		{
			ReadOnlySpan<Char> span = rootPath;
			return this.WithRootPath(span);
		}
		/// <summary>
		/// Initializes root path.
		/// </summary>
		/// <param name="rootPath">Root path.</param>
		/// <returns>Current builder instance.</returns>
		public Builder WithRootPath(CString? rootPath)
		{
			ReadOnlySpan<Byte> span = rootPath;
			Boolean? nullTerminated = rootPath?.IsNullTerminated;
			return this.WithRootPath(span, nullTerminated.GetValueOrDefault());
		}
		/// <summary>
		/// Initializes root path.
		/// </summary>
		/// <param name="rootPath">Root path.</param>
		/// <returns>Current builder instance.</returns>
		public Builder WithRootPath(ReadOnlySpan<Char> rootPath)
		{
			this._parameters.RootPath = rootPath;
			return this;
		}
		/// <summary>
		/// Initializes root path.
		/// </summary>
		/// <param name="rootPath">Root path.</param>
		/// <returns>Current builder instance.</returns>
		public Builder WithRootPath(ReadOnlySpan<Byte> rootPath)
			=> this.WithRootPath(rootPath, TextParameter.IsLiteral(rootPath));

		/// <summary>
		/// Initializes runtime config path.
		/// </summary>
		/// <param name="configPath">Runtime config path.</param>
		/// <returns>Current builder instance.</returns>
		public Builder WithRuntimeConfigPath(String? configPath)
		{
			ReadOnlySpan<Char> span = configPath;
			return this.WithRuntimeConfigPath(span);
		}
		/// <summary>
		/// Initializes runtime config path.
		/// </summary>
		/// <param name="configPath">Runtime config path.</param>
		/// <returns>Current builder instance.</returns>
		public Builder WithRuntimeConfigPath(CString? configPath)
		{
			ReadOnlySpan<Byte> span = configPath;
			Boolean? nullTerminated = configPath?.IsNullTerminated;
			return this.WithRuntimeConfigPath(span, nullTerminated.GetValueOrDefault());
		}
		/// <summary>
		/// Initializes runtime config path.
		/// </summary>
		/// <param name="configPath">Runtime config path.</param>
		/// <returns>Current builder instance.</returns>
		public Builder WithRuntimeConfigPath(ReadOnlySpan<Char> configPath)
		{
			this._parameters.ConfigPath = configPath;
			this._parameters.Arguments = default;
			return this;
		}
		/// <summary>
		/// Initializes runtime config path.
		/// </summary>
		/// <param name="configPath">Runtime config path.</param>
		/// <returns>Current builder instance.</returns>
		public Builder WithRuntimeConfigPath(ReadOnlySpan<Byte> configPath)
			=> this.WithRuntimeConfigPath(configPath, TextParameter.IsLiteral(configPath));

		/// <summary>
		/// Initializes command arguments.
		/// </summary>
		/// <param name="args">Command arguments.</param>
		/// <returns>Current builder instance.</returns>
		public Builder WithArguments(params String?[] args)
		{
			this._parameters.Arguments = ArgumentsParameter.Create(args);
			this._parameters.ConfigPath = default;
			return this;
		}
		/// <summary>
		/// Initializes command arguments.
		/// </summary>
		/// <param name="args">Command arguments.</param>
		/// <returns>Current builder instance.</returns>
		public Builder WithArguments(ReadOnlySpan<String?> args)
		{
			this._parameters.Arguments = ArgumentsParameter.Create(args);
			this._parameters.ConfigPath = default;
			return this;
		}
		/// <summary>
		/// Initializes command arguments.
		/// </summary>
		/// <param name="args">Command arguments.</param>
		/// <returns>Current builder instance.</returns>
		public Builder WithArguments(params CString?[] args)
		{
			this._parameters.Arguments = ArgumentsParameter.Create(args);
			this._parameters.ConfigPath = default;
			return this;
		}
		/// <summary>
		/// Initializes command arguments.
		/// </summary>
		/// <param name="args">Command arguments.</param>
		/// <returns>Current builder instance.</returns>
		public Builder WithArguments(ReadOnlySpan<CString?> args)
		{
			this._parameters.Arguments = ArgumentsParameter.Create(args);
			this._parameters.ConfigPath = default;
			return this;
		}
		/// <summary>
		/// Initializes command arguments.
		/// </summary>
		/// <param name="sequence">Command arguments.</param>
		/// <returns>Current builder instance.</returns>
		public Builder WithArguments(CStringSequence sequence)
		{
			this._parameters.Arguments = sequence;
			this._parameters.ConfigPath = default;
			return this;
		}
		/// <summary>
		/// Builds initialization parameters instance.
		/// </summary>
		/// <returns>Created <see cref="InitializationParameters"/> instance.</returns>
		public InitializationParameters Build() => this._parameters;
	}
}