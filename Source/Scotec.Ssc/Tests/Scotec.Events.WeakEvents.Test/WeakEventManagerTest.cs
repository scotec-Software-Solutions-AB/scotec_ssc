using Xunit;

namespace Scotec.Events.WeakEvents.Test
{
    public class WeakEventManagerTest
    {
        [Fact]
        public void Test()
        {
            var weakEventManager = new WeakEventManager();

            var sender = new Sender();
            StaticWeakEventManager.AddWeakHandler<Sender, EventArgs>(sender, nameof(Sender.MyEvent), TestOnMyEvent);
            StaticWeakEventManager.AddWeakHandler<Sender, EventArgs>(sender, nameof(Sender.MyEvent), TestOnMyEvent);
            sender.RaiseEvent();

            StaticWeakEventManager.RemoveWeakHandler<Sender, EventArgs>(sender, nameof(Sender.MyEvent), TestOnMyEvent);
            sender.RaiseEvent();
            StaticWeakEventManager.RemoveWeakHandler<Sender, EventArgs>(sender, nameof(Sender.MyEvent), TestOnMyEvent);
            sender.RaiseEvent();

        }

        private void TestOnMyEvent(Sender sender, EventArgs e)
        {
        }

    }




    public class Sender()
    {
        public event EventHandler<EventArgs>? MyEvent;

        public void RaiseEvent()
        {
            MyEvent?.Invoke(this, new EventArgs());
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
