namespace Mxrx.NetHost;

/// <summary>
/// Parameters for managed function retrieving.
/// </summary>
#if !PACKAGE
[ExcludeFromCodeCoverage]
#endif
public ref partial struct NetFunctionInfo
{
	/// <summary>
	/// Assembly path <see cref="TextParameter"/> instance.
	/// </summary>
	internal TextParameter AssemblyPath { get; private set; }
	/// <summary>
	/// Fully-qualified type name <see cref="TextParameter"/> instance.
	/// </summary>
	internal TextParameter TypeName { get; private set; }
	/// <summary>
	/// Method name.
	/// </summary>
	internal TextParameter MethodName { get; private set; }
	/// <summary>
	/// Fully-qualified delegate type.
	/// </summary>
	internal TextParameter DelegateTypeName { get; private set; }
	/// <summary>
	/// Indicates whether current method is marked as unmanaged callers only.
	/// </summary>
	internal Boolean UseUnmanagedCallersOnly { get; private set; }

	/// <summary>
	/// Initializes a builder for <see cref="NetFunctionInfo"/> instance.
	/// </summary>
	/// <returns>A <see cref="Builder"/> instance.</returns>
	public static Builder CreateBuilder() => new();

	/// <summary>
	/// Builder struct for <see cref="NetFunctionInfo"/> type.
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
			this._info.AssemblyPath = assemblyPath;
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
		/// Initializes type name.
		/// </summary>
		/// <typeparam name="T">Managed type.</typeparam>
		/// <returns>Current builder instance.</returns>
		/// <remarks>This method uses reflection.</remarks>
		public Builder WithType<T>()
#if NET9_0_OR_GREATER
			where T : allows ref struct
#endif
			=> this.WithType(typeof(T));
		/// <summary>
		/// Initializes type name.
		/// </summary>
		/// <param name="type">Managed type.</param>
		/// <returns>Current builder instance.</returns>
		/// <remarks>This method uses reflection.</remarks>
		public Builder WithType(Type type)
		{
			Builder.ThrowIfNoReflection();
			String typeName = Builder.GetAssemblyQualifiedName(type);
			return this.WithTypeName(typeName);
		}
		/// <summary>
		/// Initializes type name.
		/// </summary>
		/// <param name="typeName">Fully-qualified type name.</param>
		/// <returns>Current builder instance.</returns>
		public Builder WithTypeName(String? typeName)
		{
			ReadOnlySpan<Char> span = typeName;
			return this.WithTypeName(span);
		}
		/// <summary>
		/// Initializes type name.
		/// </summary>
		/// <param name="typeName">Fully-qualified type name.</param>
		/// <returns>Current builder instance.</returns>
		public Builder WithTypeName(CString? typeName)
		{
			ReadOnlySpan<Byte> span = typeName;
			Boolean? nullTerminated = typeName?.IsNullTerminated;
			return this.WithTypeName(span, nullTerminated.GetValueOrDefault());
		}
		/// <summary>
		/// Initializes type name.
		/// </summary>
		/// <param name="typeName">Fully-qualified type name.</param>
		/// <returns>Current builder instance.</returns>
		public Builder WithTypeName(ReadOnlySpan<Char> typeName)
		{
			this._info.TypeName = typeName;
			return this;
		}
		/// <summary>
		/// Initializes type name.
		/// </summary>
		/// <param name="typeName">Fully-qualified type name.</param>
		/// <returns>Current builder instance.</returns>
		public Builder WithTypeName(ReadOnlySpan<Byte> typeName)
			=> this.WithTypeName(typeName, TextParameter.IsLiteral(typeName));
		/// <summary>
		/// Initializes method name.
		/// </summary>
		/// <param name="methodName">Method name.</param>
		/// <returns>Current builder instance.</returns>
		public Builder WithMethodName(String? methodName)
		{
			ReadOnlySpan<Char> span = methodName;
			return this.WithMethodName(span);
		}
		/// <summary>
		/// Initializes method name.
		/// </summary>
		/// <param name="methodName">Method name.</param>
		/// <returns>Current builder instance.</returns>
		public Builder WithMethodName(CString? methodName)
		{
			ReadOnlySpan<Byte> span = methodName;
			Boolean? nullTerminated = methodName?.IsNullTerminated;
			return this.WithMethodName(span, nullTerminated.GetValueOrDefault());
		}
		/// <summary>
		/// Initializes method name.
		/// </summary>
		/// <param name="methodName">Method name.</param>
		/// <returns>Current builder instance.</returns>
		public Builder WithMethodName(ReadOnlySpan<Char> methodName)
		{
			this._info.MethodName = methodName;
			return this;
		}
		/// <summary>
		/// Initializes method name.
		/// </summary>
		/// <param name="methodName">Method name.</param>
		/// <returns>Current builder instance.</returns>
		public Builder WithMethodName(ReadOnlySpan<Byte> methodName)
			=> this.WithMethodName(methodName, TextParameter.IsLiteral(methodName));
		/// <summary>
		/// Initializes delegate type name.
		/// </summary>
		/// <typeparam name="TDelegate">Delegate type.</typeparam>
		/// <returns>Current builder instance.</returns>
		/// <remarks>This method uses reflection.</remarks>
		public Builder WithDelegateType<TDelegate>() where TDelegate : Delegate
			=> this.WithDelegateType(typeof(TDelegate));
		/// <summary>
		/// Initializes delegate type name.
		/// </summary>
		/// <param name="delegateType">Delegate type.</param>
		/// <returns>Current builder instance.</returns>
		/// <remarks>This method uses reflection.</remarks>
		public Builder WithDelegateType(Type delegateType)
		{
			Builder.ThrowIfNoReflection();
			Builder.ThrowIfNoDelegate(delegateType);
			String delegateTypeName = Builder.GetAssemblyQualifiedName(delegateType);
			return this.WithTypeName(delegateTypeName);
		}
		/// <summary>
		/// Initializes delegate type name.
		/// </summary>
		/// <param name="delegateTypeName">Delegate type name.</param>
		/// <returns>Current builder instance.</returns>
		public Builder WithDelegateTypeName(String? delegateTypeName)
		{
			ReadOnlySpan<Char> span = delegateTypeName;
			return this.WithDelegateTypeName(span);
		}
		/// <summary>
		/// Initializes delegate type name.
		/// </summary>
		/// <param name="delegateTypeName">Delegate type name.</param>
		/// <returns>Current builder instance.</returns>
		public Builder WithDelegateTypeName(CString? delegateTypeName)
		{
			ReadOnlySpan<Byte> span = delegateTypeName;
			Boolean? nullTerminated = delegateTypeName?.IsNullTerminated;
			return this.WithDelegateTypeName(span, nullTerminated.GetValueOrDefault());
		}
		/// <summary>
		/// Initializes delegate type name.
		/// </summary>
		/// <param name="delegateTypeName">Delegate type name.</param>
		/// <returns>Current builder instance.</returns>
		public Builder WithDelegateTypeName(ReadOnlySpan<Char> delegateTypeName)
		{
			this._info.DelegateTypeName = delegateTypeName;
			this._info.UseUnmanagedCallersOnly = false;
			return this;
		}
		/// <summary>
		/// Initializes delegate type name.
		/// </summary>
		/// <param name="delegateTypeName">Delegate type name.</param>
		/// <returns>Current builder instance.</returns>
		public Builder WithDelegateTypeName(ReadOnlySpan<Byte> delegateTypeName)
			=> this.WithDelegateTypeName(delegateTypeName, TextParameter.IsLiteral(delegateTypeName));
		/// <summary>
		/// Initialize delegate type as unmanaged caller only.
		/// </summary>
		/// <param name="value">
		/// Indicates whether method is marked with <see cref="UnmanagedCallersOnlyAttribute"/> attribute.
		/// </param>
		/// <returns>Current builder instance.</returns>
		public Builder WithUnmanagedCallerOnly(Boolean value)
		{
			this._info.DelegateTypeName = default;
			this._info.UseUnmanagedCallersOnly = value;
			return this;
		}
		/// <summary>
		/// Builds initialization information instance.
		/// </summary>
		/// <returns>Created <see cref="NetFunctionInfo"/> instance.</returns>
		public NetFunctionInfo Build()
		{
			if (!this._info.TypeName.IsEmpty && !this._info.MethodName.IsEmpty) return this._info;
			IMessageResource resource = IMessageResource.GetInstance();
			throw new ArgumentException(resource.MissingMethodName);
		}
	}
}