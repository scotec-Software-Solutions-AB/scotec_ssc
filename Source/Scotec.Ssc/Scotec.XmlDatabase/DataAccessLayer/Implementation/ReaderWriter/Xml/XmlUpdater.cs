#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

#endregion


namespace Scotec.XMLDatabase.ReaderWriter.Xml
{
    public class XmlUpdater
    {
        public bool NeedsUpdate(XmlDataDocument document)
        {
            return (document.SchemaVersion > document.FileSchemaVersion );
        }


        public void Update(XmlDataDocument document, IList<IDataUpdater> updaters)
        {
            var xmlDocVersion = document.FileSchemaVersion;
            var schemaVersion = document.SchemaVersion;
            try
            {
                UpdateAll( document, updaters.Where( updater => updater.Version > xmlDocVersion).OrderBy(updater => updater.Version), xmlDocVersion, schemaVersion );

                // Set the new version.
                document.FileSchemaVersion = schemaVersion;
            }
            catch( DataException )
            {
                throw;
            }
            catch( Exception e )
            {
                throw new DataException( EDataError.UpdateFailed, "Update failed", e );
            }
        }

        private void UpdateAll(XmlDataDocument document, IEnumerable<IDataUpdater> updaters, Version xmlDocVersion, Version schemaVersion)
        {
            if( updaters.Any( t => !t.Update( document.Document, xmlDocVersion, schemaVersion ) ) )
                throw new DataException( EDataError.UpdateFailed );
        }
    }
}
