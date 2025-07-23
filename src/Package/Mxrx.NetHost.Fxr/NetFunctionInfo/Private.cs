namespace Mxrx.NetHost;

public ref partial struct NetFunctionInfo
{
	public ref partial struct Builder
	{
		/// <summary>
		/// Building information.
		/// </summary>
		private NetFunctionInfo _info = new();

		/// <summary>
		/// Initializes delegate type name.
		/// </summary>
		/// <param name="delegateTypeName">Delegate type name.</param>
		/// <param name="isLiteral">Indicates delegate type name span is a UTF-8 literal.</param>
		/// <returns>Current builder instance.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Builder WithDelegateTypeName(ReadOnlySpan<Byte> delegateTypeName, Boolean isLiteral)
		{
			this._info.DelegateTypeName = TextParameter.Create(delegateTypeName, isLiteral);
			this._info.UseUnmanagedCallersOnly = false;
			return this;
		}
		/// <summary>
		/// Initializes method name.
		/// </summary>
		/// <param name="methodName">Method name.</param>
		/// <param name="isLiteral">Indicates method name span is a UTF-8 literal.</param>
		/// <returns>Current builder instance.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Builder WithMethodName(ReadOnlySpan<Byte> methodName, Boolean isLiteral)
		{
			this._info.MethodName = TextParameter.Create(methodName, isLiteral);
			return this;
		}
		/// <summary>
		/// Initializes type name.
		/// </summary>
		/// <param name="typeName">Fully-qualified type name.</param>
		/// <param name="isLiteral">Indicates type name span is a UTF-8 literal.</param>
		/// <returns>Current builder instance.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Builder WithTypeName(ReadOnlySpan<Byte> typeName, Boolean isLiteral)
		{
			this._info.TypeName = TextParameter.Create(typeName, isLiteral);
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
			this._info.AssemblyPath = TextParameter.Create(assemblyPath, isLiteral);
			return this;
		}

		/// <summary>
		/// Throws if reflection-free mode.
		/// </summary>
		/// <exception cref="InvalidOperationException">Throws if reflection-free mode.</exception>
		private static void ThrowIfNoReflection()
		{
			if (!AotInfo.IsReflectionDisabled) return;
			IMessageResource resource = IMessageResource.GetInstance();
			throw new InvalidOperationException(resource.ReflectionRequired);
		}
		/// <summary>
		/// Throws if <paramref name="type"/> is not a CLR of a delegate.
		/// </summary>
		/// <param name="type">A <see cref="Type"/> instance.</param>
		/// <exception cref="ArgumentException">
		/// Throws if <paramref name="type"/> is not a CLR of a delegate.
		/// </exception>
		private static void ThrowIfNoDelegate(Type type)
		{
			if (type.BaseType is { } baseType &&
			    (baseType == typeof(MulticastDelegate) || baseType == typeof(Delegate)))
				return;
			IMessageResource resource = IMessageResource.GetInstance();
			throw new ArgumentException(resource.InvalidDelegateType(type), nameof(type));
		}
		/// <summary>
		/// Retrieves the assembly qualified name from <paramref name="type"/>.
		/// </summary>
		/// <param name="type">A <see cref="Type"/> instance.</param>
		/// <returns>The assembly qualified name from <paramref name="type"/>.</returns>
		/// <exception cref="ArgumentException">Throws if invalid name.</exception>
		private static String GetAssemblyQualifiedName(Type type)
		{
			const String separator = ", ";
			String? typeName = type.AssemblyQualifiedName;
			Int32 indexof = typeName?.IndexOf(separator, StringComparison.InvariantCulture) ?? -1;
			if (String.IsNullOrWhiteSpace(typeName) || indexof < 0)
			{
				IMessageResource resource = IMessageResource.GetInstance();
				throw new ArgumentException(resource.InvalidTypeName(type), nameof(type));
			}
			Int32 typeNameLength =
				typeName.IndexOf(separator, indexof, StringComparison.InvariantCulture) is var ix2 && ix2 > 0 ?
					ix2 - 1 :
					typeName.Length;
			return String.Create(typeNameLength + 1, typeName, (s, str) =>
			{
				str.AsSpan()[..(s.Length - 1)].CopyTo(s);
				s[^1] = '\0'; // End with UTF-16 null character.
			});
		}
	}
}