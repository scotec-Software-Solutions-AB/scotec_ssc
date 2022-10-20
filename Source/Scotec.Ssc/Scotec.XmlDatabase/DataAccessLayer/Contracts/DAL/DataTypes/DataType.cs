#region

using System;
using System.Collections.Generic;
using System.Reflection;

#endregion


namespace Scotec.XMLDatabase.DAL.DataTypes
{
    public abstract class DataType
    {
        private static readonly IDictionary<string, ConstructorInfo> s_constructors =
                new Dictionary<string, ConstructorInfo>();


        public static DataType CreateDataType( string type )
        {
            ConstructorInfo info;

            if( s_constructors.TryGetValue( type, out info ) == false )
            {
                info = Type.GetType( type ).GetConstructor( new Type[] { } );
                s_constructors.Add( type, info );
            }
            return info.Invoke( null ) as DataType;
        }
    }

    public abstract class DataType<B, T> : DataType where B : class, new()
    {
        protected virtual T Value { get; set; }


        protected static T ConvertToValue( B value )
        {
            if( value == null )
                return default(T);

            return (value as DataType<B, T>).Value;
        }


        protected static B ConvertToType( T value )
        {
            if( value == null )
                return null;

            var b = new B() as DataType<B, T>;
            b.Value = value;

            return b as B;
        }
    }
}
