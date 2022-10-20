#region

using System.Xml;

#endregion


namespace Scotec.XMLDatabase.ReaderWriter.Transactions
{
    /// <summary>
    ///   Summary description for XMLTransactionElementMoved.
    /// </summary>
    internal class XmlTransactionElementMoved : XmlTransactionParticle
    {
        public XmlElement Parent { get; set; }

        public int From { get; set; }

        public int To { get; set; }


        protected override bool RollbackInternal()
        {
            Document.DataObjectFactory.InternalMoveElement( Parent, To, From, false );
            Document.DataObjectFactory.AddModifiedElementToChangeNotifier( Parent );

            return true;
        }


        protected override bool CommitInternal()
        {
            return true;
        }
    }
}
