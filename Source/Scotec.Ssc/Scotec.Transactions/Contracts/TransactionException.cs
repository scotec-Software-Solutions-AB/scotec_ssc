#region

using System;

#endregion


namespace Scotec.Transactions
{
    /// <summary>
    ///     Summary description for TransactionException.
    /// </summary>
    public class TransactionException : Exception
    {
        public TransactionException()
        {
        }


        public TransactionException(string message)
            : base(message)
        {
        }


        public TransactionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }


        public TransactionException(ITransaction transaction)
        {
            Transaction = transaction;
        }


        public TransactionException(ITransaction transaction, string message)
            : base(message)
        {
            Transaction = transaction;
        }


        public TransactionException(ITransaction transaction, string message, Exception innerException)
            : base(message, innerException)
        {
            Transaction = transaction;
        }


        public TransactionException(ITransaction transaction, ITransactionParticle particle)
        {
            Transaction = transaction;
            TransactionParticle = particle;
        }


        public TransactionException(ITransaction transaction, ITransactionParticle particle, string message)
            : base(message)
        {
            Transaction = transaction;
            TransactionParticle = particle;
        }


        public TransactionException(
            ITransaction transaction, ITransactionParticle particle, string message, Exception innerException)
            : base(message, innerException)
        {
            Transaction = transaction;
            TransactionParticle = particle;
        }


        public ITransaction Transaction { get; set; }

        public ITransactionParticle TransactionParticle { get; set; }
    }
}