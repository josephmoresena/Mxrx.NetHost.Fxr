namespace Mxrx.NetHost;

public ref partial struct InitializationParameters
{
	public ref partial struct Builder
	{
		/// <summary>
		/// Building parameters.
		/// </summary>
		private InitializationParameters _parameters = new();

		/// <summary>
		/// Initializes runtime config path.
		/// </summary>
		/// <param name="configPath">Runtime config path.</param>
		/// <param name="isLiteral">Indicates path span is a UTF-8 literal.</param>
		/// <returns>Current builder instance.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Builder WithRuntimeConfigPath(ReadOnlySpan<Byte> configPath, Boolean isLiteral)
		{
			this._parameters.ConfigPath = TextParameter.Create(configPath, isLiteral);
			this._parameters.Arguments = default;
			return this;
		}
		/// <summary>
		/// Initializes root path.
		/// </summary>
		/// <param name="rootPath">Root path.</param>
		/// <param name="isLiteral">Indicates path span is a UTF-8 literal.</param>
		/// <returns>Current builder instance.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Builder WithRootPath(ReadOnlySpan<Byte> rootPath, Boolean isLiteral)
		{
			this._parameters.RootPath = TextParameter.Create(rootPath, isLiteral);
			return this;
		}
		/// <summary>
		/// Initializes host path.
		/// </summary>
		/// <param name="hostPath">Host path.</param>
		/// <param name="isLiteral">Indicates path span is a UTF-8 literal.</param>
		/// <returns>Current builder instance.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Builder WithHostPath(ReadOnlySpan<Byte> hostPath, Boolean isLiteral)
		{
			this._parameters.HostPath = TextParameter.Create(hostPath, isLiteral);
			return this;
		}
	}
}