#region

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using Scotec.XMLDatabase.Attributes;
using Scotec.XMLDatabase.DAL.DataTypes;

#endregion


namespace Scotec.XMLDatabase.ReaderWriter.Xml.Attributes
{
    internal class XmlDataAttribute : IDataAttribute, IDataObject
    {
        #region Private Member

        private static readonly IDictionary<Type, MethodInfo> s_implicitOperators = new Dictionary<Type, MethodInfo>();
        private readonly string _dataType;

        /// <summary>
        ///   The XML schema description of the attribute
        /// </summary>
        private readonly XmlSchemaAttribute _description;

        /// <summary>
        ///   The parent of the attribute.
        /// </summary>
        private readonly XmlDataObject _parentDataObject;

        /// <summary>
        ///   The XML attribute node containing the XML data.
        /// </summary>
        private readonly XmlAttribute _xmlAttribute;

        private DataType _data;
        private bool _initializing;

        #endregion Private Member


        #region Contructors

        /// <summary>
        ///   The constructor for the attribute object.
        /// </summary>
        /// <param name = "parentDataObject">The parent data object.</param>
        /// <param name = "attribute">The XML attribute node.</param>
        /// <param name = "description">The XML schema description.</param>
        /// <param name="dataType"></param>
        /// <param name="initialize"></param>
        public XmlDataAttribute(
                XmlDataObject parentDataObject, XmlAttribute attribute, string dataType, XmlSchemaAttribute description,
                bool initialize )
        {
            _parentDataObject = parentDataObject;
            _dataType = dataType;
            _xmlAttribute = attribute;
            _description = description;

            if( initialize )
                InitializeData();
            else
            {
                var data = DataType.CreateDataType( _dataType );

                // Id and IdRef cannot be saved as simply Guid in XML. Both must start with an character.
                // Thus add the leading string "ID".
                var attributeValue = (data is Id || data is Idref)
                                             ? _xmlAttribute.Value.Substring( 2 )
                                             : _xmlAttribute.Value;

                MethodInfo info;
                if( s_implicitOperators.TryGetValue( data.GetType(), out info ) == false )
                {
                    info = data.GetType().GetMethod( "op_Implicit", new[] { typeof( string ) } );
                    s_implicitOperators.Add( data.GetType(), info );
                }

                _data = (DataType)info.Invoke( data, new object[] { attributeValue } );
            }
        }

        #endregion Contructors


        /// <summary>
        ///   The attribute's parent.
        /// </summary>
        internal IDataObject InternalParent
        {
            get { return _parentDataObject; }
        }


        #region IDataAttribute Members

        public IDataObject Parent
        {
            get
            {
                if( !DataAvailable )
                    return null;
                return InternalParent;
            }
        }

        /// <summary>
        ///   The attribute's value.
        /// </summary>
        public virtual object Value
        {
            get
            {
                string attributeValue;

                if( _data is Id || _data is Idref )
                {
                    // Id and IdRef cannot be saved as simply Guid in XML. Both must start with an character.
                    // Thus remove the leading string "ID".
                    attributeValue = Attribute.Value.Substring( 2 );
                }
                else
                    attributeValue = Attribute.Value;

                if( _data.ToString() != attributeValue )
                {
                    MethodInfo info;
                    if( s_implicitOperators.TryGetValue( _data.GetType(), out info ) == false )
                    {
                        info = _data.GetType().GetMethod( "op_Implicit", new[] { typeof( string ) } );
                        s_implicitOperators.Add( _data.GetType(), info );
                    }

                    _data = (DataType)info.Invoke( _data, new object[] { attributeValue } );
                }

                return _data;
            }
            set
            {
                _data = value as DataType;
                if( _data != null )
                {
                    // Id and IdRef cannot be saved as simply Guid in XML. Both must start with an character.
                    // Thus add the leading string "ID".
                    var attributeValue = (_data is Id || _data is Idref)
                                                 ? ("ID" + (_data))
                                                 : (_data).ToString();

                    ((XmlDataObject)InternalParent).Document.DataObjectFactory.SetAttributeValue( Attribute
                                                                                                  , attributeValue,
                                                                                                  !_initializing );
                }
                else
                {
                    ((XmlDataObject)InternalParent).Document.DataObjectFactory.SetAttributeValue( Attribute, "",
                                                                                                  !_initializing );
                }
            }
        }

        /// <summary>
        ///   The attributes default value specified in the schema.
        /// </summary>
        public virtual object DefaultValue
        {
            get
            {
                var defaultValue = string.Empty;

                if( Description.DefaultValue != null )
                {
                    defaultValue = Description.DefaultValue;
                }
                else
                {
                    var content = Description.AttributeSchemaType.Content as XmlSchemaSimpleTypeRestriction;
                    if( content != null && content.Facets.Count > 0)
                    {
                        var facet = content.Facets[0] as XmlSchemaEnumerationFacet;
                        if( facet != null )
                            defaultValue = facet.Value;
                    }
                    //((System.Xml.Schema.XmlSchemaSimpleTypeRestriction)(Description.AttributeSchemaType.Content)).Facets
                }
                var data = DataType.CreateDataType( _dataType );

                MethodInfo info;
                if( s_implicitOperators.TryGetValue( data.GetType(), out info ) == false )
                {
                    info = data.GetType().GetMethod( "op_Implicit", new[] { typeof( string ) } );
                    s_implicitOperators.Add( data.GetType(), info );
                }

                data = (DataType)info.Invoke( data, new object[] { defaultValue } );

                return data;
            }
        }


        /// <summary>
        ///   Validates the the value against the schema.
        /// </summary>
        /// <param name = "value"></param>
        /// <returns></returns>
        public virtual bool Validate( object value )
        {
            // Get type info used for validation.
            //			XmlSchemaSimpleType simple = (XmlSchemaSimpleType)Description.AttributeType;
            //			XmlSchemaSimpleTypeRestriction restriction = (XmlSchemaSimpleTypeRestriction)simple.Content;
            //XmlSchemaType type = Data.Document.GetSchemaType(restriction.BaseTypeName);

            // The default implementation always returns true.
            return true;
        }

        #endregion


        #region IDataObject Members

        /// <summary>
        ///   The name of the attribute.
        /// </summary>
        public string Name
        {
            get { return _xmlAttribute.Name; }
        }


        public bool DataAvailable
        {
            get
            {
                XmlNode document = Attribute.OwnerDocument;
                XmlNode parent = Attribute.OwnerElement;
                while( parent != null && parent != document )
                    parent = parent.ParentNode;

                return (parent != null);
            }
        }

        IDataFactoryInfo IDataObject.DataFactoryInfo
        {
            get { return DataFactoryInfo; }
        }

        IDataSession IDataObject.Session
        {
            get { return Parent.Session; }
        }


        bool IDataObject.IsAttribute( string name )
        {
            return false;
        }


        bool IDataObject.HasAttribute( string name )
        {
            return false;
        }


        IDataAttribute IDataObject.GetAttribute( string name )
        {
            throw new Exception( "An attribute does not contain attributes." );
        }


        IDataAttribute IDataObject.CreateAttribute( string name )
        {
            throw new Exception( "An attribute does not contain attributes." );
        }


        void IDataObject.DeleteAttribute( string name )
        {
            throw new Exception( "An attribute does not contain attributes." );
        }


        bool IDataObject.IsDataObject( string name )
        {
            return false;
        }


        bool IDataObject.HasDataObject( string name )
        {
            return false;
        }


        IDataObject IDataObject.GetDataObject( string name )
        {
            throw new Exception( "An attribute does not contain data objects." );
        }


        IDataObject IDataObject.CreateDataObject( string name )
        {
            throw new Exception( "An attribute does not contain data objects." );
        }


        IDataObject IDataObject.CreateDataObject( string name, string type )
        {
            throw new Exception( "An attribute does not contain data objects." );
        }


        void IDataObject.DeleteDataObject( string name )
        {
            throw new Exception( "An attribute does not contain data objects." );
        }


        void IDataObject.SetReference( string name, IDataObject reference )
        {
            throw new Exception( "An attribute does not contain a references to data objects." );
        }


        IDataObject IDataObject.GetReference( string name )
        {
            throw new Exception( "An attribute does not contain a references to data objects." );
        }


        //IDataDocument IDataObject.Document
        //{
        //    get { return InternalParent.Document; }
        //}

        /// <summary>
        ///   Tests if the attribute is the same as a given attribute. 
        ///   Just comparing two objects is not enough because two instances
        ///   of an attribute can refere to the same data.
        /// </summary>
        /// <param name = "obj"></param>
        /// <returns></returns>
        bool IDataObject.IsSameAs( IDataObject obj )
        {
            if( !(obj is XmlDataAttribute) )
                return false;

            return (Attribute == ((XmlDataAttribute)obj).Attribute);
        }


        void IDataObject.Reload( bool forceNotification )
        {
            // Xml objects do not need to be reloaded.
            // Do nothing.
        }

        #endregion


        #region Properties

        public XmlDataObject Data
        {
            get { return _parentDataObject; }
        }

        public XmlAttribute Attribute
        {
            get { return _xmlAttribute; }
        }

        public XmlSchemaAttribute Description
        {
            get { return _description; }
        }

        public IDataFactoryInfo DataFactoryInfo { get; set; }

        #endregion Properties


        #region XmlDataAttribute Implementation

        public XmlSchemaType SchemaType
        {
            get { return _parentDataObject.Document.GetSchemaType( Description.SchemaTypeName ); }
        }


        protected void InitializeData()
        {
            _initializing = true;

            Value = DefaultValue;

            OnInitializeData();

            _initializing = false;
        }


        protected virtual void OnInitializeData()
        {
        }

        #endregion XmlDataAttribute Implementation
    }
}
