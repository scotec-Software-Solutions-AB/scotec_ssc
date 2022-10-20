#region

using System;

#endregion


namespace Scotec.Transactions
{
    /// <summary>
    ///     Summary description for TransactionParticle.
    /// </summary>
    public abstract class TransactionParticle : ITransactionParticle
    {
        /// <summary>
        ///     The Id of the particle
        /// </summary>
        private readonly Guid _id = Guid.NewGuid();

        /// <summary>
        ///     The caller of commit or rollback must authorize itself by providing this ID.
        /// </summary>
        private Guid _authorizationId;

        private bool _disposed;
        private ITransaction _transaction;

        internal bool Released { get; private set; }


        #region ITransactionParticle Members

        public void Initialize(ITransaction transaction, Guid authorizationId)
        {
            _transaction = transaction;
            _authorizationId = authorizationId;
        }


        public bool Commit(Guid authorizationId)
        {
            if (_authorizationId != authorizationId)
                throw new TransactionException(_transaction, "Unauthorized commit.");

            return Commit();
        }


        public bool Rollback(Guid authorizationId)
        {
            if (_authorizationId != authorizationId)
                throw new TransactionException(_transaction, "Unauthorized roolback.");

            return Rollback();
        }


        public void Release(Guid authorizationId)
        {
            if (!Released)
            {
                if (_authorizationId != authorizationId)
                    throw new TransactionException(_transaction, "Unauthorized release.");

                Released = true;
            }
        }


        public EParticleMode Mode { get; set; }


        Guid ITransactionParticle.Id => _id;


        bool ITransactionParticle.SupportsMode(EParticleMode mode)
        {
            return SupportsMode(mode);
        }


        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                if (!Released)
                    _transaction.ReleaseParticle(this);
                GC.SuppressFinalize(this);
            }
        }

        #endregion


        protected abstract bool SupportsMode(EParticleMode mode);


        protected abstract bool Commit();


        protected abstract bool Rollback();


        ~TransactionParticle()
        {
            Dispose();
        }
    }
}