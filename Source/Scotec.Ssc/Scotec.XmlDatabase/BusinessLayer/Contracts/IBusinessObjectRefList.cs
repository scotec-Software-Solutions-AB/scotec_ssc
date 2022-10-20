#region

using System.Collections.Generic;

#endregion


namespace Scotec.XMLDatabase
{
    public interface IBusinessObjectRefList<IBusinessObjectType> : IEnumerable<IBusinessObjectType>, IBusinessObject
            where IBusinessObjectType : IBusinessObject
    {
        /// <summary>
        ///   The number of references in the list.
        /// </summary>
        int Count { get; }


        /// <summary>
        ///   Returns the element at the given position.
        /// </summary>
        IBusinessObjectType this[ int index ] { get; }


        /// <summary>
        ///   The index of a given object.
        /// </summary>
        /// <param name = "businessObject"></param>
        /// <returns>The index of the object. -1 if the object is not in the list.</returns>
        int IndexOf( IBusinessObjectType businessObject );


        /// <summary>
        ///   Appends a reference at the end of the list.
        /// </summary>
        /// <param name = "businessObject"></param>
        /// <returns>The new element.</returns>
        int Append( IBusinessObjectType businessObject );


        /// <summary>
        ///   Inserts a reference at the given position..
        /// </summary>
        /// <param name = "index"></param>
        /// <param name = "businessObject"></param>
        /// <returns>The new element.</returns>
        int InsertAt( int index, IBusinessObjectType businessObject );


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
        /// <param name = "businessObject"></param>
        void MoveTo( int index, IBusinessObjectType businessObject );


        /// <summary>
        ///   Moves an object reference after the specified target object reference.
        /// </summary>
        /// <param name = "sourceObject">The object to move.</param>
        /// <param name = "targetObject">The object after which the new object shall be moved.</param>
        void MoveAfter( IBusinessObjectType sourceObject, IBusinessObjectType targetObject );


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
        /// <param name = "businessObject"></param>
        /// <returns>True if the list cointains a reference, otherwise false</returns>
        bool Contains( IBusinessObjectType businessObject );
    }
}
