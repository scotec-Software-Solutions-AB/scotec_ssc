#region

using System;
using Scotec.Transactions;

#endregion


namespace Scotec.XMLDatabase
{
    internal class BusinessTransaction : IBusinessTransaction
    {
        public BusinessTransaction(IBusinessSession session, ITransaction transaction)
        {
            Session = session;
            Transaction = transaction;
        }

        #region IBusinessTransaction Members

        public void Dispose()
        {
            try
            {
                Transaction.Dispose();
            }
            catch( Exception e)
            {
                throw new BusinessException( EBusinessError.Session, "Disposing the transaction failed.", e );
            }
        }


        public void Commit()
        {
            try
            {
                Transaction.Commit( Session );
            }
            catch( Exception e)
            {
                throw new BusinessException(EBusinessError.Session, "Committing the transaction failed.", e);
            }
        }


        public void Rollback()
        {
            try
            {
                Transaction.Rollback(Session);
            }
            catch (Exception e)
            {
                throw new BusinessException(EBusinessError.Session, "Roll back of the transaction failed.", e);
            }
        }


        public void SetPending()
        {
            try
            {
                Transaction.SetPending(Session);
            }
            catch (Exception e)
            {
                throw new BusinessException(EBusinessError.Session, "Setting the transaction into the pending state failed.", e);
            }
        }


        public IBusinessSession Session { get; private set; }

        public ETransactionStatus Status
        {
            get
            {
                try
                {
                    return Transaction.Status;
                }
                catch (Exception e)
                {
                    throw new BusinessException(EBusinessError.Session, "Returning the transaction status failed.", e);
                }
            }
        }

        public ETransactionMode Mode
        {
            get
            {
                try
                {
                    return Transaction.Mode;
                }
                catch (Exception e)
                {
                    throw new BusinessException(EBusinessError.Session, "Returning the transaction mode failed.", e);
                }
            }
        }

        #endregion


        private ITransaction Transaction { get; set; }
    }
}
