#region

using System;
using Scotec.XMLDatabase.Attributes;

#endregion


namespace Scotec.XMLDatabase
{
    public abstract class BusinessAttribute : BusinessObject, IBusinessAttribute
    {
        #region IBusinessAttribute Members

        public abstract object DefaultValue { get; }

        public IBusinessObject Owner
        {
            get
            {
                try
                {
                    return BusinessSession.Factory.GetBusinessObject( DataAttribute.Parent );
                }
                catch( DataException e )
                {
                    throw new BusinessException( (EBusinessError)e.DataError, e.Message, e );
                }
                catch( Exception e )
                {
                    throw new BusinessException( EBusinessError.Document, "Caught unhandled exception.", e );
                }
            }
        }


        public abstract bool Validate( object value );


        public abstract object Value { get; set; }

        #endregion


        #region Protected Properties

        public IDataAttribute DataAttribute
        {
            get { return (DataObject as IDataAttribute); }
        }

        #endregion Protected Properties
    }
}
