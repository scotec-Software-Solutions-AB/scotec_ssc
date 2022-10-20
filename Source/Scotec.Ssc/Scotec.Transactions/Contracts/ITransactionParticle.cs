#region

using System;

#endregion


namespace Scotec.Transactions
{
    public interface ITransactionParticle : IDisposable
    {
        /// <summary>
        ///     The particles ID.
        /// </summary>
        Guid Id { get; }

        EParticleMode Mode { get; }


        /// <summary>
        ///     Test if the particle supports read or write mode.
        /// </summary>
        /// <param name="mode">The mode to test for.</param>
        /// <returns>Return true if the particle supports the given mode, otherwise false.</returns>
        bool SupportsMode(EParticleMode mode);


        bool Commit(Guid authorizationId);


        bool Rollback(Guid authorizationId);


        void Release(Guid authorizationId);


        void Initialize(ITransaction transaction, Guid authorizationId);
    }
}