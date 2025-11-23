namespace Mxrx.NetHost;

public ref partial struct GetHostPathParameters
{
	public ref partial struct Builder
	{
		/// <summary>
		/// Building parameters.
		/// </summary>
		private GetHostPathParameters _parameters = new();

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
		/// Initializes assembly path.
		/// </summary>
		/// <param name="assemblyPath">Assembly path.</param>
		/// <param name="isLiteral">Indicates path span is a UTF-8 literal.</param>
		/// <returns>Current builder instance.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Builder WithAssemblyPathPath(ReadOnlySpan<Byte> assemblyPath, Boolean isLiteral)
		{
			this._parameters.AssemblyPath = TextParameter.Create(assemblyPath, isLiteral);
			return this;
		}
	}
}