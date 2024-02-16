namespace App.VG;

public class Cache
{
    private Queue<Path> _paths = new Queue<Path>();
    public Rect Bounds { get; private set; } = new Rect();

    public void AddPath(Path path) => this._paths.Enqueue(path);
    public Path? GetPath() => this._paths.TryPeek(out var path) ? path : null;
    
    public void Clear()
    {
        this._paths.Clear();
        this.Bounds = new Rect();
    }
}