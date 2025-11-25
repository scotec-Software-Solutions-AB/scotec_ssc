using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace Scotec.Events.WeakEvents;

public class WeakEventManager : IDisposable
{
    private readonly ConcurrentDictionary<HandlerKey, List<Delegate>> _handlerDelegates = new();

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


    public void AddWeakHandler<TObject, TEventArgs, TEventHandler>(TObject source, string eventName, Action<TObject, TEventArgs> handler)
        where TObject : class
        where TEventArgs : EventArgs
        where TEventHandler : Delegate
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

        var handlerDelegate = new EventHandler<TEventArgs>((s, e) =>
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
        });

        
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
#if NETSTANDARD2_1_OR_GREATER
                var handlerDelegate = list[^1];
#else
                var handlerDelegate = list.Last();
#endif
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

    public void RemoveWeakHandler<TObject>(TObject source, string eventName, Action<TObject, EventArgs> handler)
        where TObject : class
    {
        RemoveWeakHandler<TObject, EventArgs>(source, eventName, handler);
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
                    {
                        return false; // static method, always alive
                    }

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
}
