#region

using System.Xml;

#endregion


namespace Scotec.XMLDatabase.ReaderWriter.Transactions
{
    /// <summary>
    ///   Summary description for XmlTransactionDataAdded.
    /// </summary>
    internal class XmlTransactionAttributeDeleted : XmlTransactionParticle
    {
        private int _index = -1;

        public XmlElement Parent { get; set; }

        public XmlAttribute Attribute { get; set; }

//		public XmlSchemaAttribute Description
//		{
//			get{return m_description;}
//			set{m_description = value;}
//		}

        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }


        protected override bool RollbackInternal()
        {
            Document.DataObjectFactory.InternalAddAttribute( Parent, Attribute, Index, false );
            Document.DataObjectFactory.AddAddedAttributeToChangeNotifier( Attribute );

            return true;
        }


        protected override bool CommitInternal()
        {
            return true;
        }
    }
}
