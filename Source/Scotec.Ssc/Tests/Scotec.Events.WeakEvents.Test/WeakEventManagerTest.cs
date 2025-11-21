using System;
using Xunit;

namespace Scotec.Events.WeakEvents.Test
{
    public class MyEventArgs : EventArgs
    {

    }


    public class WeakEventManagerTest
    {
        [Fact]
        public void Test()
        {
            var weakEventManager = new WeakEventManager();
            
            var sender = new Sender();
            weakEventManager.AddWeakHandler(sender, nameof(Sender.MyEvent), TestOnMyEvent);
            weakEventManager.AddWeakHandler<Sender, MyEventArgs>(sender, nameof(Sender.MyEvent2), TestOnMyEvent2);
            sender.RaiseEvent();

            weakEventManager.RemoveWeakHandler<Sender, MyEventArgs>(sender, nameof(Sender.MyEvent), TestOnMyEvent);
            sender.RaiseEvent();
            weakEventManager.RemoveWeakHandler<Sender, EventArgs>(sender, nameof(Sender.MyEvent), TestOnMyEvent);
            sender.RaiseEvent();

        }


        private void TestOnMyEvent(Sender sender, EventArgs e)
        {
        }
        private void TestOnMyEvent2(Sender sender, MyEventArgs e)
        {
        }

    }




    public class Sender()
    {
        public event EventHandler<EventArgs>? MyEvent;
        public event EventHandler<MyEventArgs>? MyEvent2;
        //public event MyEventHandler<Sender, EventArgs>? MyEvent2;

        public void RaiseEvent()
        {
            MyEvent?.Invoke(this, new EventArgs());
            MyEvent2?.Invoke(this, new MyEventArgs());
        }
    }


    public class Observer
    {
        public Observer(Sender test)
        {
            //StaticWeakEventManager.AddWeakHandler<Sender, EventArgs>(test, nameof(Sender.MyEvent), OnMyEvent);
        }

        private void OnMyEvent(Sender sender, EventArgs e)
        {
        }

        ~Observer()
        {

        }
    }


}
