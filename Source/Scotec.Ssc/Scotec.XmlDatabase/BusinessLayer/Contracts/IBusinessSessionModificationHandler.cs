namespace Scotec.XMLDatabase
{
    public interface IBusinessSessionModificationHandler
    {
        void Run( IBusinessSession session, DataChangedEventArgs args );
    }
}