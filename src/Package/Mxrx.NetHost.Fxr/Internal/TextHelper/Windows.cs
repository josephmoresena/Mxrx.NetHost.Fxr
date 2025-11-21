namespace Mxrx.NetHost.Internal;

internal abstract unsafe partial class TextHelper
{
	/// <summary>
	/// Text helper implementation for Windows OS.
	/// </summary>
	private sealed class Windows : TextHelper
	{
		/// <inheritdoc/>
		public override ref NativeChar GetRef(TextParameter text, out Array? chars)
		{
			chars = null;
			if (text.IsEmpty)
				return ref Unsafe.NullRef<NativeChar>();
			if (!text.IsUtf8 && text.NullTerminated)
				return ref MemoryMarshal.GetReference(text.Value.AsValues<Byte, NativeChar>());

			Int32 utf16Length = !text.IsUtf8 ?
				text.Value.Length / sizeof(Char) + 1 :
				Encoding.UTF8.GetCharCount(text.Value) + (!text.NullTerminated ? 1 : 0);
			chars = ArrayPool<Char>.Shared.Rent(utf16Length);
			Span<Char> charSpan = ((Char[])chars).AsSpan()[..utf16Length];

			if (!text.IsUtf8)
				text.Value.CopyTo(charSpan.AsBytes());
			else
				Encoding.UTF8.GetChars(text.Value, charSpan);
			charSpan[^1] = '\0';

			return ref MemoryMarshal.GetReference(charSpan.AsValues<Char, NativeChar>());
		}
		/// <inheritdoc/>
		public override Int32 LoadArgsAddr(ArgumentsParameter args, Span<NativeCharPointer> addr,
			Span<ArgHandle> handles)
		{
			if (args.IsEmpty) return 0;
			if (args.Sequence is null)
				return Windows.LoadFromSpan(args.Values, addr.AsValues<NativeCharPointer, ReadOnlyValPtr<Char>>(),
				                            handles);
			Windows.LoadFromSequence(args.Sequence, addr.AsValues<NativeCharPointer, ReadOnlyValPtr<Char>>(), handles);
			return args.Sequence.NonEmptyCount;
		}
		/// <inheritdoc/>
		public override VolatileText CreateLiteral(NativeCharPointer charPointer)
			=> VolatileText.CreateLiteral(
				MemoryMarshal.CreateReadOnlySpanFromNullTerminated((Char*)charPointer.Pointer));
		/// <inheritdoc/>
		public override void Clean(ReadOnlySpan<Array?> arrays, Span<ArgHandle> handles = default)
		{
			ref Char[]? aRef = ref Unsafe.As<Array?, Char[]?>(ref MemoryMarshal.GetReference(arrays));
			TextHelper.Clean(MemoryMarshal.CreateReadOnlySpan(ref aRef, arrays.Length), handles);
		}
		/// <inheritdoc/>
		public override String GetString(ReadOnlySpan<NativeChar> chars) => new(chars.AsValues<NativeChar, Char>());

		/// <summary>
		/// Loads addresses to arguments values from <paramref name="args"/> at <paramref name="addr"/>
		/// </summary>
		/// <param name="args">A <see cref="CStringSequence"/> instance.</param>
		/// <param name="addr">Destination addresses span.</param>
		/// <param name="handles">Destination handles span.</param>
		private static void LoadFromSequence(CStringSequence args, Span<ReadOnlyValPtr<Char>> addr,
			Span<ArgHandle> handles)
		{
			ReadOnlySpan<Byte> utfChars = args.ToString().AsSpan().AsBytes();
			Int32 charsCount = Encoding.UTF8.GetCharCount(utfChars);
			Span<Int32> offsets = stackalloc Int32[args.NonEmptyCount];
			Char[] chars = ArrayPool<Char>.Shared.Rent(charsCount);

			Encoding.UTF8.GetChars(utfChars, chars);
			chars[charsCount] = '\0';
			handles[0] = GCHandle.Alloc(chars, GCHandleType.Pinned);

			addr[0] = chars.AsSpan().GetUnsafeValPtr();
			for (Int32 i = 1; i < offsets.Length; i++)
			{
				Int32 offset = Encoding.UTF8.GetCharCount(utfChars[offsets[i - 1]..offsets[i]]);
				addr[i] = addr[i - 1] + offset;
			}
		}
		/// <summary>
		/// Loads addresses to arguments values from <paramref name="args"/> at <paramref name="addr"/>
		/// </summary>
		/// <param name="args">A <see cref="CStringSequence"/> instance.</param>
		/// <param name="addr">Destination addresses span.</param>
		/// <param name="handles">Destination handles span.</param>
		private static Int32 LoadFromSpan(ReadOnlySpan<Object?> args, Span<ReadOnlyValPtr<Char>> addr,
			Span<ArgHandle> handles)
		{
			Int32 ixHandle = 0;
			Int32 ixAddress = 0;
			for (Int32 i = 0; i < args.Length; i++)
			{
				String? value = args[i] is CString { Length: > 0, } c ? $"{c}\0" : args[i] as String;
				if (String.IsNullOrWhiteSpace(value))
				{
					addr[ixAddress] = default;
					ixAddress++;
					continue;
				}

				if (value.AsSpan()[^1] != '\0')
					value = $"{value}\0";

				handles[ixHandle] = value;
				addr[ixAddress] = (ReadOnlyValPtr<Char>)handles[ixHandle].Handle.AddrOfPinnedObject();
				ixHandle++;
				ixAddress++;
			}
			return ixAddress;
		}
	}
}