#region

using System;

#endregion


namespace Scotec.XMLDatabase
{
    public interface IBusinessObjectConverter : IDisposable
    {
        int Run( IBusinessObject dataIn, IBusinessObject dataOut );


        bool WaitUntilFinished();


        void Cancel();
    }
}
