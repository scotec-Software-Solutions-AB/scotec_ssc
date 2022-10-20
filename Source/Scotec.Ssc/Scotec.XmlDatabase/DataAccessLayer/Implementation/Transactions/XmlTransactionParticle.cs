#region

using Scotec.Transactions;
using Scotec.XMLDatabase.ReaderWriter.Xml;

#endregion


namespace Scotec.XMLDatabase.ReaderWriter.Transactions
{
    /// <summary>
    ///   Summary description for XmlTransactionParticle.
    /// </summary>
    internal abstract class XmlTransactionParticle : TransactionParticle
    {
        #region Private Member

        private bool _open = true;

        #endregion Private Member


        #region Constructor

        #endregion Constructor


        public XmlDataDocument Document { get; set; }


        #region Protected Properties

        private bool Open
        {
            get { return _open; }
        }

        #endregion Protected Properties


        #region Protected Methods

        protected abstract bool RollbackInternal();


        protected abstract bool CommitInternal();

        #endregion Protected Methods


        protected override bool SupportsMode( EParticleMode mode )
        {
            return (mode == EParticleMode.Write);
        }


        protected override bool Rollback()
        {
            if( Open )
            {
                _open = false;
                return RollbackInternal();
            }
            return false;
        }


        protected override bool Commit()
        {
            if( Open )
            {
                _open = false;
                return CommitInternal();
            }
            return false;
        }
    }
}
