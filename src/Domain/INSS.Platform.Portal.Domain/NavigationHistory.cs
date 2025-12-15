namespace INSS.Platform.Portal.Domain;

public class NavigationHistory
{
    private readonly List<NavigationItem> _history = [];

    public int Count => _history.Count;
    
    public NavigationItem? Peek()
    {
        return _history.Count > 0 ? _history[^1] : null;
    }
    
    public bool IsLastEntry(string path)
    {
        return _history.Count > 0 && _history[^1].PageUrl == path;
    }

    public void Push(NavigationItem item)
    {
        if (_history.Count == 0 || (_history.Count > 0 && _history[^1] != item))
        {
            _history.Add(item);
        }
    }

    public NavigationItem? Pop()
    {
        NavigationItem? item = null;
        
        if (_history.Count > 0)
        {
            item = _history[^1];
            _history.Remove(item);
        }

        return item;
    }

    public void Clear()
    {
        _history.Clear();
    }
}