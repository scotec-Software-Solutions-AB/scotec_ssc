namespace Scotec.XMLDatabase.ChangeNotification
{
    internal class ChangeNotificationObject : IChangeNotificationObject
    {
        public ChangeNotificationObject( IBusinessObject businessObject, EChangeNotificationType changeType )
        {
            BusinessObject = businessObject;
            ChangeType = changeType;
        }


        #region IChangeNotificationObject Members

        public IBusinessObject BusinessObject { get; private set; }

        public EChangeNotificationType ChangeType { get; private set; }

        #endregion
    }
}
