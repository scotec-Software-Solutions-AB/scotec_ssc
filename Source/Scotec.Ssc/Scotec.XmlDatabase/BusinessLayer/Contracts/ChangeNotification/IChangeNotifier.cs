namespace Scotec.XMLDatabase.ChangeNotification
{
    public interface IChangeNotifier
    {
        void AddChangedObject( IBusinessObject obj, EChangeNotificationType type );


        void Lock();


        void Unlock();
    }
}
