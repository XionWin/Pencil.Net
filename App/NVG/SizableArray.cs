using System.Collections.Immutable;

namespace App.NVG;


internal abstract class SizableArray<T, TRAW>
{
    public TRAW[] Value { get; private set; } = new TRAW[0];

    public int Count => this.Value.Length;
    
    public void Add(T item)
    {
        var raw = ToRaw(item);
        var array = Value;
        var originalLen = array.Length;
        var addLen = raw.Length;
        Array.Resize(ref array, originalLen + addLen);
        Array.Copy(raw, 0, array, originalLen, addLen);
    }

    public abstract TRAW[] ToRaw(T item);

    public void Clear()
    {
        this.Value = new TRAW[0];
    }
}