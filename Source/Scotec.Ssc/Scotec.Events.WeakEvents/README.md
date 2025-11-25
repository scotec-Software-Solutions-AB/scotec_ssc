# WeakEventManager

The `WeakEventManager` class in the `Scotec.Events.WeakEvents` namespace provides a thread-safe mechanism for managing event subscriptions using weak references. This helps prevent memory leaks that can occur when event handlers are strongly referenced, especially in scenarios where publishers outlive their subscribers.

## Purpose

In .NET, traditional event subscriptions create strong references between the event source and the event handler. If the subscriber is not explicitly unsubscribed, the event source will keep the subscriber alive, potentially causing memory leaks. `WeakEventManager` solves this by storing event handlers as weak references, allowing the garbage collector to reclaim subscribers when they are no longer in use.

## Features

- Add weak event handlers for any event on any object.
- Remove weak event handlers manually or automatically when the target is collected.
- Automatic cleanup of dead handlers.
- Thread-safe management of event handler delegates.
- Disposable remover for easy unsubscription.
- Static API for global weak event management.

## Usage

### 1. Adding a Weak Event Handler (Instance)

Subscribe to an event using a weak reference:

```csharp
var manager = new WeakEventManager(); 
IDisposable remover = manager.AddWeakHandler<MySource, MyEventArgs>( sourceObject, "MyEvent", (src, args) => { /* handle event */ } );
```

- `sourceObject`: The object that publishes the event.
- `"MyEvent"`: The name of the event to subscribe to.
- Handler: A lambda or method matching `(src, args)`.


### 2. Removing a Weak Event Handler

You can remove a handler using the returned `IDisposable`:

```csharp
remover.Dispose();
```

Or, remove it directly:

```csharp
manager.RemoveWeakHandler<MySource, MyEventArgs>( sourceObject, "MyEvent", handler );
```


### 3. Cleaning Up Dead Handlers

To remove handlers whose targets have been garbage collected:

```csharp
manager.Cleanup();
```

This can be useful to call periodically, for example, in an `Application.Deactivated` event or similar global pause event, to ensure that any unused handlers are removed, and memory can be reclaimed.

```csharp
manager.CleanupDeadHandlers();
```


### 4. Disposing the Manager

Dispose the manager to remove all handlers and clean up resources:

```csharp
manager.Dispose();
```


### 5. Static Usage

For global event management, use the static API:

```csharp
WeakEventManager.AddWeakHandler<MySource, MyEventArgs>( sourceObject, "MyEvent", (src, args) => { /* handle event */ } );
```

## API Reference

- `AddWeakHandler<TObject, TEventArgs>(TObject source, string eventName, Action<TObject, TEventArgs> handler)`: Registers a weak event handler and returns an `IDisposable` for manual removal.
- `RemoveWeakHandler<TObject, TEventArgs>(TObject source, string eventName, Action<TObject, TEventArgs> handler)`: Removes a previously registered weak event handler.
- `CleanupDeadHandlers()`: Removes handlers whose targets have been garbage collected.
- `Dispose()`: Removes all handlers and releases resources.

## Example

```csharp
public class MySource 
{ 
    public event EventHandler<MyEventArgs> MyEvent; 
}

public class MySubscriber 
{ 
    public void Subscribe(MySource source, WeakEventManager manager) 
    { 
        manager.AddWeakHandler<MySource, MyEventArgs>( source, nameof(source.MyEvent), (src, args) => HandleEvent(args) ); 
    }
    private void HandleEvent(MySource source, MyEventArgs args)
    {
        // Handle event logic
    }
}
```

## When to Use

- To avoid memory leaks from event subscriptions.
- In MVVM, UI, or long-lived publisher scenarios.
- When subscribers may be short-lived or dynamically created.

## Notes

- The event name must match the actual event on the source object.
- Only events with compatible delegate signatures are supported.
- The manager is thread-safe.
- Compatible with .NET Standard 2.0 and 2.1.

## License

See [LICENSE](LICENSE) for details.

---

For more information, see the source code in `Scotec.Events.WeakEvents\WeakEventManager.cs`




