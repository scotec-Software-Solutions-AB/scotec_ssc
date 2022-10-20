#region

using System;

#endregion


namespace Scotec.XMLDatabase
{
    /// <summary>
    ///   <para> Call GetNotificationLock on the session to create an change notification lock. To release the lock, dispose the lock object. After the lock has been disposed, the session will send a change notification if any objects have changed during the fifetime of the lock. </para>
    ///   <para> Nested locks are possible. Notifications will be send after all locks have been disposed. </para>
    /// </summary>
    public interface INotificationLock : IDisposable
    {
    }
}