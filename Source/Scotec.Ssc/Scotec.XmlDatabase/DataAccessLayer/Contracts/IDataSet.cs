#region

using System.Collections;
using System.Collections.Generic;

#endregion


namespace Scotec.XMLDatabase
{
    public interface IDataSet : IEnumerable<IDataObject>
    {
        /// <summary>
        ///   Number of elements in the set.
        /// </summary>
        int Count { get; }


        /// <summary>
        ///   Gets a data object at the given index.
        /// </summary>
        IDataObject this[ int index ] { get; }


        /// <summary>
        ///   Adds a new data object to the set.
        /// </summary>
        /// <param name = "dataObject">The data object to add.</param>
        /// <returns>Returns the number of objects in the list.</returns>
        int Add( IDataObject dataObject );


        /// <summary>
        ///   Removes a data object at the given index.
        /// </summary>
        /// <param name = "index">The index of the object to remove.</param>
        /// <returns>Returns the number of objects in the list.</returns>
        int RemoveAt( int index );


        /// <summary>
        /// </summary>
        /// <param name = "dataSet"></param>
        /// <returns></returns>
        IDataSet Union( IDataSet dataSet );


        /// <summary>
        /// </summary>
        /// <param name = "dataSet"></param>
        /// <returns></returns>
        IDataSet Intersect( IDataSet dataSet );


        /// <summary>
        /// </summary>
        /// <param name = "dataSet"></param>
        /// <returns></returns>
        IDataSet Difference( IDataSet dataSet );


        /// <summary>
        /// </summary>
        /// <param name = "dataSet"></param>
        /// <returns></returns>
        IDataSet SymmetricDifference( IDataSet dataSet );


        /// <summary>
        ///   Tests if the set contains the given object.
        /// </summary>
        /// <param name = "dataObject"></param>
        /// <returns>True if the set cointains the object, otherwise false</returns>
        bool Contains( IDataObject dataObject );
    }
}
