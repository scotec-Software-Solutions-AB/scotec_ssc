#region

using System.ComponentModel;
using System.Runtime.CompilerServices;

#endregion


namespace Scotec.Events.WeakEvents;

#region delegates

public delegate void EventHandlerOfTUnregisterCallback<TEventArgs>(EventHandler<TEventArgs> eventHandler)
    where TEventArgs : EventArgs;

public delegate void EventHandlerUnregisterCallback(EventHandler eventHandler);

public delegate void PropertyChangedEventHandlerUnregisterCallback(PropertyChangedEventHandler eventHandler);

#endregion delegates

#region interfaces

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

#endregion interfaces

#region WeakEventHandler<T>

public class WeakEventHandler<T> : IWeakEventHandler, IDisposable
    where T : class
{
    private EventHandler _handler;
    private OpenEventHandler _openHandler;
    private WeakReference _targetRef;
    private EventHandlerUnregisterCallback _unregister;

    public WeakEventHandler(EventHandler eventHandler, EventHandlerUnregisterCallback unregister)
    {
        if (eventHandler == null)
            throw new ArgumentNullException(nameof(eventHandler));

        _targetRef = new WeakReference(eventHandler);
        _openHandler =
            (OpenEventHandler)Delegate.CreateDelegate(typeof(OpenEventHandler), null, eventHandler.Method);
        _handler = Invoke;
        _unregister = unregister;
    }

    public EventHandler Handler => Invoke;


    #region IDisposable Members

    public void Dispose()
    {
        UnregisterHandler();
    }

    #endregion


    #region IWeakEventHandler Members

    EventHandler IWeakEventHandler.Handler => _handler;

    #endregion


    private void Invoke(object sender, EventArgs e)
    {
        var subject = (T)_targetRef.Target;

        if (subject != null)
            _openHandler(subject, RuntimeHelpers.GetObjectValue(sender), e);
        else
            UnregisterHandler();
    }

    public static implicit operator EventHandler(WeakEventHandler<T> weakEventHandler)
    {
        return weakEventHandler?._handler;
    }

    private void UnregisterHandler()
    {
        if (_unregister != null)
        {
            _unregister(_handler);
            _unregister = null;
        }

        _targetRef = null;
        _openHandler = null;
        _handler = null;
    }

    public EventHandler ToEventHandler()
    {
        return _handler;
    }


    #region Nested type: OpenEventHandler

    private delegate void OpenEventHandler(T subject, object sender, EventArgs e);

    #endregion
}

#endregion //WeakEventHandler<T>

#region WeakEventHandler<T, TEventArgs>

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
            throw new ArgumentNullException(nameof(eventHandler));

        _targetRef = new WeakReference(RuntimeHelpers.GetObjectValue(eventHandler.Target));
        _openHandler =
            (OpenEventHandler)Delegate.CreateDelegate(typeof(OpenEventHandler), null, eventHandler.Method);
        Handler = Invoke;
        _unregister = unregister;
    }


    #region IDisposable Members

    public void Dispose()
    {
        UnregisterHandler();
    }

    #endregion


    #region IWeakEventHandler<TEventArgs> Members

    public EventHandler<TEventArgs> Handler { get; private set; }

    #endregion


    private void Invoke(object sender, TEventArgs e)
    {
        var subject = (T)_targetRef.Target;

        if (subject != null)
            _openHandler(subject, RuntimeHelpers.GetObjectValue(sender), e);
        else
            UnregisterHandler();
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


    #region Nested type: OpenEventHandler

    private delegate void OpenEventHandler(T subject, object sender, TEventArgs e);

    #endregion
}

#endregion //WeakEventHandler<T, TEventArgs>

#region WeakPropertyChangedEventHandler<T>

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
            throw new ArgumentNullException(nameof(eventHandler));

        _targetRef = new WeakReference(RuntimeHelpers.GetObjectValue(eventHandler.Target));
        _openHandler =
            (OpenEventHandler)Delegate.CreateDelegate(typeof(OpenEventHandler), null, eventHandler.Method);
        Handler = Invoke;
        _unregister = unregister;
    }

    public PropertyChangedEventHandler Handler { get; private set; }


    #region IDisposable Members

    public void Dispose()
    {
        UnregisterHandler();
    }

    #endregion


    #region IWeakPropertyChangedEventHandler Members

    PropertyChangedEventHandler IWeakPropertyChangedEventHandler.Handler => Handler;

    #endregion


    private void Invoke(object sender, PropertyChangedEventArgs e)
    {
        var subject = (T)_targetRef.Target;

        if (subject != null)
            _openHandler(subject, RuntimeHelpers.GetObjectValue(sender), e);
        else
            UnregisterHandler();
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


    #region Nested type: OpenEventHandler

    private delegate void OpenEventHandler(T subject, object sender, PropertyChangedEventArgs e);

    #endregion
}

#endregion //WeakPropertyChangedEventHandler<T>

public static class EventHandlerUtils
{
    public static void AddWeak(this INotifyPropertyChanged obj, PropertyChangedEventHandler handler)
    {
        obj.PropertyChanged += handler.MakeWeak(eh => obj.PropertyChanged -= handler);
    }

    public static PropertyChangedEventHandler MakeWeak(this PropertyChangedEventHandler eventHandler,
        PropertyChangedEventHandlerUnregisterCallback unregister)
    {
        if (eventHandler == null)
            throw new ArgumentNullException(nameof(eventHandler));

        if (eventHandler.Method.IsStatic || eventHandler.Target == null)
            throw new ArgumentException(@"Only instance methods are supported.", nameof(eventHandler));

        var constructorInfo =
            typeof(WeakPropertyChangedEventHandler<>).MakeGenericType(eventHandler.Method.DeclaringType)
                .GetConstructor(new[]
                {
                    typeof(PropertyChangedEventHandler),
                    typeof(PropertyChangedEventHandlerUnregisterCallback)
                });

        if (constructorInfo == null)
            throw new Exception("Could not create weak event handler.");

        return ((IWeakPropertyChangedEventHandler)constructorInfo.Invoke(new object[] { eventHandler, unregister }))
            .Handler;
    }

    public static EventHandler<TEventArgs> MakeWeak<TEventArgs>(this EventHandler<TEventArgs> eventHandler,
        EventHandlerOfTUnregisterCallback<TEventArgs> unregister)
        where TEventArgs : EventArgs
    {
        if (eventHandler == null)
            throw new ArgumentNullException(nameof(eventHandler));

        if (eventHandler.Method.IsStatic || eventHandler.Target == null)
            // ReSharper disable once LocalizableElement
            throw new ArgumentException(@"Only instance methods are supported.", nameof(eventHandler));

        var constructorInfo =
            typeof(WeakEventHandler<,>).MakeGenericType(eventHandler.Method.DeclaringType, typeof(TEventArgs))
                .GetConstructor(new[]
                {
                    typeof(EventHandler<TEventArgs>),
                    typeof(EventHandlerOfTUnregisterCallback<TEventArgs>)
                });

        if (constructorInfo == null)
            throw new Exception("Could not create weak event handler.");

        return ((IWeakEventHandler<TEventArgs>)constructorInfo.Invoke(new object[] { eventHandler, unregister }))
            .Handler;
    }

    public static EventHandler MakeWeak(this EventHandler eventHandler, EventHandlerUnregisterCallback unregister)
    {
        if (eventHandler == null)
            throw new ArgumentNullException(nameof(eventHandler));

        if (eventHandler.Method.IsStatic || eventHandler.Target == null)
            // ReSharper disable once LocalizableElement
            throw new ArgumentException(@"Only instance methods are supported.", nameof(eventHandler));

        var constructorInfo =
            typeof(WeakEventHandler<>).MakeGenericType(eventHandler.Method.DeclaringType)
                .GetConstructor(new[] { typeof(EventHandler), typeof(EventHandlerUnregisterCallback) });

        if (constructorInfo == null)
            throw new Exception("Could not create weak event handler.");

        return ((IWeakEventHandler)constructorInfo.Invoke(new object[] { eventHandler, unregister })).Handler;
    }
}