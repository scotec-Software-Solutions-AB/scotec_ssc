#region

using System.Xml;

#endregion


namespace Scotec.XMLDatabase.ReaderWriter.Transactions
{
    /// <summary>
    ///   Summary description for XmlTransactionElementDeleted.
    /// </summary>
    internal class XmlTransactionElementDeleted : XmlTransactionParticle
    {
        public XmlElement Parent { get; set; }

        public XmlElement Element { get; set; }

        public int Position { get; set; }


        protected override bool RollbackInternal()
        {
            Document.DataObjectFactory.InternalAddElement( Parent, Element, Position, false );
            Document.DataObjectFactory.AddAddedElementToChangeNotifier( Element );
            return true;
        }


        protected override bool CommitInternal()
        {
            return true;
        }
    }
}
