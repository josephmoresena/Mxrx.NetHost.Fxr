namespace Mxrx.NetHost;

public ref partial struct LoadAssemblyParameters
{
	public ref partial struct Builder
	{
		/// <summary>
		/// Building information.
		/// </summary>
		private LoadAssemblyParameters _parameters = new();

		/// <summary>
		/// Initializes assembly path.
		/// </summary>
		/// <param name="assemblyPath">Assembly path.</param>
		/// <param name="isLiteral">Indicates path span is a UTF-8 literal.</param>
		/// <returns>Current builder instance.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Builder WithAssemblyPathPath(ReadOnlySpan<Byte> assemblyPath, Boolean isLiteral)
		{
			this._parameters.AssemblyPath = TextParameter.Create(assemblyPath, isLiteral);
			this._parameters.AssemblyBytes = default;
			this._parameters.SymbolsBytes = default;
			return this;
		}
	}
}