namespace Scotec.Transactions
{
    public interface ITransactionManager
    {
        ITransaction RunningTransaction { get; }


        /// <summary>
        ///     Creates a new transaction handler with a given name.
        /// </summary>
        /// <param name="name">The name used to access the handler.</param>
        /// <returns></returns>
        ITransactionHandler CreateTransactionHandler(string name);


        /// <summary>
        ///     Gets the transaction handler with the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        ITransactionHandler GetTransactionHandler(string name);


        /// <summary>
        ///     Releases a transaction handler with a given name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="commit"></param>
        void ReleaseTransactionHandler(string name, bool commit);


        /// <summary>
        ///     Checks if a trx handler with the given name exists.
        /// </summary>
        /// <param name="name">The name of the trx handler to look for.</param>
        /// <returns>Returns true if the trx handler exists, otherwide false.</returns>
        bool HasTransactionHandler(string name);


        ITransaction CreateTransaction(object owner, ETransactionMode mode, string text);
    }
}