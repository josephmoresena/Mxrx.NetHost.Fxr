namespace Mxrx.NetHost.Internal;

/// <summary>
/// Handle of argument.
/// </summary>
internal struct ArgHandle
{
	/// <summary>
	/// A <see cref="GCHandle"/> for pinned argument.
	/// </summary>
	public GCHandle Handle { get; private init; }
	/// <summary>
	/// Indicates if target should be returned to an array pool.
	/// </summary>
	public Boolean FromArrayPool { get; private init; }

	/// <summary>
	/// Defines a conversion from <paramref name="handle"/> to a <see cref="ArgHandle"/> value.
	/// </summary>
	/// <param name="handle">A <see cref="GCHandle"/> value.</param>
	/// <returns>A <see cref="ArgHandle"/> value.</returns>
	public static implicit operator ArgHandle(GCHandle handle) => new() { Handle = handle, FromArrayPool = true, };
	/// <summary>
	/// Defines a conversion from <paramref name="value"/> to a <see cref="String"/> value.
	/// </summary>
	/// <param name="value">A <see cref="String"/> instance.</param>
	/// <returns>A <see cref="ArgHandle"/> value.</returns>
	public static implicit operator ArgHandle(String value)
	{
		GCHandle handle = GCHandle.Alloc(value, GCHandleType.Pinned);
		return new() { Handle = handle, FromArrayPool = false, };
	}
	/// <summary>
	/// Defines a conversion from <paramref name="value"/> to a <see cref="String"/> value.
	/// </summary>
	/// <param name="value">A <see cref="String"/> instance.</param>
	/// <returns>A <see cref="ArgHandle"/> value.</returns>
	public static implicit operator ArgHandle(CString value)
	{
		GCHandle handle = MemHandle.GetHandle(value);
		if (handle.IsAllocated)
			return new() { Handle = handle, FromArrayPool = false, };
		return default;
	}

	/// <summary>
	/// Struct to map <see cref="MemoryHandle"/> value.
	/// </summary>
#if !PACKAGE
	[SuppressMessage(Constants.CSharpSquid, Constants.CheckIdS6640,
	                 Justification = Constants.SecureUnsafeCodeJustification)]
	[SuppressMessage(Constants.CSharpSquid, Constants.CheckIdS3459,
	                 Justification = Constants.BinaryStructJustification)]
	[SuppressMessage(Constants.CSharpSquid, Constants.CheckIdS1144,
	                 Justification = Constants.BinaryStructJustification)]
#endif
	private unsafe struct MemHandle
	{
#pragma warning disable CS0649
#pragma warning disable CS0169
		private void* _pointer;
		private GCHandle _handle;
		private IPinnable? _pinnable;
#pragma warning restore CS0169
#pragma warning restore CS0649

		/// <summary>
		/// Retrieves the <see cref="GCHandle"/> from the pinning of <paramref name="value"/>.
		/// </summary>
		/// <param name="value">A <see cref="CString"/> to pin.</param>
		/// <returns>The <see cref="GCHandle"/> value from <paramref name="value"/> pinning.</returns>
		public static GCHandle GetHandle(CString value)
		{
			MemoryHandle handle = value.TryPin(out Boolean pinned);
			if (!pinned)
				return default;
			ref MemHandle replacement = ref Unsafe.As<MemoryHandle, MemHandle>(ref handle);
			if (replacement._handle != default)
				return replacement._handle;
			handle.Dispose();
			return default;
		}
	}
}