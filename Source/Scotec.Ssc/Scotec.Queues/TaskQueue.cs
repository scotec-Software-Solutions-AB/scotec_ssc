namespace Scotec.Queues;

public class TaskQueue<TQueueable>
{
    private readonly Action<TQueueable> _action;
    private readonly int _delay;
    private readonly ManualResetEvent _emptyEvent = new(false);
    private readonly AutoResetEvent _itemEnqueuedEvent = new(false);
    private readonly Action<IEnumerable<TQueueable>> _listAction;
    private readonly Queue<TQueueable> _queue = new();
    private readonly AutoResetEvent _startedEvent = new(false);

    private readonly object _startStopSyncObject = new();
    private readonly ManualResetEvent _stopEvent = new(false);
    private readonly AutoResetEvent _stoppedEvent = new(false);
    private readonly object _syncObject = new();
    private DateTime _lastActionCalled;
    private bool _stop = true;
    private Thread _thread;

    public TaskQueue(Action<TQueueable> action, int delay = 0)
    {
        _action = action;
        _delay = delay;
        _lastActionCalled = DateTime.UtcNow;
    }

    public TaskQueue(Action<IEnumerable<TQueueable>> listAction, int delay = 0)
    {
        _listAction = listAction;
        _delay = delay;
        _lastActionCalled = DateTime.UtcNow;
    }


    public bool IsRunning { get; private set; }

    public void Clear()
    {
        lock (_syncObject)
        {
            _queue.Clear();
        }

        _itemEnqueuedEvent.Set();
    }

    public void Enqueue(TQueueable item)
    {
        lock (_syncObject)
        {
            if (_stop)
                return;

            _emptyEvent.Reset();

            OnEnqueue(_queue, item);
        }

        _itemEnqueuedEvent.Set();
    }

    public void Start(ThreadPriority priority = ThreadPriority.Normal)
    {
        lock (_startStopSyncObject)
        {
            _stop = false;

            if (IsRunning)
                return;

            lock (_syncObject)
            {
                // If items have been enqueued before the queue has been started the emptyEvent must be reset. Otherwise it must be set.
                if (_queue.Count == 0)
                    _emptyEvent.Set();
                else
                    _emptyEvent.Reset();
            }

            _itemEnqueuedEvent.Reset();
            _stopEvent.Reset();

            _thread = new Thread(Run) { Priority = priority };
            _thread.Start();

            _startedEvent.WaitOne(-1);
        }
    }

    public void Stop()
    {
        lock (_startStopSyncObject)
        {
            if (_stop)
                return;

            lock (_syncObject)
            {
                _stop = true;
                _stopEvent.Set();
            }

            _itemEnqueuedEvent.Set();
            _stoppedEvent.WaitOne();
        }
    }

    public void Wait(int timeout = -1, int delay = 0)
    {
        if (delay > 0)
            Thread.Sleep(delay);

        _stopEvent.Set();
        _itemEnqueuedEvent.Set();

        // ReSharper disable once InconsistentlySynchronizedField
        _emptyEvent.WaitOne(timeout);
    }

    protected virtual void OnEnqueue(Queue<TQueueable> queue, TQueueable item)
    {
        queue.Enqueue(item);
    }

    private void DelayProcess()
    {
        if (_delay > 0)
        {
            var timeElapsed = (int)(DateTime.UtcNow - _lastActionCalled).TotalMilliseconds;

            if (timeElapsed < _delay) _stopEvent.WaitOne(_delay - timeElapsed);
        }
    }

    private IEnumerable<TQueueable> DequeueAll()
    {
#if TRUE
            // Copy all objects to a buffer at once.
            // Performance might be better. 
            lock (_syncObject)
            {
                var count = _queue.Count;
                var queueables = new TQueueable[count];

                _queue.CopyTo(queueables, 0);
                _queue.Clear();
                return queueables;
            }
#else

        // Dequeue all objects
        var list = new List<TQueueable>();

        lock (_syncObject)
        {
            while (_queue.Count > 0)
                list.Add(_queue.Dequeue());

            _emptyEvent.Set();
        }

        return list;
#endif
    }

    private TQueueable DequeueItem()
    {
        lock (_syncObject)
        {
            var queueable = _queue.Dequeue();

            return queueable;
        }
    }

    private void ProcessAllItems()
    {
        DelayProcess();

        IEnumerable<TQueueable> items;

        lock (_syncObject)
        {
            items = DequeueAll();
        }

        _listAction?.Invoke(items);

        lock (_syncObject)
        {
            if (_queue.Count == 0)
                _emptyEvent.Set();
        }

        ResetDelay();
    }

    private void ProcessNextItem()
    {
        while (!_stop)
        {
            DelayProcess();

            TQueueable item;

            lock (_syncObject)
            {
                if (_stop || _queue.Count == 0)
                    break;

                item = DequeueItem();
            }

            _action?.Invoke(item);

            lock (_syncObject)
            {
                if (_queue.Count == 0)
                    _emptyEvent.Set();
            }

            ResetDelay();
        }
    }

    private void ResetDelay()
    {
        if (_delay > 0)
            _lastActionCalled = DateTime.UtcNow;
    }

    private void Run()
    {
        _stop = false;
        IsRunning = true;
        _startedEvent.Set();

        while (!_stop)
        {
            _itemEnqueuedEvent.WaitOne();

            lock (_syncObject)
            {
                if (_stop || _queue.Count == 0)
                    continue;
            }

            if (_action != null)
                ProcessNextItem();
            else
                ProcessAllItems();
        }

        IsRunning = false;
        Clear();

        _thread = null;
        _stoppedEvent.Set();
    }
}