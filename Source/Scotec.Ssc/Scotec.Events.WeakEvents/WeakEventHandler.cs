using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Scotec.Events.WeakEvents;

public delegate void EventHandlerOfTUnregisterCallback<TEventArgs>(EventHandler<TEventArgs> eventHandler)
    where TEventArgs : EventArgs;

public delegate void EventHandlerUnregisterCallback(EventHandler eventHandler);

public delegate void PropertyChangedEventHandlerUnregisterCallback(PropertyChangedEventHandler eventHandler);

public interface IWeakEventHandler
{
    EventHandler Handler { get; }
}

public interface IWeakEventHandler<TEventArgs>
    where TEventArgs : EventArgs
{
    EventHandler<TEventArgs> Handler { get; }
}

public interface IWeakPropertyChangedEventHandler
{
    PropertyChangedEventHandler Handler { get; }
}

public class WeakEventHandler<T> : IWeakEventHandler, IDisposable
    where T : class
{
    private OpenEventHandler _openHandler;
    private WeakReference _targetRef;
    private EventHandlerUnregisterCallback _unregister;

    public WeakEventHandler(EventHandler eventHandler, EventHandlerUnregisterCallback unregister)
    {
        if (eventHandler == null)
        {
            throw new ArgumentNullException(nameof(eventHandler));
        }

        _targetRef = new WeakReference(eventHandler);
        _openHandler =
            (OpenEventHandler)Delegate.CreateDelegate(typeof(OpenEventHandler), null, eventHandler.Method);
        Handler = Invoke;
        _unregister = unregister;
    }

    public EventHandler Handler { get; private set; }

    public void Dispose()
    {
        UnregisterHandler();
    }

    EventHandler IWeakEventHandler.Handler => Handler;

    private void Invoke(object sender, EventArgs e)
    {
        if (_targetRef is null)
        {
            return;
        }
        
        var subject = (T)_targetRef.Target;

        if (subject != null)
        {
            _openHandler(subject, RuntimeHelpers.GetObjectValue(sender), e);
        }
        else
        {
            UnregisterHandler();
        }
    }

    public static implicit operator EventHandler(WeakEventHandler<T> weakEventHandler)
    {
        return weakEventHandler?.Handler;
    }

    private void UnregisterHandler()
    {
        if (_unregister != null)
        {
            _unregister(Handler);
            _unregister = null;
        }

        _targetRef = null;
        _openHandler = null;
        Handler = null;
    }

    public EventHandler ToEventHandler()
    {
        return Handler;
    }

    private delegate void OpenEventHandler(T subject, object sender, EventArgs e);
}

public class WeakEventHandler<T, TEventArgs> : IWeakEventHandler<TEventArgs>, IDisposable
    where TEventArgs : EventArgs
    where T : class
{
    private OpenEventHandler _openHandler;
    private WeakReference _targetRef;
    private EventHandlerOfTUnregisterCallback<TEventArgs> _unregister;

    public WeakEventHandler(EventHandler<TEventArgs> eventHandler,
                            EventHandlerOfTUnregisterCallback<TEventArgs> unregister)
    {
        if (eventHandler == null)
        {
            throw new ArgumentNullException(nameof(eventHandler));
        }

        _targetRef = new WeakReference(RuntimeHelpers.GetObjectValue(eventHandler.Target));
        _openHandler =
            (OpenEventHandler)Delegate.CreateDelegate(typeof(OpenEventHandler), null, eventHandler.Method);
        Handler = Invoke;
        _unregister = unregister;
    }

    public void Dispose()
    {
        UnregisterHandler();
    }

    public EventHandler<TEventArgs> Handler { get; private set; }

    private void Invoke(object sender, TEventArgs e)
    {
        if (_targetRef is null)
        {
            return;
        }

        var subject = (T)_targetRef.Target;

        if (subject != null)
        {
            _openHandler(subject, RuntimeHelpers.GetObjectValue(sender), e);
        }
        else
        {
            UnregisterHandler();
        }
    }

    public static implicit operator EventHandler<TEventArgs>(WeakEventHandler<T, TEventArgs> weakEventHandler)
    {
        return weakEventHandler?.Handler;
    }

    private void UnregisterHandler()
    {
        if (_unregister != null)
        {
            _unregister(Handler);
            _unregister = null;
        }

        _targetRef = null;
        _openHandler = null;
        Handler = null;
    }

    public EventHandler<TEventArgs> ToEventHandler()
    {
        return Handler;
    }

    private delegate void OpenEventHandler(T subject, object sender, TEventArgs e);
}

public class WeakPropertyChangedEventHandler<T> : IWeakPropertyChangedEventHandler, IDisposable
    where T : class
{
    private OpenEventHandler _openHandler;
    private WeakReference _targetRef;
    private PropertyChangedEventHandlerUnregisterCallback _unregister;

    public WeakPropertyChangedEventHandler(PropertyChangedEventHandler eventHandler,
                                           PropertyChangedEventHandlerUnregisterCallback unregister)
    {
        if (eventHandler == null)
        {
            throw new ArgumentNullException(nameof(eventHandler));
        }

        _targetRef = new WeakReference(RuntimeHelpers.GetObjectValue(eventHandler.Target));
        _openHandler =
            (OpenEventHandler)Delegate.CreateDelegate(typeof(OpenEventHandler), null, eventHandler.Method);
        Handler = Invoke;
        _unregister = unregister;
    }

    public PropertyChangedEventHandler Handler { get; private set; }

    public void Dispose()
    {
        UnregisterHandler();
    }

    PropertyChangedEventHandler IWeakPropertyChangedEventHandler.Handler => Handler;

    private void Invoke(object sender, PropertyChangedEventArgs e)
    {
        if (_targetRef is null)
        {
            return;
        }

        var subject = (T)_targetRef.Target;

        if (subject != null)
        {
            _openHandler(subject, RuntimeHelpers.GetObjectValue(sender), e);
        }
        else
        {
            UnregisterHandler();
        }
    }

    public static implicit operator PropertyChangedEventHandler(WeakPropertyChangedEventHandler<T> weakEventHandler)
    {
        return weakEventHandler?.Handler;
    }

    private void UnregisterHandler()
    {
        if (_unregister != null)
        {
            _unregister(Handler);
            _unregister = null;
        }

        _targetRef = null;
        _openHandler = null;
        Handler = null;
    }

    public PropertyChangedEventHandler ToPropertyChangedEventHandler()
    {
        return Handler;
    }

    private delegate void OpenEventHandler(T subject, object sender, PropertyChangedEventArgs e);
}

public static class EventHandlerUtils
{
    public static void AddWeak(this INotifyPropertyChanged obj, PropertyChangedEventHandler handler)
    {
        obj.PropertyChanged += handler.MakeWeak(eh => obj.PropertyChanged -= eh);
    }

    public static PropertyChangedEventHandler MakeWeak(this PropertyChangedEventHandler eventHandler,
                                                       PropertyChangedEventHandlerUnregisterCallback unregister)
    {
        if (eventHandler == null)
        {
            throw new ArgumentNullException(nameof(eventHandler));
        }

        if (eventHandler.Method.IsStatic || eventHandler.Target == null)
        {
            throw new ArgumentException("Only instance methods are supported.", nameof(eventHandler));
        }

        var constructorInfo =
            typeof(WeakPropertyChangedEventHandler<>).MakeGenericType(eventHandler.Method.DeclaringType)
                                                     .GetConstructor([
                                                         typeof(PropertyChangedEventHandler),
                                                         typeof(PropertyChangedEventHandlerUnregisterCallback)
                                                     ]);

        if (constructorInfo == null)
        {
            throw new Exception("Could not create weak event handler.");
        }

        return ((IWeakPropertyChangedEventHandler)constructorInfo.Invoke([eventHandler, unregister]))
            .Handler;
    }

    public static EventHandler<TEventArgs> MakeWeak<TEventArgs>(this EventHandler<TEventArgs> eventHandler,
                                                                EventHandlerOfTUnregisterCallback<TEventArgs> unregister)
        where TEventArgs : EventArgs
    {
        if (eventHandler == null)
        {
            throw new ArgumentNullException(nameof(eventHandler));
        }

        if (eventHandler.Method.IsStatic || eventHandler.Target == null)
            // ReSharper disable once LocalizableElement
        {
            throw new ArgumentException("Only instance methods are supported.", nameof(eventHandler));
        }

        var constructorInfo =
            typeof(WeakEventHandler<,>).MakeGenericType(eventHandler.Method.DeclaringType, typeof(TEventArgs))
                                       .GetConstructor([
                                           typeof(EventHandler<TEventArgs>),
                                           typeof(EventHandlerOfTUnregisterCallback<TEventArgs>)
                                       ]);

        if (constructorInfo == null)
        {
            throw new Exception("Could not create weak event handler.");
        }

        return ((IWeakEventHandler<TEventArgs>)constructorInfo.Invoke([eventHandler, unregister]))
            .Handler;
    }

    public static EventHandler MakeWeak(this EventHandler eventHandler, EventHandlerUnregisterCallback unregister)
    {
        if (eventHandler == null)
        {
            throw new ArgumentNullException(nameof(eventHandler));
        }

        if (eventHandler.Method.IsStatic || eventHandler.Target == null)
            // ReSharper disable once LocalizableElement
        {
            throw new ArgumentException("Only instance methods are supported.", nameof(eventHandler));
        }

        var constructorInfo =
            typeof(WeakEventHandler<>).MakeGenericType(eventHandler.Method.DeclaringType)
                                      .GetConstructor([typeof(EventHandler), typeof(EventHandlerUnregisterCallback)]);

        if (constructorInfo == null)
        {
            throw new Exception("Could not create weak event handler.");
        }

        return ((IWeakEventHandler)constructorInfo.Invoke([eventHandler, unregister])).Handler;
    }
}
