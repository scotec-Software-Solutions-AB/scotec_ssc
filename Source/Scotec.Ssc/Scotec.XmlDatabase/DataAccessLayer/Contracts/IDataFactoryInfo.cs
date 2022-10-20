#region

using System.Collections.Generic;

#endregion


namespace Scotec.XMLDatabase
{
    /// <summary>
    ///   The IDataFactoryInfo interface is used by object factories 
    ///   to create data wrapper, e.g. business objects. Typically, this
    ///   interface is implemented by data objects providing the IDataObject
    ///   interface. The info object contains a set of key/value pairs. Client 
    ///   can add additional data to the info object.
    /// </summary>
    public interface IDataFactoryInfo : IDictionary<string, object>
    {
    }
}
