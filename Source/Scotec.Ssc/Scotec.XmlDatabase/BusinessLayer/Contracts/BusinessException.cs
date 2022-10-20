#region

using System;

#endregion


namespace Scotec.XMLDatabase
{
    public enum EBusinessError : uint
    {
        // Common
        Document = 0x40001001,
        NotSupported = 0x40001002,
        Schema = 0x40001003,
        Rule = 0x40001004,
        Listener = 0x40001005,
        Session = 0x40001006, 

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

    public class BusinessException : Exception
    {
        private readonly EBusinessError _error = EBusinessError.Document;


        public BusinessException( EBusinessError error )
        {
            _error = error;
        }


        public BusinessException( EBusinessError error, string message )
                : base( message )
        {
            _error = error;
        }


        public BusinessException( EBusinessError error, string message, Exception innerException )
                : base( message, innerException )
        {
            _error = error;
        }


        public EBusinessError BusinessError
        {
            get { return _error; }
        }
    }
}
