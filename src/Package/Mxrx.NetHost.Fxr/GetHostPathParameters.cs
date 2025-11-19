namespace Mxrx.NetHost;

/// <summary>
/// Parameters for get host path.
/// </summary>
#if !PACKAGE
[ExcludeFromCodeCoverage]
#endif
public ref partial struct GetHostPathParameters
{
	/// <summary>
	/// Host path <see cref="TextParameter"/> instance.
	/// </summary>
	internal TextParameter AssemblyPath { get; private set; }
	/// <summary>
	/// Root path <see cref="TextParameter"/> instance.
	/// </summary>
	internal TextParameter RootPath { get; private set; }

	/// <summary>
	/// Indicates whether current instance is empty.
	/// </summary>
	public Boolean IsEmpty => this.AssemblyPath.IsEmpty && this.RootPath.IsEmpty;

	/// <summary>
	/// Initializes a builder for <see cref="GetHostPathParameters"/> instance.
	/// </summary>
	/// <returns>A <see cref="Builder"/> instance.</returns>
	public static Builder CreateBuilder() => new();

	/// <summary>
	/// Builder struct for <see cref="GetHostPathParameters"/> type.
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
		/// Builds initialization parameters instance.
		/// </summary>
		/// <returns>Created <see cref="GetHostPathParameters"/> instance.</returns>
		public GetHostPathParameters Build() => this._parameters;
	}
}