namespace App.VG;

public class FinalizableList<T>
{
    private List<T> _values = new List<T>();

    private T[]? _valueCache = null;
    public T[] Values => _valueCache is null ? this._values.ToArray() : this._valueCache;
    public int Count => this._values.Count();
    public bool IsFinalized { get; internal set; }

    public void Add(T vlaue)
    {
        if(this.IsFinalized)
        {
            throw new Exception("Path is finalized");
        }
        this._values.Add(vlaue);
        this._valueCache = null;
    }

    public void Clear()
    {
        if(this.IsFinalized)
        {
            throw new Exception("Path is finalized");
        }
        this._values.Clear();
        this._valueCache = null;
    }

    public void FinalizeList()
    {
        this.IsFinalized = true;
        this._values.ToArray();
    }
}