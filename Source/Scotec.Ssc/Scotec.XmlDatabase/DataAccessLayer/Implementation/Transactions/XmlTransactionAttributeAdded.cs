#region

using System.Xml;

#endregion


namespace Scotec.XMLDatabase.ReaderWriter.Transactions
{
    /// <summary>
    ///   Summary description for XmlTransactionDataAdded.
    /// </summary>
    internal class XmlTransactionAttributeAdded : XmlTransactionParticle
    {
        public XmlElement Parent { get; set; }

        public XmlAttribute Attribute { get; set; }


        protected override bool RollbackInternal()
        {
            Document.DataObjectFactory.AddRemovedAttributeToChangeNotifier( Attribute );
            Document.DataObjectFactory.AddRecreateObject( Attribute.Name, Attribute );
            Document.DataObjectFactory.InternalRemoveAttribute( Parent, Attribute, false );
            return true;
        }


        protected override bool CommitInternal()
        {
            return true;
        }
    }
}
