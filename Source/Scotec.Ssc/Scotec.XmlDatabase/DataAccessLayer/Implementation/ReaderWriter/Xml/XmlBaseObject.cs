#region

using System.Collections.Specialized;
using System.Xml;

#endregion


namespace Scotec.XMLDatabase.ReaderWriter.Xml
{
    /// <summary>
    ///   Summary description for XmlBaseObject.
    /// </summary>
    internal abstract class XmlBaseObject
    {
        public abstract void Initialize(
                XmlDataDocument document, XmlElement element,
                XmlQualifiedName typeName, XmlNode parent, ListDictionary attributes );
    }
}
