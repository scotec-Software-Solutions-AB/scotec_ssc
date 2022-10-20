#region

using System;

#endregion


namespace Scotec.XMLDatabase
{
    public enum EDataError : uint
    {
        // Common
        Document = 0x40001001,
        NotSupported = 0x40001002,
        Schema = 0x40001003,
        Rule = 0x40001004,
        Listener = 0x40001005,

        // Connection
        Connection = 0x40002001,

        // Data access
        AccessDenied = 0x40003001,
        ReadError = 0x40003002,
        WriteError = 0x40003003,
        MissingAttribute = 0x40003004,
        Deleted = 0x40003005,

        //Update
        UpdateCancelled = 0x40004001,
        UpdateFailed = 0x40004002,
    }

    public class DataException : Exception
    {
        private readonly EDataError _error = EDataError.Document;


        public DataException( EDataError error )
        {
            _error = error;
        }


        public DataException( EDataError error, string message )
                : base( message )
        {
            _error = error;
        }


        public DataException( EDataError error, string message, Exception innerException )
                : base( message, innerException )
        {
            _error = error;
        }


        public EDataError DataError
        {
            get { return _error; }
        }
    }
}
