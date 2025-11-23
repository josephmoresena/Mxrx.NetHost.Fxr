namespace Mxrx.NetHost;

public partial class FrameworkResolver
{
	/// <summary>
	/// Host Path parameters.
	/// </summary>
#if !PACKAGE
	[SuppressMessage(Constants.CSharpSquid, Constants.CheckIdS3881, Justification = Constants.OptimizedJustification)]
#endif
	[StructLayout(LayoutKind.Sequential)]
	private protected unsafe struct HostPathParameters
	{
		/// <summary>
		/// Size of the current structure.
		/// </summary>
		private readonly UIntPtr _size;
		/// <summary>
		/// Pointer to Assembly Path.
		/// </summary>
		private readonly void* _assemblyPath;
		/// <summary>
		/// Pointer to Root Path.
		/// </summary>
		private readonly void* _rootPath;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="assemblyPath"></param>
		/// <param name="rootPath"></param>
		public HostPathParameters(void* assemblyPath, void* rootPath)
		{
			this._size = (UIntPtr)sizeof(HostPathParameters);
			this._assemblyPath = assemblyPath;
			this._rootPath = rootPath;
		}
	}
}