namespace Loupedeck.MapsPlugin.Helpers;

// Least-recently-used cache of tile images, as the OpenStreetMap tile policy asks clients not to refetch tiles.
public sealed class TileCache
{
    private readonly Object _lock = new();
    private readonly Int32 _capacity;
    private readonly Dictionary<TileKey, LinkedListNode<KeyValuePair<TileKey, Byte[]>>> _entries = new();
    private readonly LinkedList<KeyValuePair<TileKey, Byte[]>> _usageOrder = new();

    public TileCache(Int32 capacity)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(capacity);
        this._capacity = capacity;
    }

    public Int32 Count
    {
        get
        {
            lock (this._lock)
            {
                return this._entries.Count;
            }
        }
    }

    public Boolean TryGet(TileKey key, out Byte[] tile)
    {
        lock (this._lock)
        {
            tile = null;
            if (!this._entries.TryGetValue(key, out var node))
            {
                return false;
            }

            this.MarkAsRecent(node);
            tile = node.Value.Value;
            return true;
        }
    }

    public void Add(TileKey key, Byte[] tile)
    {
        ArgumentNullException.ThrowIfNull(tile);
        lock (this._lock)
        {
            if (this._entries.Remove(key, out var existing))
            {
                this._usageOrder.Remove(existing);
            }

            this._entries[key] = this._usageOrder.AddFirst(new KeyValuePair<TileKey, Byte[]>(key, tile));
            this.EvictOverflow();
        }
    }

    private void MarkAsRecent(LinkedListNode<KeyValuePair<TileKey, Byte[]>> node)
    {
        this._usageOrder.Remove(node);
        this._usageOrder.AddFirst(node);
    }

    private void EvictOverflow()
    {
        while (this._entries.Count > this._capacity)
        {
            var oldest = this._usageOrder.Last;
            this._usageOrder.RemoveLast();
            this._entries.Remove(oldest.Value.Key);
        }
    }
}
