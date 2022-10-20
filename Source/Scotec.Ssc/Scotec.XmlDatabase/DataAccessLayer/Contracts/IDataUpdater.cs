#region

using System;
using System.Xml;

#endregion


namespace Scotec.XMLDatabase
{
    public interface IDataUpdater
    {
        Version Version { get; }


        bool Update( XmlDocument xmlDoc, Version xmlDocVersion, Version schemaVersion );
    }
}
