namespace Mxrx.NetHost.Internal;

/// <summary>
/// Handle of argument.
/// </summary>
#if !PACKAGE
[ExcludeFromCodeCoverage]
#endif
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
		GCHandle handle = ArgHandle.GetHandle(value);
		if (handle.IsAllocated)
			return new() { Handle = handle, FromArrayPool = false, };
		return default;
	}

	/// <summary>
	/// Retrieves the <see cref="GCHandle"/> from the pinning of <paramref name="value"/>.
	/// </summary>
	/// <param name="value">A <see cref="CString"/> to pin.</param>
	/// <returns>The <see cref="GCHandle"/> value from <paramref name="value"/> pinning.</returns>
	private static GCHandle GetHandle(CString value)
	{
		MemoryHandle handle = value.TryPin(out Boolean pinned);
		if (!pinned)
			return default;
		ref GCHandle result = ref ArgHandle.GetHandleRef(ref handle);
		if (result != default)
			return result;
		handle.Dispose();
		return default;
	}
	/// <summary>
	/// Retrieves a managed reference to the <see cref="GCHandle"/> instance of <paramref name="memoryHandle"/>.
	/// </summary>
	/// <param name="memoryHandle">Managed reference to a <see cref="MemoryHandle"/> instance.</param>
	/// <returns>
	/// A managed reference to the <see cref="GCHandle"/> instance of <paramref name="memoryHandle"/>.
	/// </returns>
	[UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_handle")]
	private static extern ref GCHandle GetHandleRef(ref MemoryHandle memoryHandle);
}