namespace Scotec.XMLDatabase
{
    public interface IDataConverter
    {
        int Run( IDataObject dataIn, IDataObject dataOut );


        bool WaitUntilFinished();
    }
}
