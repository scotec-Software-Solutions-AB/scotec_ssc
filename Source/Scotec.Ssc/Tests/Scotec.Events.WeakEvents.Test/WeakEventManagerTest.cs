using System;
using System.ComponentModel;
using Xunit;

namespace Scotec.Events.WeakEvents.Test
{
    public delegate void MyEventHandler(Sender sender, MyEventArgs args);

    public class MyEventArgs : EventArgs
    {

    }

    public static class Extension
    {
        public static void AddWeak(this Delegate handlerDelegate)
        {

        }
    }

    public class WeakEventManagerTest
    {
        [Fact]
        public void Test()
        {
            var sender = new Sender();

            var weakEventManager = new WeakEventManager();

            weakEventManager.AddWeakHandler<Sender, EventArgs>(sender, nameof(Sender.MyEvent), TestOnMyEvent);
            weakEventManager.AddWeakHandler<Sender, PropertyChangedEventArgs>(sender, nameof(Sender.MyEvent2), TestOnMyEvent2);
            weakEventManager.AddWeakHandler<Sender, MyEventArgs>(sender, nameof(Sender.MyEvent3), TestOnMyEvent3);

            sender.RaiseEvent();

            weakEventManager.RemoveWeakHandler<Sender, EventArgs>(sender, nameof(Sender.MyEvent), TestOnMyEvent);
            weakEventManager.RemoveWeakHandler<Sender, PropertyChangedEventArgs>(sender, nameof(Sender.MyEvent2), TestOnMyEvent2);
            weakEventManager.RemoveWeakHandler<Sender, MyEventArgs>(sender, nameof(Sender.MyEvent), TestOnMyEvent3);
            sender.RaiseEvent();

            RunTest(sender);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            sender.RaiseEvent();

        }

        private void TestOnMyEvent(Sender sender, EventArgs e)
        {
        }
        private void TestOnMyEvent2(Sender sender, PropertyChangedEventArgs e)
        {
        }

        private void TestOnMyEvent3(Sender sender, MyEventArgs e)
        {
        }

        static void RunTest(Sender sender)
        {
            var o = new Observer(sender);
            sender.RaiseEvent();

        }
    }

    public class Sender
    {
        public event EventHandler<EventArgs>? MyEvent;
        public event PropertyChangedEventHandler? MyEvent2;
        public event MyEventHandler? MyEvent3;

        public void RaiseEvent()
        {
            MyEvent?.Invoke(this, EventArgs.Empty);
            MyEvent2?.Invoke(this, new PropertyChangedEventArgs("Test"));
            MyEvent3?.Invoke(this, new MyEventArgs());
        }
    }
    
    public class Observer
    {
        public Observer(Sender test)
        {
            StaticWeakEventManager.AddWeakHandler<Sender, EventArgs>(test, nameof(Sender.MyEvent), OnMyEvent);
        }

        private void OnMyEvent(Sender sender, EventArgs e)
        {
        }

        ~Observer()
        {

        }
    }
}
