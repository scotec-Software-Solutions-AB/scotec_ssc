#region

using System.Collections;
using System.Collections.Generic;

#endregion


namespace Scotec.XMLDatabase.ReaderWriter.Xml
{
    internal class XmlDataFactoryInfo : IDataFactoryInfo
    {
        private readonly Dictionary<string, object> _dictionary = new Dictionary<string, object>();


        #region IDataFactoryInfo Members

        public ICollection<string> Keys
        {
            get { return _dictionary.Keys; }
        }

        public ICollection<object> Values
        {
            get { return _dictionary.Values; }
        }


        public object this[ string key ]
        {
            get { return _dictionary[key]; }
            set { _dictionary[key] = value; }
        }


        public int Count
        {
            get { return (_dictionary as ICollection<KeyValuePair<string, object>>).Count; }
        }

        public bool IsReadOnly
        {
            get { return (_dictionary as ICollection<KeyValuePair<string, object>>).IsReadOnly; }
        }


        public void Add( string key, object value )
        {
            _dictionary.Add( key, value );
        }


        public bool ContainsKey( string key )
        {
            return _dictionary.ContainsKey( key );
        }


        public bool Remove( string key )
        {
            return _dictionary.Remove( key );
        }


        public bool TryGetValue( string key, out object value )
        {
            return _dictionary.TryGetValue( key, out value );
        }


        public void Add( KeyValuePair<string, object> item )
        {
            (_dictionary as ICollection<KeyValuePair<string, object>>).Add( item );
        }


        public void Clear()
        {
            (_dictionary as ICollection<KeyValuePair<string, object>>).Clear();
        }


        public bool Contains( KeyValuePair<string, object> item )
        {
            return (_dictionary as ICollection<KeyValuePair<string, object>>).Contains( item );
        }


        public void CopyTo( KeyValuePair<string, object>[] array, int arrayIndex )
        {
            (_dictionary as ICollection<KeyValuePair<string, object>>).CopyTo( array, arrayIndex );
        }


        public bool Remove( KeyValuePair<string, object> item )
        {
            return (_dictionary as ICollection<KeyValuePair<string, object>>).Remove( item );
        }


        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return (_dictionary as IEnumerable<KeyValuePair<string, object>>).GetEnumerator();
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return (_dictionary as IEnumerable).GetEnumerator();
        }

        #endregion
    }
}
