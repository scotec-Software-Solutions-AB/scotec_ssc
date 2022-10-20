namespace Scotec.XMLDatabase.DAL
{
    public interface IDatabaseRefList
    {
        /// <summary>
        ///   The number of references in the list.
        /// </summary>
        int Count { get; }


        /// <summary>
        ///   Returns the element at the given position.
        /// </summary>
        IDatabaseObject this[ int index ] { get; }


        /// <summary>
        ///   The index of a given object.
        /// </summary>
        /// <param name = "dabaseObject"></param>
        /// <returns>The index of the object. -1 if the object is not in the list.</returns>
        int IndexOf( IDatabaseObject dabaseObject );


        /// <summary>
        ///   Appends a reference at the end of the list.
        /// </summary>
        /// <param name = "dabaseObject"></param>
        /// <returns>The new element.</returns>
        int Append( IDatabaseObject dabaseObject );


        /// <summary>
        ///   Inserts a reference at the given position..
        /// </summary>
        /// <param name = "index"></param>
        /// <param name = "dabaseObject"></param>
        /// <returns>The new element.</returns>
        int InsertAt( int index, IDatabaseObject dabaseObject );


        /// <summary>
        ///   Moves an object reference from a source position to the target position.
        /// </summary>
        /// <param name = "from">Source position</param>
        /// <param name = "to">Target position</param>
        void Move( int from, int to );


        /// <summary>
        ///   Moves an object reference to the specified position.
        /// </summary>
        /// <param name = "index"></param>
        /// <param name = "dabaseObject"></param>
        void MoveTo( int index, IDatabaseObject dabaseObject );


        /// <summary>
        ///   Moves an object reference after the specified target object reference.
        /// </summary>
        /// <param name = "sourceObject">The object to move.</param>
        /// <param name = "targetObject">The object after which the new object shall be moved.</param>
        void MoveAfter( IDatabaseObject sourceObject, IDatabaseObject targetObject );


        /// <summary>
        ///   Deletes the reference at the given position.
        /// </summary>
        /// <param name = "index"></param>
        /// <returns>The number of remaining elements.</returns>
        int RemoveAt( int index );


        /// <summary>
        ///   Deletes all references.
        /// </summary>
        void RemoveAll();


        /// <summary>
        ///   Tests if the list contains a reference to the given object.
        /// </summary>
        /// <param name = "dabaseObject"></param>
        /// <returns>True if the list cointains a reference, otherwise false</returns>
        bool Contains( IDatabaseObject dabaseObject );
    }
}
