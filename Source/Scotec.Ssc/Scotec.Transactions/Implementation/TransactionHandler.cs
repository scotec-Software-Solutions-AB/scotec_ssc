#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

#endregion


namespace Scotec.Transactions
{
    /// <summary>
    ///     Summary description for TransactionHandler.
    /// </summary>
    internal class TransactionHandler : ITransactionHandler
    {
        #region TransactionHandler events

        public event OnBeforeCommitTransactionEventHandler OnBeforeCommitTransaction;
        public event OnAfterCommitTransactionEventHandler OnAfterCommitTransaction;

        public event OnBeforeRollBackTransactionEventHandler OnBeforeRollBackTransaction;
        public event OnAfterRollBackTransactionEventHandler OnAfterRollBackTransaction;

        #endregion TransactionHandler events


        private readonly Stack _runingTransactions = new Stack();
        private readonly Dictionary<Guid, ITransaction> _transactionTable = new Dictionary<Guid, ITransaction>();
        private readonly IList<ITransaction> _transactions = new List<ITransaction>();


        public TransactionHandler(string name)
        {
            Name = name;
        }


        #region ITransactionHandler Members

        public void Rollback(ITransaction transaction, object owner)
        {
            try
            {
                Monitor.Enter(this);
                transaction.Rollback(owner);
            }
            catch (Exception e)
            {
                throw new TransactionException(transaction, "Error in rollback.", e);
            }
            finally
            {
                Monitor.Exit(this);
            }
        }


        public void Commit(ITransaction transaction, object owner)
        {
            try
            {
                Monitor.Enter(this);
                transaction.Commit(owner);
            }
            catch (Exception e)
            {
                throw new TransactionException(transaction, "Error in commit.", e);
            }
            finally
            {
                Monitor.Exit(this);
            }
        }


        public void SetPending(ITransaction transaction, object owner)
        {
            try
            {
                Monitor.Enter(this);
                transaction.SetPending(owner);
            }
            catch (Exception e)
            {
                throw new TransactionException(transaction, "Error while setting transaction into the pending state.",
                    e);
            }
            finally
            {
                Monitor.Exit(this);
            }
        }


        public ITransaction RunningTransaction
        {
            get
            {
                try
                {
                    Monitor.Enter(this);
                    if (_runingTransactions.Count > 0)
                        return (ITransaction) _runingTransactions.Peek();
                    return null;
                }
                catch (Exception e)
                {
                    throw new TransactionException("Error while getting running transaction.", e);
                }
                finally
                {
                    Monitor.Exit(this);
                }
            }
        }

        public string Name { get; }


        public ITransaction GetTransaction(Guid id)
        {
            ITransaction transaction;

            _transactionTable.TryGetValue(id, out transaction);

            return transaction;
        }


        public ITransaction CreateTransaction(object owner, ETransactionMode mode, string text)
        {
            ITransaction transaction;

            try
            {
                // Lock any requests from different threads.
                Monitor.Enter(this);

                // Check if the new mode is valid in the current context.
                if (!IsValidTransactionMode(mode))
                    throw new TransactionException("Transaction mode '" + mode
                                                                        + "' is not allowed in the current context.");

                //Get the running trx.
                var runningTransaction = RunningTransaction;

                // Just create a new transaction
                transaction = new Transaction(this, owner, text, mode);

                if (runningTransaction == null)
                {
                    _transactions.Add(transaction);
                    _transactionTable.Add(transaction.Id, transaction);
                }
                else
                {
                    // Make it a nested trx.
                    ((Transaction) transaction).Parent = (Transaction) runningTransaction;

                    // If a differnet thread tries to created a particle,
                    // it must wait until the nested transaction has been released.
                    Monitor.Enter(runningTransaction);

                    // Lock the trx handler as long as a nested transaction is living.
                    // During this time no other thread can enter the transaction handler.
                    // Thus no other thread can create a nested transaction.
                    Monitor.Enter(this);
                }

                _runingTransactions.Push(transaction);
            }
            catch (Exception e)
            {
                throw new TransactionException("Error while creating transaction.", e);
            }
            finally
            {
                Monitor.Exit(this);
            }

            return transaction;
        }


        public bool IsValidTransactionMode(ETransactionMode mode)
        {
            // If there is no running transaction, all modes are allowed.
            if (RunningTransaction == null)
                return true;

            var valid = false;
            switch (RunningTransaction.Mode)
            {
                case ETransactionMode.Write:
                {
                    valid = true;
                    break;
                }
                case ETransactionMode.ExclusiveWrite:
                {
                    if (mode == ETransactionMode.ExclusiveWrite || mode == ETransactionMode.ExclusiveRead)
                        valid = true;
                    break;
                }
                case ETransactionMode.Read:
                {
                    if (mode == ETransactionMode.ExclusiveRead || mode == ETransactionMode.Read)
                        valid = true;
                    break;
                }
                case ETransactionMode.ExclusiveRead:
                {
                    if (mode == ETransactionMode.ExclusiveRead)
                        valid = true;
                    break;
                }
            }

            return valid;
        }

        #endregion


        internal void RollbackAll()
        {
            try
            {
                Monitor.Enter(this);

                var count = _transactions.Count;
                if (count > 0)
                {
                    var trx = (Transaction) _transactions[0];
                    Rollback(trx, trx.Owner);
                }
            }
            catch (Exception e)
            {
                throw new TransactionException("Error while rolling back transactions.", e);
            }
            finally
            {
                Monitor.Exit(this);
            }
        }


        internal void Rollback(ITransaction transaction)
        {
            try
            {
                if (OnBeforeRollBackTransaction != null)
                    OnBeforeRollBackTransaction();

                Monitor.Enter(this);

                if (((Transaction) transaction).Nested)
                {
                    ((Transaction) transaction).Rollback();
                }
                else
                {
                    var count = _transactions.Count;
                    var i = count - 1;
                    for (; i >= 0; --i)
                    {
                        var trx = (Transaction) _transactions[i];
                        trx.Rollback();
                        _transactionTable.Remove(trx.Id);
                        if (trx == transaction) break;
                    }

                    RemoveRange(i, count - i);
                }

                if (OnAfterRollBackTransaction != null)
                    OnAfterRollBackTransaction();
            }
            catch (Exception e)
            {
                throw new TransactionException(transaction, "Error while rolling back transaction.", e);
            }
            finally
            {
                Monitor.Exit(this);
            }
        }


        internal void CommitAll()
        {
            try
            {
                Monitor.Enter(this);

                var count = _transactions.Count;
                if (count > 0)
                {
                    var trx = (Transaction) _transactions[count - 1];
                    Commit(trx, trx.Owner);
                }
            }
            catch (Exception e)
            {
                throw new TransactionException("Error while committing transactions.", e);
            }
            finally
            {
                Monitor.Exit(this);
            }
        }


        internal void Commit(ITransaction transaction)
        {
            // Do nothing for nested trx.
            if (((Transaction) transaction).Nested)
                return;

            try
            {
                if (OnBeforeCommitTransaction != null)
                    OnBeforeCommitTransaction();

                Monitor.Enter(this);

                var count = _transactions.Count;
                var i = 0;
                for (; i < count; ++i)
                {
                    var trx = (Transaction) _transactions[i];
                    trx.Commit();
                    _transactionTable.Remove(trx.Id);
                    if (trx == transaction) break;
                }

                RemoveRange(0, i + 1 /*count*/);

                if (OnAfterCommitTransaction != null)
                    OnAfterCommitTransaction();
            }
            catch (Exception e)
            {
                throw new TransactionException(transaction, "Error while committing transaction.", e);
            }
            finally
            {
                Monitor.Exit(this);
            }
        }


        internal void SetPending(ITransaction transaction)
        {
            try
            {
                Monitor.Enter(this);

                if (transaction.Status != ETransactionStatus.Pending
                    && transaction.Status != ETransactionStatus.Closed)
                {
                    ((Transaction) transaction).SetPending();

                    // Remove the trx from the running transaction stack.
                    var runningTransaction = (ITransaction) _runingTransactions.Peek();
                    if (transaction != runningTransaction)
                        throw new Exception("Transaction is not the running transaction.");

                    _runingTransactions.Pop();

                    if (_runingTransactions.Count >= 1)
                    {
                        // Unlock the top transaction
                        Monitor.Exit(_runingTransactions.Peek());
                        Monitor.Exit(this);
                    }
                }
            }
            catch (Exception e)
            {
                throw new TransactionException(transaction, "Error while setting pending state.", e);
            }
            finally
            {
                Monitor.Exit(this);
            }
        }


        private void RemoveRange(int index, int count)
        {
            for (var i = 0; i < count; i++)
                _transactions.RemoveAt(index);
        }
    }
}