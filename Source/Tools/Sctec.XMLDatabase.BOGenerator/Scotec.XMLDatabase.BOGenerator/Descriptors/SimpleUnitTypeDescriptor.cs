#region

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;

#endregion


namespace Scotec.XMLDatabase.BOGenerator.Descriptors
{
    public class SimpleUnitTypeDescriptor : SimpleTypeDescriptor
    {

        private string TypeCode { get; set; }
        private string FullTypeName { get; set; }

        public SimpleUnitTypeDescriptor( XmlSchemaType schemaType, ProjectDescriptor projectDescriptor )
                : base( schemaType, projectDescriptor )
        {
        }


        //public string ValueType
        //{
        //    get
        //    {
        //        return FullTypeName;
        //    }
        //}


        //public string DataType
        //{
        //    get { return "Scotec.XMLDatabase.DAL.DataTypes." + TypeCode; }
        //}


        public override string EnumType
        {
            get { return TypeName.Substring(1); }
        }

    }
}
