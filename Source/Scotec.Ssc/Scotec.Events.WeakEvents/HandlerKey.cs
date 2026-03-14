using System.Reflection;

namespace Scotec.Events.WeakEvents;

internal sealed class HandlerKey : IEquatable<HandlerKey>
{
    public WeakReference SourceRef { get; }
    public string EventName { get; }
    public WeakReference? HandlerTargetRef { get; }
    public MethodInfo HandlerMethod { get; }
    private readonly int _hashCode;

    public HandlerKey(object? source, string eventName, Delegate handler)
    {
        SourceRef = new WeakReference(source);
        EventName = eventName;
        HandlerTargetRef = handler.Target != null ? new WeakReference(handler.Target) : null;
        HandlerMethod = handler.Method;

        // Compute hash code once using the original references
        var sourceHash = source != null ? System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(source) : 0;
        var targetHash = handler.Target != null ? System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(handler.Target) : 0;
        var methodHash = HandlerMethod.GetHashCode();
        var eventNameHash = EventName.GetHashCode();
        _hashCode = sourceHash ^ targetHash ^ methodHash ^ eventNameHash;
    }

    public override int GetHashCode() => _hashCode;

    public bool Equals(HandlerKey? other)
    {
        if (other is null) return false;
        return ReferenceEquals(SourceRef.Target, other.SourceRef.Target)
               && ReferenceEquals(HandlerTargetRef?.Target, other.HandlerTargetRef?.Target)
               && EventName == other.EventName
               && HandlerMethod == other.HandlerMethod;
    }

    public override bool Equals(object? obj) => Equals(obj as HandlerKey);
}
