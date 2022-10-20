namespace Scotec.XMLDatabase.Attributes
{
    public interface IDataAttribute
    {
        object Value { get; set; }
        object DefaultValue { get; }


        IDataObject Parent { get; }


        bool Validate( object value );
    }
}
