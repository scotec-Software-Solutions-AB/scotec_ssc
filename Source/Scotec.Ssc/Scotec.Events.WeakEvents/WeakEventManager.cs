using System.Collections.Concurrent;
using System.Reflection;

namespace Scotec.Events.WeakEvents;

//public delegate void MyEventHandler<in TObject, in TEventArgs>(TObject sender, TEventArgs i)
//    where TObject : class
//    where TEventArgs : EventArgs;

public static class StaticWeakEventManager
{
    private static readonly WeakEventManager Instance = new();

    public static void AddWeakHandler<TObject, TEventArgs>(TObject source, string eventName, Action<TObject, TEventArgs> handler)
        where TObject : class
        where TEventArgs : EventArgs
    {
        Instance.AddWeakHandler(source, eventName, handler);
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


public class WeakEventManager : IDisposable
{
    private readonly ConcurrentDictionary<HandlerKey, List<Delegate>> _handlerDelegates = new();

    public void AddWeakHandler<TObject, TEventArgs>(TObject source, string eventName, Action<TObject, TEventArgs> handler)
        where TObject : class
        where TEventArgs : EventArgs
    {
        var eventInfo = source.GetType().GetEvent(eventName);
        if (eventInfo == null)
        {
            throw new ArgumentException($"Event '{eventName}' not found on type '{source.GetType()}'.");
        }

        var target = handler.Target;
        var method = handler.Method;
        var weakTarget = target != null ? new WeakReference(target) : null;
        var key = new HandlerKey(source, eventName, handler);

        Action<TObject, TEventArgs>? handlerDelegate = null!;

        void WeakHandler(TObject s, TEventArgs e)
        {
            if (weakTarget == null)
            {
                method.Invoke(null, [s, e]);
            }
            else if (weakTarget.IsAlive)
            {
                method.Invoke(weakTarget.Target, [s, e]);
            }
            else
            {
                TryRemoveHandlerDelegate(source, key, eventInfo);
            }
        }

        handlerDelegate = WeakHandler;

        eventInfo.AddEventHandler(source, handlerDelegate);

        var list = _handlerDelegates.GetOrAdd(key, _ => new List<Delegate>());

        lock (list)
        {
            list.Add(handlerDelegate);
        }
    }

    private void TryRemoveHandlerDelegate<TObject>(TObject source, HandlerKey key, EventInfo eventInfo)
        where TObject : class
    {
        if (_handlerDelegates.TryGetValue(key, out var list) && list.Count > 0)
        {
            // Remove the last registered handler
            lock (list)
            {
                var handlerDelegate = list[^1];
                list.Remove(handlerDelegate);
                eventInfo.RemoveEventHandler(source, handlerDelegate);

                // Clean up if no more delegates
                if (list.Count == 0)
                {
                    _handlerDelegates.TryRemove(key, out _);
                }
            }
        }
    }

    public void RemoveWeakHandler<TObject, TEventArgs>(TObject source, string eventName, Action<TObject, TEventArgs> handler)
        where TObject : class
        where TEventArgs : EventArgs
    {
        var eventInfo = source.GetType().GetEvent(eventName);
        if (eventInfo == null)
        {
            throw new ArgumentException($"Event '{eventName}' not found on type '{source.GetType()}'.");
        }

        var key = new HandlerKey(source, eventName, handler);
        TryRemoveHandlerDelegate(source, key, eventInfo);
    }

    public void CleanupDeadHandlers()
    {
        foreach (var kvp in _handlerDelegates)
        {
            var key = kvp.Key;
            var list = kvp.Value;

            lock (list)
            {
                // Remove dead delegates from the list
                list.RemoveAll(d =>
                {
                    // For each delegate, check if its target is dead
                    var target = d.Target;
                    if (target is null)
                        return false; // static method, always alive

                    // If target is a WeakReference, check IsAlive and if the key's HandlerTargetRef is dead
                    return key.HandlerTargetRef != null && (key.HandlerTargetRef.Target == null || !key.HandlerTargetRef.IsAlive);
                });

                // Remove the key if the list is now empty
                if (list.Count == 0)
                {
                    _handlerDelegates.TryRemove(key, out _);
                }
            }
        }
    }

    public void Dispose()
    {
        foreach (var kvp in _handlerDelegates)
        {
            var key = kvp.Key;
            var list = kvp.Value;

            lock (list)
            {
                var source = key.SourceRef.Target;
                var eventInfo = source?.GetType().GetEvent(key.EventName);

                if (eventInfo != null && source != null)
                {
                    foreach (var handler in list)
                    {
                        eventInfo.RemoveEventHandler(source, handler);
                    }
                }
            }
        }
        _handlerDelegates.Clear();
    }
}