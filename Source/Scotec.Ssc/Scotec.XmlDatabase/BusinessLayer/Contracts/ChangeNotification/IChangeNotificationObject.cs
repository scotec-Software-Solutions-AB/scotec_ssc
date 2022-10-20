namespace Scotec.XMLDatabase.ChangeNotification
{
    public interface IChangeNotificationObject
    {
        IBusinessObject BusinessObject { get; }
        EChangeNotificationType ChangeType { get; }
    }
}
