namespace Mxrx.NetHost;

/// <summary>
/// Parameters for assembly loading.
/// </summary>
public ref partial struct LoadAssemblyParameters
{
	/// <summary>
	/// Assembly path <see cref="TextParameter"/> instance.
	/// </summary>
	internal TextParameter AssemblyPath { get; private set; }
	/// <summary>
	/// Assembly bytes.
	/// </summary>
	internal ReadOnlySpan<Byte> AssemblyBytes { get; private set; }
	/// <summary>
	/// Assembly symbol bytes.
	/// </summary>
	internal ReadOnlySpan<Byte> SymbolsBytes { get; private set; }

	/// <summary>
	/// Initializes a builder for <see cref="LoadAssemblyParameters"/> instance.
	/// </summary>
	/// <returns>A <see cref="Builder"/> instance.</returns>
	public static Builder CreateBuilder() => new();

	/// <summary>
	/// Builder struct for <see cref="LoadAssemblyParameters"/> type.
	/// </summary>
	public ref partial struct Builder()
	{
		/// <summary>
		/// Initializes assembly path.
		/// </summary>
		/// <param name="assemblyPath">Assembly path.</param>
		/// <returns>Current builder instance.</returns>
		public Builder WithAssemblyPathPath(String? assemblyPath)
		{
			ReadOnlySpan<Char> span = assemblyPath;
			return this.WithAssemblyPathPath(span);
		}
		/// <summary>
		/// Initializes assembly path.
		/// </summary>
		/// <param name="assemblyPath">Assembly path.</param>
		/// <returns>Current builder instance.</returns>
		public Builder WithAssemblyPathPath(CString? assemblyPath)
		{
			ReadOnlySpan<Byte> span = assemblyPath;
			Boolean? nullTerminated = assemblyPath?.IsNullTerminated;
			return this.WithAssemblyPathPath(span, nullTerminated.GetValueOrDefault());
		}
		/// <summary>
		/// Initializes assembly path.
		/// </summary>
		/// <param name="assemblyPath">Assembly path.</param>
		/// <returns>Current builder instance.</returns>
		public Builder WithAssemblyPathPath(ReadOnlySpan<Char> assemblyPath)
		{
			this._parameters.AssemblyPath = assemblyPath;
			this._parameters.AssemblyBytes = default;
			this._parameters.SymbolsBytes = default;
			return this;
		}
		/// <summary>
		/// Initializes assembly path.
		/// </summary>
		/// <param name="assemblyPath">Assembly path.</param>
		/// <returns>Current builder instance.</returns>
		public Builder WithAssemblyPathPath(ReadOnlySpan<Byte> assemblyPath)
			=> this.WithAssemblyPathPath(assemblyPath, TextParameter.IsLiteral(assemblyPath));
		/// <summary>
		/// Initializes assembly binary information.
		/// </summary>
		/// <param name="assemblyBytes">Bytes of dll file.</param>
		/// <param name="symbolsBytes">Bytes of pdb file.</param>
		/// <returns>Current builder instance.</returns>
		public Builder WithBytes(ReadOnlySpan<Byte> assemblyBytes, ReadOnlySpan<Byte> symbolsBytes = default)
		{
			this._parameters.AssemblyPath = default;
			this._parameters.AssemblyBytes = assemblyBytes;
			this._parameters.SymbolsBytes = symbolsBytes;
			return this;
		}
		/// <summary>
		/// Builds initialization information instance.
		/// </summary>
		/// <returns>Created <see cref="LoadAssemblyParameters"/> instance.</returns>
		public LoadAssemblyParameters Build()
		{
			if (!this._parameters.AssemblyPath.IsEmpty || !this._parameters.AssemblyBytes.IsEmpty)
				return this._parameters;
			IMessageResource resource = IMessageResource.GetInstance();
			throw new ArgumentException(resource.MissingAssembly);
		}
	}
}