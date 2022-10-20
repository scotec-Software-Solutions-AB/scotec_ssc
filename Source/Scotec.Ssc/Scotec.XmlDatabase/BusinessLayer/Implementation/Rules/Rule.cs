#region

using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Scotec.XMLDatabase.Attributes;
using Scotec.XMLDatabase.ChangeNotification;

#endregion


namespace Scotec.XMLDatabase.Rules
{
    internal class Rule<T> : IDataRule where T : IBusinessObject
    {
        #region Private Attributes

        private readonly BusinessDocument _document;
        private readonly IBusinessRule<T> _businessRule;
        private readonly IList<string> _types = new List<string>();

        #endregion Private Attributes


        #region Constructors

        public Rule( BusinessDocument document, IBusinessRule<T> businessRule )
        {
            _document = document;
            _businessRule = businessRule;

            var type = typeof( T );
            // Ignore the leading 'I' of the interface name.
            _types.Add( type.Namespace + "." + type.Name.Substring( 1 ) );
        }

        #endregion Constructors


        #region IDataRule Members

        IList<string> IDataRule.HandledTypes
        {
            get { return _types; }
        }


        void IDataRule.Run(IDataChangedObject dco)
        {
            T businessObject;
            var dataObject = dco.DataObject;

            var session = (BusinessSession)_document.GetSession( dataObject.Session.Id );

            businessObject = (T)session.Factory.GetBusinessObject( dataObject );
            var attributes = (from a in dco.Attributes
                select
                    new KeyValuePair<IBusinessAttribute, EChangeNotificationType>( session.Factory.GetBusinessAttribute( (IDataAttribute)a.DataObject ),
                        ChangeTypeConverter.Convert( a.ChangeType ) )).ToList();

            _businessRule.Run(businessObject, attributes, ChangeTypeConverter.Convert(dco.ChangeType));
        }



        #endregion
    }
}
