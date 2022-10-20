namespace Scotec.Transactions
{
    #region TransactionHandler delegates

    public delegate void OnBeforeCommitTransactionEventHandler();

    public delegate void OnAfterCommitTransactionEventHandler();

    public delegate void OnBeforeRollBackTransactionEventHandler();

    public delegate void OnAfterRollBackTransactionEventHandler();

    #endregion TransactionHandler delegates 
}