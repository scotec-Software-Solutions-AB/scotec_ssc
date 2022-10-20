#region

using System.Collections.Generic;

#endregion


namespace Scotec.XMLDatabase
{
    /// <summary>
    ///   A set contains a collection of business objects. By design, a set can hold 
    ///   all types of business objects. Such a set is not typesafe.
    /// </summary>
    public interface IBusinessObjectSet : IEnumerable<IBusinessObject>
    {
        /// <summary>
        ///   Number of elements in the set.
        /// </summary>
        int Count { get; }


        /// <summary>
        ///   Gets a business object at the given index.
        /// </summary>
        IBusinessObject this[ int index ] { get; }


        /// <summary>
        ///   Adds a new business object to the set.
        /// </summary>
        /// <param name = "businessObject">The business object to add.</param>
        /// <returns>Returns the number of objects in the list.</returns>
        int Add( IBusinessObject businessObject );


        /// <summary>
        ///   Removes a business object at the given index.
        /// </summary>
        /// <param name = "index">The index of the object to remove.</param>
        /// <returns>Returns the number of objects in the list.</returns>
        int RemoveAt( int index );


        /// <summary>
        ///   Compbines two sets.
        /// </summary>
        /// <param name = "businessObjectSet"></param>
        /// <returns></returns>
        IBusinessObjectSet Union( IBusinessObjectSet businessObjectSet );


        /// <summary>
        ///   Intersects two sets.
        /// </summary>
        /// <param name = "businessObjectSet"></param>
        /// <returns></returns>
        IBusinessObjectSet Intersect( IBusinessObjectSet businessObjectSet );


        /// <summary>
        ///   Craetes a set containing the difference of two sets.
        /// </summary>
        /// <param name = "businessObjectSet"></param>
        /// <returns></returns>
        IBusinessObjectSet Difference( IBusinessObjectSet businessObjectSet );


        /// <summary>
        ///   Craetes a set containing the symmetric difference of two sets.
        /// </summary>
        /// <param name = "businessObjectSet"></param>
        /// <returns></returns>
        IBusinessObjectSet SymmetricDifference( IBusinessObjectSet businessObjectSet );


        /// <summary>
        ///   Tests if the set contains the given object.
        /// </summary>
        /// <param name = "businessObject"></param>
        /// <returns>True if the set cointains the object, otherwise false</returns>
        bool Contains( IBusinessObject businessObject );
    }
}
