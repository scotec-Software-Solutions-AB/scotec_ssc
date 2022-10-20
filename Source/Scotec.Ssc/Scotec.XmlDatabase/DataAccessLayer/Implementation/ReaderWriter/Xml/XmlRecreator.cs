#region

using System.Collections.Generic;
using System.Xml;

#endregion


namespace Scotec.XMLDatabase.ReaderWriter.Xml
{
    internal sealed class XmlRecreator
    {
        private readonly Stack<XmlRecreateObject> _recreateCache = new Stack<XmlRecreateObject>();


        internal void AddNode( string type, XmlNode node )
        {
            _recreateCache.Push( new XmlRecreateObject( type, node ) );
        }


        internal XmlNode RecreateNode( string type )
        {
            if( _recreateCache.Count > 0 )
            {
                var recreateObject = _recreateCache.Pop();
                if( recreateObject.Type == type )
                    return recreateObject.Node;

                // Remove all further objects. They cannot be reused if the 
                // application tries to create an object of a different type.
                _recreateCache.Clear();
            }

            return null;
        }


        #region Nested type: XmlRecreateObject

        private class XmlRecreateObject
        {
            public XmlRecreateObject( string type, XmlNode node )
            {
                Type = type;
                Node = node;
            }


            internal string Type { get; private set; }

            internal XmlNode Node { get; private set; }
        }

        #endregion
    }
}
