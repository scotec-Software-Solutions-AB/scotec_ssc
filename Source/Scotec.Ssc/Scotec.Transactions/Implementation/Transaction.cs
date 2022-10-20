#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

#endregion


namespace Scotec.Transactions
{
    /// <summary>
    ///     Summary description for Transaction.
    /// </summary>
    public class Transaction : ITransaction
    {
        private readonly Guid _authorizationId = Guid.NewGuid();

        private readonly IDictionary<Type, ConstructorInfo> _particleConstructors =
            new Dictionary<Type, ConstructorInfo>();

        private readonly ArrayList _particles = new ArrayList(128);

        private readonly TransactionHandler _trxHandler;
        private ITransactionParticle _currentParticle;
        private bool _disposed;
        private bool _rollback;


        internal Transaction(ITransactionHandler trxHandler, object owner, string text, ETransactionMode mode)
        {
            _trxHandler = (TransactionHandler) trxHandler;
            Owner = owner;
            Text = text;
            Mode = mode;
        }


        public Transaction Parent { get; set; }

        public bool Nested => Parent != null;

        internal object Owner { get; private set; }


        #region ITransaction Members

        public Guid Id { get; } = Guid.NewGuid();


        public string Text { get; }

        public ETransactionMode Mode { get; } = ETransactionMode.Read;


        public void Rollback(object owner)
        {
            // Treat rollback for nested trx like rollback for normal trx.
            // Particles do not need to be moved to the parent trx.
            _rollback = true;

            if (Status == ETransactionStatus.Running)
                SetPending(owner);

            if (Status == ETransactionStatus.Pending)
            {
                if (owner != null && !Owner.Equals(owner))
                    throw new TransactionException(this,
                        "Rollback() can be called by the owner of the transaction only.");
                Owner = null;

                _trxHandler.Rollback(this);
            }

            _rollback = false;
        }


        public void Commit(object owner)
        {
            if (Status == ETransactionStatus.Running)
                SetPending(owner);

            if (Status == ETransactionStatus.Pending)
            {
                if (owner != null && !Owner.Equals(owner))
                    throw new TransactionException(this,
                        "Commit() can be called by the owner of the transaction only.");
                Owner = null;

                _trxHandler.Commit(this);
            }
        }


        public void SetPending(object owner)
        {
            if (Status == ETransactionStatus.Running)
            {
                if (owner != null && !Owner.Equals(owner))
                    throw new TransactionException(this,
                        "SetPending() can be called by the owner of the transaction only.");

                _trxHandler.SetPending(this);
            }
        }


        public ITransactionHandler TransactionHandler => _trxHandler;


        /// <summary>
        ///     Creates a new particle of a user defined type. However, the type must be derived
        ///     from TransactionParticle. Only one active write or repeatable read particle can
        ///     exist within the transaction or its nested transactions but there might be multiple
        ///     instances of unrepeatable particles.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public ITransactionParticle CreateParticle(Type type, EParticleMode mode)
        {
            if (type.IsSubclassOf(typeof(TransactionParticle)) == false)
                throw new TransactionException("Transaction particle must be derived from \"TransactionParticle\".");

            if ((Mode == ETransactionMode.Read || Mode == ETransactionMode.ExclusiveRead) &&
                mode != EParticleMode.Read)
                throw new TransactionException(this,
                    "Particle mode '" + mode + "' not allowed in a '" + Mode
                    + "' transaction.");

            // All threads must wait until the currently activ particle has been released.
            // To avoid deadlocks, the owner thread of the currently active particle can
            // enter. However, an exception will be thrown.

            // The monitor will be exited in the ReleaseParticle() method.
            Monitor.Enter(this);
            if (_currentParticle != null)
            {
                Monitor.Exit(this);
                throw new TransactionException(this,
                    "Transaction particles must be atomar. A transaction particle cannot be nested in other particles.");
            }

            try
            {
                ConstructorInfo info;

                if (!_particleConstructors.TryGetValue(type, out info))
                {
                    info = type.GetConstructor(BindingFlags.Public |
                                               BindingFlags.NonPublic |
                                               BindingFlags.Instance,
                        null, Type.EmptyTypes, null);

                    _particleConstructors.Add(type, info);
                }

                var particle = (ITransactionParticle) info.Invoke(null);

                if (!particle.SupportsMode(mode))
                    throw new TransactionException(this, "Particle does not support '" + mode + "' mode.");

                particle.Initialize(this, _authorizationId);

                // Add only write particles. Read particles must be neither committed nor rolled back.
                if (mode == EParticleMode.Write)
                    _particles.Add(particle);

                _currentParticle = particle;

                return particle;
            }
            catch (Exception e)
            {
                Monitor.Exit(this);
                _currentParticle = null;
                throw new TransactionException(this, "Particle could not be created", e);
            }
        }


        public void ReleaseParticle(ITransactionParticle particle)
        {
            var transactionParticle = (TransactionParticle) particle;
            transactionParticle.Release(_authorizationId);

            if (transactionParticle.Mode == EParticleMode.Read)
                return; // Don't care about unrepeatable read particles.

            _currentParticle = null;

            // Exit the monitor entered in the CreateParticle() method.
            Monitor.Exit(this);
        }


        public ETransactionStatus Status { get; private set; } = ETransactionStatus.Running;


        void IDisposable.Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                if (Status == ETransactionStatus.Running)
                    Rollback(Owner);
                GC.SuppressFinalize(this);
            }
        }

        #endregion


        ~Transaction()
        {
            ((IDisposable) this).Dispose();
        }


        internal void Rollback()
        {
            // Roll back all particles in reverse order.
            var count = _particles.Count;
            for (var i = count - 1; i >= 0; --i)
            {
                var particle = _particles[i];
                if (particle is TransactionParticle)
                    ((TransactionParticle) particle).Rollback(_authorizationId);
            }

            ResetMember();

            Status = ETransactionStatus.RolledBack;
        }


        internal void Commit()
        {
            // Commit all particles.
            var count = _particles.Count;
            for (var i = 0; i < count; ++i)
            {
                var particle = _particles[i];
                if (particle is TransactionParticle)
                    ((TransactionParticle) particle).Commit(_authorizationId);
            }

            ResetMember();

            Status = ETransactionStatus.Committed;
        }


        internal void SetPending()
        {
            if (!_rollback && Parent != null)
            {
                // If a nested trx has been committed or set into the pending state,
                // all particles must be moved to the parent trx.
                Parent.AppendNestedParticles(_particles);
                _particles.Clear();
            }

            Status = ETransactionStatus.Pending;
        }


        private void ResetMember()
        {
            _currentParticle = null;
            _particles.Clear();
            _particles.Capacity = 0;
        }


        private void AppendNestedParticles(ArrayList particles)
        {
            foreach (TransactionParticle particle in particles)
            {
                particle.Initialize(this, _authorizationId);
                _particles.Add(particle);
            }
        }
    }
}