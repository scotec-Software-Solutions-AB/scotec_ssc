#region

using System;

#endregion


namespace Scotec.Transactions
{
    /// <summary>
    ///     Transaction state.
    /// </summary>
    public enum ETransactionStatus
    {
        Running,
        Pending,
        Committed,
        RolledBack,
        Closed
    }

    public interface ITransaction : IDisposable
    {
        /// <summary>
        ///     The Id of the transaction.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        ///     A describing string. Can be empty.
        /// </summary>
        string Text { get; }

        /// <summary>
        ///     The current status of the transaction.
        /// </summary>
        ETransactionStatus Status { get; }

        ETransactionMode Mode { get; }

        /// <summary>
        ///     Returns a reference to the transaction handler.
        /// </summary>
        ITransactionHandler TransactionHandler { get; }


        /// <summary>
        ///     Creates a new trx particle of the given type.
        /// </summary>
        /// <param name="type">The tye of the particle to create.</param>
        /// <param name="mode">The mode of the particle.</param>
        /// <returns></returns>
        ITransactionParticle CreateParticle(Type type, EParticleMode mode);


        /// <summary>
        ///     Releases a particle.
        /// </summary>
        /// <param name="particle">The particle to release.</param>
        void ReleaseParticle(ITransactionParticle particle);


        /// <summary>
        ///     Commits the transaction.
        /// </summary>
        /// <param name="owner">The owner of the particle.</param>
        void Commit(object owner);


        /// <summary>
        ///     Rolls back the transaction.
        /// </summary>
        /// <param name="owner">The owner of the particle.</param>
        void Rollback(object owner);


        /// <summary>
        ///     Sets a transaction into the pending state.
        /// </summary>
        /// <param name="owner">The owner of the particle.</param>
        void SetPending(object owner);
    }
}