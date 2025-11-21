namespace Scotec.Events.WeakEvents;

public static class StaticWeakEventManager
{
    private static readonly WeakEventManager Instance = new();

    public static void AddWeakHandler<TObject>(TObject source, string eventName, Action<TObject, EventArgs> handler)
        where TObject : class
    {
        Instance.AddWeakHandler(source, eventName, handler);
    }

    public static void AddWeakHandler<TObject, TEventArgs>(TObject source, string eventName, Action<TObject, TEventArgs> handler)
        where TObject : class
        where TEventArgs : EventArgs
    {
        Instance.AddWeakHandler(source, eventName, handler);
    }

    public static void RemoveWeakHandler<TObject>(TObject source, string eventName, Action<TObject, EventArgs> handler)
        where TObject : class
    {
        Instance.RemoveWeakHandler(source, eventName, handler);
    }

    public static void RemoveWeakHandler<TObject, TEventArgs>(TObject source, string eventName, Action<TObject, TEventArgs> handler)
        where TObject : class
        where TEventArgs : EventArgs
    {
        Instance.RemoveWeakHandler(source, eventName, handler);
    }

    public static void CleanupDeadHandlers()
    {
        Instance.CleanupDeadHandlers();
    }
}
