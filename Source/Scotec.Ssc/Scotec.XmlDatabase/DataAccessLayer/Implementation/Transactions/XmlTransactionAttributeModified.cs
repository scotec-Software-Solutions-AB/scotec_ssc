#region

using System.Xml;

#endregion


namespace Scotec.XMLDatabase.ReaderWriter.Transactions
{
    /// <summary>
    ///   Summary description for XmlTransactionDataAdded.
    /// </summary>
    internal class XmlTransactionAttributeModified : XmlTransactionParticle
    {
        public XmlAttribute Attribute { get; set; }

        public string OldValue { get; set; }


        protected override bool RollbackInternal()
        {
            if( Document.DataObjectFactory.InternalModifyAttribute( Attribute, OldValue, false ) )
                Document.DataObjectFactory.AddModifiedAttributeToChangeNotifier( Attribute );
            return true;
        }


        protected override bool CommitInternal()
        {
            return true;
        }
    }
}
