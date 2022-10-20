namespace Scotec.XMLDatabase.DAL
{
    public interface IDatabaseList
    {
        int Count { get; }


        /// <summary>
        ///   Returns the element at the given position.
        /// </summary>
        IDatabaseObject this[ int index ] { get; }


        /// <summary>
        ///   The index of a given object.
        /// </summary>
        /// <param name = "databaseObject"></param>
        /// <returns>The index of the object. -1 if the object is not in the list.</returns>
        int IndexOf( IDatabaseObject databaseObject );


        /// <summary>
        ///   Creates a new element at the end of the list.
        /// </summary>
        /// <returns>The new element.</returns>
        IDatabaseObject CreateDatabaseObject( string type );


        /// <summary>
        ///   Creates a new element at the given position. All further elements
        ///   will be moved to the next position.
        /// </summary>
        /// <param name = "type"></param>
        /// <param name = "index"></param>
        /// <returns>The new element.</returns>
        IDatabaseObject CreateDatabaseObjectAt( int index, string type );


        /// <summary>
        ///   Appends a element at the end of the list. The list creates a deep copy
        ///   of the element.
        /// </summary>
        /// <param name = "databaseObject"></param>
        /// <returns>The new element.</returns>
        IDatabaseObject Append( IDatabaseObject databaseObject );


        /// <summary>
        ///   Inserts a element at the given position. The list creates a deep copy and
        ///   returns the new element.
        /// </summary>
        /// <param name = "index">The position of the new object.</param>
        /// <param name = "databaseObject">The object to insert.</param>
        /// <returns>The new element.</returns>
        IDatabaseObject InsertAt( int index, IDatabaseObject databaseObject );


        /// <summary>
        ///   Moves an object from a source position to the target position.
        /// </summary>
        /// <param name = "from">Source position</param>
        /// <param name = "to">Target position</param>
        void Move( int from, int to );


        /// <summary>
        ///   Moves an object to the specified position.
        /// </summary>
        /// <param name = "index"></param>
        /// <param name = "databaseObject"></param>
        void MoveTo( int index, IDatabaseObject databaseObject );


        /// <summary>
        ///   Moves an object after the specified target object.
        /// </summary>
        /// <param name = "sourceObject">The object to move.</param>
        /// <param name = "targetObject">The object after which the new object shall be moved.</param>
        void MoveAfter( IDatabaseObject sourceObject, IDatabaseObject targetObject );


        /// <summary>
        ///   Deletes the element at the given position.
        /// </summary>
        /// <param name = "index">The index of the object to delete</param>
        /// <returns>The number of remaining elements.</returns>
        int DeleteAt( int index );


        /// <summary>
        ///   Deletes all elements.
        /// </summary>
        void DeleteAll();


        /// <summary>
        ///   Tests if the list contains the given object.
        /// </summary>
        /// <param name = "databaseObject"></param>
        /// <returns>True if the list cointains the object, otherwise false</returns>
        bool Contains( IDatabaseObject databaseObject );
    }
}
