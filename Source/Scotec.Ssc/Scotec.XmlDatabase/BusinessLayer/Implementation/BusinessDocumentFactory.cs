
namespace Scotec.XMLDatabase
{
    public class BusinessDocumentFactory : IBusinessDocumentFactory
    {
        private readonly Func<string, IBusinessDocument> _businessDocumentFactory;

        public BusinessDocumentFactory(Func<string, IBusinessDocument> factory, string configurationName )
        {
            _businessDocumentFactory = factory;
            ConfigurationName = configurationName;
        }


        private string ConfigurationName { get; set; }


        #region IBusinessDocumentFactory Members

        public IBusinessDocument GetNewDocument( Uri schema )
        {
            return GetNewDocument( schema, null );
        }

        public IBusinessDocument GetNewDocument( Uri schema, string root )
        {
            var document = _businessDocumentFactory(ConfigurationName);

            // May be already set in configuration.
            if ( document.Schema == null )
                document.Schema = schema;

            // May be already set in configuration.
            if( document.Root == null )
                document.Root = root;

            return document;
        }

        #endregion
    }
}
