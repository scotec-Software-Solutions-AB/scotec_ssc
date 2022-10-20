#region

using Scotec.XMLDatabase.ChangeNotification;

#endregion


namespace Scotec.XMLDatabase
{
    internal class NotificationLock : INotificationLock
    {
        private readonly IChangeNotifier _changeNotifier;
        private bool _disposed;

        public NotificationLock(IChangeNotifier changeNotifier)
        {
            _changeNotifier = changeNotifier;
            _changeNotifier.Lock();
        }


        #region INotificationLock Members

        public void Dispose()
        {
            if( !_disposed )
            {
                _disposed = true;
                _changeNotifier.Unlock();
            }
        }

        #endregion
    }
}