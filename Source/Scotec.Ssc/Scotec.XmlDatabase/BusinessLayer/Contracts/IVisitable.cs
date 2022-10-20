namespace Scotec.XMLDatabase
{
    /// <summary>
    ///     Summary description for IVisitable.
    /// </summary>
    public interface IVisitable
    {
        T Apply<T>( IVisitor<T> visitor );
    }
}