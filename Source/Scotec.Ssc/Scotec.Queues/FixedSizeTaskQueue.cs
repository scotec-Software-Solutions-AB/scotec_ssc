namespace Scotec.Queues;

public class FixedSizeTaskQueue<TQueueable> : TaskQueue<TQueueable>
{
    private readonly int _size;

    public FixedSizeTaskQueue(Action<TQueueable> action, int size)
        : base(action)
    {
        _size = size;
    }

    public FixedSizeTaskQueue(Action<IEnumerable<TQueueable>> listAction, int size, int delay = 0)
        : base(listAction, delay)
    {
        _size = size;
    }

    protected override void OnEnqueue(Queue<TQueueable> queue, TQueueable item)
    {
        base.OnEnqueue(queue, item);

        if (queue.Count > _size)
            queue.Dequeue();
    }
}