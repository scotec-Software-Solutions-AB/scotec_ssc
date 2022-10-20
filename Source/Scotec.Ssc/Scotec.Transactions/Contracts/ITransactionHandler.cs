#region

using System;

#endregion


namespace Scotec.Transactions
{
    public interface ITransactionHandler
    {
        /// <summary>
        ///     Returns the name of the transaction handler. The name is used to get an
        ///     transaction handler instance from the trx manager.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Returns the currently running transaction handled by the trx handler.
        /// </summary>
        ITransaction RunningTransaction { get; }


        /// <summary>
        ///     Creates a new transaction. If there is already a running transaction
        ///     belonging to the same thread, a nested transaction will becreated.
        ///     If the CreateTransaction() has been called from a different thread,
        ///     the thread will be blocked until the current running transaction has
        ///     been committed, rolled back, or set into the pending state.
        /// </summary>
        /// <param name="owner">
        ///     The owner of the new transaction. This object must
        ///     be passed when committing, rolling back or setting into the pending state
        /// </param>
        /// <param name="mode">The mode for the new trx.</param>
        /// <param name="text">A string describing the trx.</param>
        /// <returns></returns>
        ITransaction CreateTransaction(object owner, ETransactionMode mode, string text);


        /// <summary>
        ///     Gets a transaction by its ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ITransaction GetTransaction(Guid id);


        /// <summary>
        ///     Commits a running or pending trx. Committing a transaction
        ///     causes a commit of all previous transaction that has been set
        ///     into the pending state.
        /// </summary>
        /// <param name="transaction">The trx to be commited.</param>
        /// <param name="owner">The owner of the trx.</param>
        void Commit(ITransaction transaction, object owner);


        /// <summary>
        ///     Rolls back a trx. Rolling back a transaction causes a roll back
        ///     of all following pending transaction. If ther is still a running
        ///     transaction, a roll back can be done for that running trx only.
        /// </summary>
        /// <param name="transaction">The trx to be rolled back</param>
        /// <param name="owner">The owner of the trx.</param>
        void Rollback(ITransaction transaction, object owner);


        /// <summary>
        ///     Sets a running trx into the pending state.
        /// </summary>
        /// <param name="transaction">The trx to be set into the pending state.</param>
        /// <param name="owner">The owner of the trx.</param>
        void SetPending(ITransaction transaction, object owner);


        /// <summary>
        ///     Tests whether a transaction can be created in the given mode.
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        bool IsValidTransactionMode(ETransactionMode mode);


        #region TransactionHandler events

        event OnBeforeCommitTransactionEventHandler OnBeforeCommitTransaction;
        event OnAfterCommitTransactionEventHandler OnAfterCommitTransaction;

        event OnBeforeRollBackTransactionEventHandler OnBeforeRollBackTransaction;
        event OnAfterRollBackTransactionEventHandler OnAfterRollBackTransaction;

        #endregion events
    }
}