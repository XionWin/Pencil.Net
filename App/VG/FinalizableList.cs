namespace App.VG;

public class LockableList<T>
{
    private List<T> _values = new List<T>();

    private T[]? _valuesCache = null;
    private T[] _lockedValues => this._valuesCache is null ? this._valuesCache = this._values.ToArray() : this._valuesCache;
    public IEnumerable<T> Values => IsLocked ? this._values : this._lockedValues;
    public int Count => this._values.Count();
    public bool IsLocked { get; internal set; }

    public void Add(T vlaue)
    {
        if(this.IsLocked)
        {
            throw new Exception("List is locked");
        }
        this._values.Add(vlaue);
        this._valuesCache = null;
    }

    public void Clear()
    {
        if(this.IsLocked)
        {
            throw new Exception("List is locked");
        }
        this._values.Clear();
        this._valuesCache = null;
    }

    public void Reverse()
    {
        this._values.Reverse();
        this._valuesCache = null;
    }

    public void FinalizeList()
    {
        this.IsLocked = true;
        this._values.ToArray();
    }
}