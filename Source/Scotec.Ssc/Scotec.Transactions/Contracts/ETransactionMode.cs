namespace Scotec.Transactions
{
    //TODO: Check if these modes are really needed.
    //      Maybe this could be used to synchronize access between transactions
    //      owned by other trx handler. In this case a "Write" mode could be
    //      needed as well.

    public enum ETransactionMode
    {
        /// <summary>
        ///     Write access for the current transaction. All threads can read and write.
        /// </summary>
        Write,

        /// <summary>
        ///     Grants exclusive write access. No other thread can run within this transaction.
        ///     Other threads will be blocked if they try to create a transaction. However, this
        ///     does not affect transaction owned by a different trx handler.
        ///     Read and write particles are supported in the exclusive write mode.
        /// </summary>
        ExclusiveWrite,

        /// <summary>
        ///     Reading access. All threads have read access. No write particles can be
        ///     used in read transaction. However, this does not affect transaction owned
        ///     by a different trx handler.
        /// </summary>
        Read,

        /// Grants exclusive read access. No other thread can run within this transaction.
        /// Other threads will be blocked if they try to create a transaction. However, this
        /// does not affect transaction owned by a different trx handler.
        /// Read  particles are supported in the exclusive read mode only.
        ExclusiveRead
    }
}