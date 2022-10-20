#region

using System.Collections;

#endregion


namespace Scotec.Transactions
{
    public class TransactionManager : ITransactionManager
    {
        private readonly Hashtable _handler = new Hashtable();


        #region ITransactionManager Members

        ITransactionHandler ITransactionManager.GetTransactionHandler(string name)
        {
            return GetTransactionHandler(name);
        }


        ITransactionHandler ITransactionManager.CreateTransactionHandler(string name)
        {
            return CreateTransactionHandler(name);
        }


        void ITransactionManager.ReleaseTransactionHandler(string name, bool commit)
        {
            ReleaseTransactionHandler(name, commit);
        }


        bool ITransactionManager.HasTransactionHandler(string name)
        {
            return HasTransactionHandler(name);
        }


        public ITransaction CreateTransaction(object owner, ETransactionMode mode, string text)
        {
            return GetTransactionHandler("").CreateTransaction(owner, mode, text);
        }


        public ITransaction RunningTransaction =>
            HasTransactionHandler("") ? GetTransactionHandler("").RunningTransaction : null;

        #endregion


        public ITransactionHandler GetTransactionHandler(string name)
        {
            if (string.IsNullOrEmpty(name))
                name = "__internalTrxHandler";

            if (!_handler.ContainsKey(name))
                throw new TransactionException("Handler with name '" + name + "' does not exist.");

            return (ITransactionHandler) _handler[name];
        }


        public ITransactionHandler CreateTransactionHandler(string name)
        {
            if (string.IsNullOrEmpty(name))
                name = "__internalTrxHandler";

            if (_handler.ContainsKey(name))
                throw new TransactionException("Handler with name '" + name + "' does already exist.");

            var handler = new TransactionHandler(name);
            _handler[name] = handler;

            return handler;
        }


        public void ReleaseTransactionHandler(string name, bool commit)
        {
            if (string.IsNullOrEmpty(name))
                name = "__internalTrxHandler";

            if (!_handler.ContainsKey(name))
                throw new TransactionException("Handler with name '" + name + "' does not exist.");

            var handler = (TransactionHandler) _handler[name];
            _handler.Remove(name);

            //Commit/roll back all transactions.
            if (commit)
                handler.CommitAll();
            else
                handler.RollbackAll();
        }


        public bool HasTransactionHandler(string name)
        {
            if (string.IsNullOrEmpty(name))
                name = "__internalTrxHandler";

            return _handler.ContainsKey(name);
        }
    }
}