#if NET9_0_OR_GREATER
using IEnumerator = System.Collections.IEnumerator;
#endif

namespace Mxrx.NetHost;

/// <summary>
/// Property key/value pair collection.
/// </summary>
public readonly ref partial struct RuntimePropertyCollection
{
	/// <summary>
	/// A <see cref="HostContext"/> instance.
	/// </summary>
	private readonly HostContext _hostContext;
	/// <summary>
	/// Property keys buffer.
	/// </summary>
	private readonly ReadOnlySpan<IntPtr> _keys;
	/// <summary>
	/// Property values buffer.
	/// </summary>
	private readonly ReadOnlySpan<IntPtr> _values;

	/// <summary>
	/// The number of properties in the current collection.
	/// </summary>
	public Int32 Count => this._keys.Length;

	/// <summary>
	/// The <see cref="RuntimePropertyPair"/> for <paramref name="index"/>.
	/// </summary>
	/// <param name="index">Property index.</param>
	public RuntimePropertyPair this[Int32 index]
		=> new()
		{
			Key = RuntimePropertyCollection.getText(this._hostContext, this._keys[index]),
			Value = RuntimePropertyCollection.getText(this._hostContext, this._values[index]),
		};

	/// <summary>
	/// Internal constructor.
	/// </summary>
	/// <param name="hostContext">A <see cref="HostContext"/> instance.</param>
	/// <param name="keys">Property keys buffer.</param>
	/// <param name="values">Property values buffer.</param>
	internal RuntimePropertyCollection(HostContext hostContext, ReadOnlySpan<IntPtr> keys, ReadOnlySpan<IntPtr> values)
	{
		this._hostContext = hostContext;
		this._keys = keys;
		this._values = values;
	}

	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	public Enumerator GetEnumerator() => new(this);

	/// <summary>
	/// Enumerates the <see cref="RuntimePropertyPair"/> instaces within a <see cref="RuntimePropertyCollection"/>.
	/// </summary>
	public ref struct Enumerator
#if NET9_0_OR_GREATER
		: IEnumerator<RuntimePropertyPair>
#endif
	{
		/// <summary>
		/// Current <see cref="RuntimePropertyCollection"/> instance.
		/// </summary>
		private readonly RuntimePropertyCollection _instance;

		/// <summary>
		/// Current index.
		/// </summary>
		private Int32 _index;

		/// <summary>
		/// Gets the element in the sequence at the current position of the enumerator.
		/// </summary>
		public RuntimePropertyPair Current => this._instance[this._index];

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="instance">A <see cref="RuntimePropertyCollection"/> instance.</param>
		internal Enumerator(RuntimePropertyCollection instance)
		{
			this._instance = instance;
			this.Reset();
		}

#if NET9_0_OR_GREATER
		Object IEnumerator.Current => this._instance[this._index].ToString();

		void IDisposable.Dispose() { }
#endif
		/// <summary>
		/// Advances the enumerator to the next element of the enumeration.
		/// </summary>
		/// <returns>
		/// <see langword="true"/> if the enumerator was successfully advanced to the next element;
		/// <see langword="false"/> if the enumerator has passed the end of the enumeration.
		/// </returns>
		public Boolean MoveNext()
		{
			if (this._index >= this._instance.Count) return false;
			this._index++;
			return true;
		}
		/// <summary>
		/// Resets the enumerator to the beginning of the enumeration, starting over.
		/// </summary>
		public void Reset() => this._index = -1;
	}
}