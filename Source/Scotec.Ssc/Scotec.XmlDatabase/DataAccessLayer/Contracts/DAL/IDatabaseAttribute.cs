namespace Scotec.XMLDatabase.DAL
{
    public interface IDatabaseAttribute
    {
        object Value { get; set; }

        IDatabaseObject Parent { get; }


        bool IsSameAs( IDatabaseAttribute attribute );
    }
}
