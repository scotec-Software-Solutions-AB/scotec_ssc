#region

using System.Xml;

#endregion


namespace Scotec.XMLDatabase.ReaderWriter.Transactions
{
    /// <summary>
    ///   Summary description for XmlTransactionElementCreated.
    /// </summary>
    internal class XmlTransactionElementCreated : XmlTransactionParticle
    {
        public XmlElement Parent { get; set; }

        public XmlElement Element { get; set; }


        protected override bool RollbackInternal()
        {
            Document.DataObjectFactory.AddRemovedElementToChangeNotifier( Element );
            Document.DataObjectFactory.AddRecreateObject( Element.Attributes["xsi:type"].Value, Element );
            Document.DataObjectFactory.InternalRemoveElement( Parent, Element, false );
            return true;
        }


        protected override bool CommitInternal()
        {
            return true;
        }
    }
}
