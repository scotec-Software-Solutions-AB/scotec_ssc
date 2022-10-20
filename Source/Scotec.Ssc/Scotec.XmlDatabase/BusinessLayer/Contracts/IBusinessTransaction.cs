#region

using System;
using Scotec.Transactions;

#endregion


namespace Scotec.XMLDatabase
{
    public interface IBusinessTransaction : IDisposable
    {
        IBusinessSession Session { get; }


        /// <summary>
        ///   The current status of the transaction.
        /// </summary>
        ETransactionStatus Status { get; }


        /// <summary>
        ///   The transaction mode.
        /// </summary>
        ETransactionMode Mode { get; }

        /// <summary>
        ///   Commits the transaction.
        /// </summary>
        void Commit();


        /// <summary>
        ///   Rolls back the transaction.
        /// </summary>
        void Rollback();


        /// <summary>
        ///   Sets the transaction into the pending state.
        /// </summary>
        void SetPending();
    }
}