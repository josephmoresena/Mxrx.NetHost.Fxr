namespace Mxrx.NetHost.Internal;

/// <summary>
/// Text class helper.
/// </summary>
internal abstract partial class TextHelper
{
	/// <summary>
	/// Singleton instance.
	/// </summary>
	private static readonly TextHelper instance = SystemInfo.IsWindows ? new Windows() : new Unix();

	/// <summary>
	/// Retrieves a managed <see cref="NativeChar"/> reference from <paramref name="text"/>.
	/// </summary>
	/// <param name="text">A <see cref="TextParameter"/> instance.</param>
	/// <param name="chars">Output. Created <see cref="Array"/> array.</param>
	/// <returns>A managed <see cref="NativeChar"/> reference from <paramref name="text"/>.</returns>
	public static ref NativeChar GetRef(TextParameter text, out Array? chars)
		=> ref TextHelper.instance.GetRefImpl(text, out chars);
	/// <summary>
	/// Loads addresses to arguments values from <paramref name="args"/> at <paramref name="addr"/>
	/// </summary>
	/// <param name="args">A <see cref="ArgumentsParameter"/> instance.</param>
	/// <param name="addr">Destination addresses span.</param>
	/// <param name="handles">Destination handles span.</param>
	public static Int32 LoadArgsAddr(ArgumentsParameter args, Span<NativeCharPointer> addr, Span<ArgHandle> handles)
		=> TextHelper.instance.LoadArgsAddrImpl(args, addr, handles);
	/// <summary>
	/// Creates a literal <see cref="TextParameter"/> instance from <paramref name="charPointer"/>.
	/// </summary>
	/// <param name="charPointer">A null-terminated text pointer.</param>
	/// <returns>Created <see cref="TextParameter"/> instance.</returns>
	public static VolatileText CreateLiteral(NativeCharPointer charPointer)
		=> TextHelper.instance.CreateLiteralImpl(charPointer);
	/// <summary>
	/// Cleans up the initialization memory.
	/// </summary>
	/// <param name="arrays">Arrays.</param>
	/// <param name="handles">Arguments handles.</param>
	public static void Clean(ReadOnlySpan<Array?> arrays, Span<ArgHandle> handles = default)
		=> TextHelper.instance.CleanImpl(arrays, handles);
	/// <summary>
	/// Retrieves a <see cref="String"/> from <paramref name="chars"/>.
	/// </summary>
	/// <param name="chars">A read-only span of native chars.</param>
	/// <returns>A <see cref="String"/> instance.</returns>
	public static String GetString(ReadOnlySpan<NativeChar> chars) => TextHelper.instance.GetStringImpl(chars);

	/// <summary>
	/// Cleans up the initialization memory.
	/// </summary>
	/// <typeparam name="TChar">Text char type</typeparam>
	/// <param name="arrays">Arrays.</param>
	/// <param name="handles">Arguments handles.</param>
	private static void Clean<TChar>(ReadOnlySpan<TChar[]?> arrays, Span<ArgHandle> handles = default)
		where TChar : unmanaged, IEquatable<TChar>
	{
		foreach (TChar[]? array in arrays)
		{
			if (array is not null)
				ArrayPool<TChar>.Shared.Return(array);
		}

		foreach (ArgHandle handle in handles)
		{
			if (!handle.Handle.IsAllocated) break;
			if (handle is { FromArrayPool: true, Handle.Target: TChar[] arr, })
				ArrayPool<TChar>.Shared.Return(arr);
			handle.Handle.Free();
		}
	}
}