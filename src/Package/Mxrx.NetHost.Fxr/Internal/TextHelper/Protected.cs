namespace Mxrx.NetHost.Internal;

internal abstract partial class TextHelper
{
	/// <summary>
	/// Retrieves a managed <see cref="NativeChar"/> reference from <paramref name="text"/>.
	/// </summary>
	/// <param name="text">A <see cref="TextParameter"/> instance.</param>
	/// <param name="chars">Output. Created <see cref="Array"/> array.</param>
	/// <returns>A managed <see cref="NativeChar"/> reference from <paramref name="text"/>.</returns>
	protected abstract ref NativeChar GetRefImpl(TextParameter text, out Array? chars);
	/// <summary>
	/// Loads addresses to arguments values from <paramref name="args"/> at <paramref name="addr"/>
	/// </summary>
	/// <param name="args">A <see cref="ArgumentsParameter"/> instance.</param>
	/// <param name="addr">Destination addresses span.</param>
	/// <param name="handles">Destination handles span.</param>
	protected abstract Int32 LoadArgsAddrImpl(ArgumentsParameter args, Span<NativeCharPointer> addr,
		Span<ArgHandle> handles);
	/// <summary>
	/// Creates a literal <see cref="TextParameter"/> instance from <paramref name="charPointer"/>.
	/// </summary>
	/// <param name="charPointer">A null-terminated text pointer.</param>
	/// <returns>Created <see cref="TextParameter"/> instance.</returns>
	protected abstract VolatileText CreateLiteralImpl(NativeCharPointer charPointer);
	/// <summary>
	/// Cleans up the initialization memory.
	/// </summary>
	/// <param name="arrays">Arrays.</param>
	/// <param name="handles">Arguments handles.</param>
	protected abstract void CleanImpl(ReadOnlySpan<Array?> arrays, Span<ArgHandle> handles = default);
	/// <summary>
	/// Retrieves a <see cref="String"/> from <paramref name="chars"/>.
	/// </summary>
	/// <param name="chars">A read-only span of native chars.</param>
	/// <returns>A <see cref="String"/> instance.</returns>
	protected abstract String GetStringImpl(ReadOnlySpan<NativeChar> chars);
}