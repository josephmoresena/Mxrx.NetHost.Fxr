namespace Mxrx.NetHost.Internal;

internal abstract unsafe partial class TextHelper
{
	/// <summary>
	/// Text helper implementation for Unix-like OS.
	/// </summary>
	private sealed class Unix : TextHelper
	{
		/// <inheritdoc/>
		public override ref NativeChar GetRef(TextParameter text, out Array? chars)
		{
			chars = null;
			if (text.IsEmpty)
				return ref Unsafe.NullRef<NativeChar>();
			switch (text.IsUtf8)
			{
				case true when text.NullTerminated:
					return ref MemoryMarshal.GetReference(text.Value.AsValues<Byte, NativeChar>());
				case true:
					chars = ArrayPool<Byte>.Shared.Rent(text.Value.Length + 1);
					text.Value.CopyTo(((Byte[])chars).AsSpan());
					((Byte[])chars)[text.Value.Length] = (Byte)'\0';
					break;
				default:
				{
					ReadOnlySpan<Char> utf16Char = text.Value.AsValues<Byte, Char>();
					Int32 utf8Length = Encoding.UTF8.GetByteCount(utf16Char);
					Int32 extraLength = utf16Char[^1] != '\0' ? 1 : 0;
					chars = ArrayPool<Byte>.Shared.Rent(utf8Length + extraLength);
					Encoding.UTF8.GetBytes(utf16Char, (Byte[])chars);
					((Byte[])chars)[utf8Length] = default;
					break;
				}
			}

			((Byte[])chars)[^1] = (Byte)'\0';
			return ref MemoryMarshal.GetReference(((Byte[])chars).AsSpan().AsValues<Byte, NativeChar>());
		}
		/// <inheritdoc/>
		public override Int32 LoadArgsAddr(ArgumentsParameter args, Span<NativeCharPointer> addr,
			Span<ArgHandle> handles)
		{
			if (args.IsEmpty) return 0;
			if (args.Sequence is null)
				return Unix.LoadFromSpan(args.Values, addr.AsValues<NativeCharPointer, ReadOnlyValPtr<Byte>>(),
				                         handles);

			Unix.LoadFromSequence(args.Sequence, addr.AsValues<NativeCharPointer, ReadOnlyValPtr<Byte>>(), handles);
			return args.Sequence.NonEmptyCount;
		}
		/// <inheritdoc/>
		public override VolatileText CreateLiteral(NativeCharPointer charPointer)
			=> VolatileText.CreateLiteral(
				MemoryMarshal.CreateReadOnlySpanFromNullTerminated((Byte*)charPointer.Pointer));
		/// <inheritdoc/>
		public override void Clean(ReadOnlySpan<Array?> arrays, Span<ArgHandle> handles = default)
		{
			ref Byte[]? aRef = ref Unsafe.As<Array?, Byte[]?>(ref MemoryMarshal.GetReference(arrays));
			TextHelper.Clean(MemoryMarshal.CreateReadOnlySpan(ref aRef, arrays.Length), handles);
		}
		/// <inheritdoc/>
		public override String GetString(ReadOnlySpan<NativeChar> chars) => Encoding.UTF8.GetString(chars.AsBytes());

		/// <summary>
		/// Loads addresses to arguments values from <paramref name="args"/> at <paramref name="addr"/>
		/// </summary>
		/// <param name="args">A <see cref="CStringSequence"/> instance.</param>
		/// <param name="addr">Destination addresses span.</param>
		/// <param name="handles">Destination handles span.</param>
		private static void LoadFromSequence(CStringSequence args, Span<ReadOnlyValPtr<Byte>> addr,
			Span<ArgHandle> handles)
		{
			String buffer = args.ToString();
			ReadOnlyValPtr<Byte> ptr = buffer.AsSpan().AsBytes().GetUnsafeValPtr();
			Span<Int32> offsets = stackalloc Int32[args.NonEmptyCount];

			args.GetOffsets(offsets);
			handles[0] = buffer;

			for (Int32 i = 0; i < offsets.Length; i++)
				addr[i] = ptr + offsets[i];
		}
		/// <summary>
		/// Loads addresses to arguments values from <paramref name="args"/> at <paramref name="addr"/>
		/// </summary>
		/// <param name="args">A <see cref="CStringSequence"/> instance.</param>
		/// <param name="addr">Destination addresses span.</param>
		/// <param name="handles">Destination handles span.</param>
#if !PACKAGE
		[SuppressMessage(Constants.CSharpSquid, Constants.CheckIdS3776,
		                 Justification = Constants.OptimizedJustification)]
#endif
		private static Int32 LoadFromSpan(ReadOnlySpan<Object?> args, Span<ReadOnlyValPtr<Byte>> addr,
			Span<ArgHandle> handles)
		{
			Int32 ixHandle = 0;
			Int32 ixAddress = 0;
			for (Int32 i = 0; i < args.Length; i++)
			{
				Byte[]? arr;

				switch (args[i])
				{
					case null:
						continue;
					case CString c:
					{
						if (CString.IsNullOrEmpty(c)) continue;
						if (c.IsNullTerminated)
						{
							ReadOnlySpan<Byte> span = c.AsSpan();
							ArgHandle argH = c;
							if (argH.Handle.IsAllocated || !span.MayBeNonLiteral() || c.IsReference)
							{
								addr[ixAddress] = c.AsSpan().GetUnsafeValPtr();
								ixAddress++;
								if (argH.Handle.IsAllocated)
								{
									handles[ixHandle] = argH;
									ixHandle++;
								}
								continue;
							}
						}

						arr = ArrayPool<Byte>.Shared.Rent(c.Length + 1);
						c.AsSpan().CopyTo(arr);
						arr[c.Length] = (Byte)'\0';
						break;
					}
					default:
					{
						String? value = args[i]?.ToString();
						if (String.IsNullOrEmpty(value)) continue;

						Int32 utf8Length = Encoding.UTF8.GetByteCount(value);
						Int32 extraLength = value[^1] != '\0' ? 1 : 0;
						arr = ArrayPool<Byte>.Shared.Rent(value.Length + extraLength);
						Encoding.UTF8.GetBytes(value, arr);
						arr[utf8Length] = (Byte)'\0';
						break;
					}
				}

				handles[ixHandle] = GCHandle.Alloc(arr, GCHandleType.Pinned);
				addr[ixHandle] = (ReadOnlyValPtr<Byte>)handles[ixHandle].Handle.AddrOfPinnedObject();
				ixHandle++;
				ixAddress++;
			}
			return ixAddress;
		}
	}
}