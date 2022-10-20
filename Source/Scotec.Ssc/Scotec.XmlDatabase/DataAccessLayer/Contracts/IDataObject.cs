#region

using Scotec.XMLDatabase.Attributes;

#endregion


namespace Scotec.XMLDatabase
{
    /// <summary>
    ///     The IDataObject interface is the base interface for all objects representing data
    ///     from an underlying data model. Thus, all objects implementing the IDataAttribute,
    ///     IDataList or IDataRefList must implement the IDataObject interface as well.
    /// </summary>
    public interface IDataObject
    {
        /// <summary>
        ///     The object name.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     The parent property of the data object.
        /// </summary>
        IDataObject Parent { get; }

        /// <summary>
        ///     Checks whether the object contains valid data. Typically data are invalid
        ///     after the object has been deleted but there are still references to the object.
        /// </summary>
        bool DataAvailable { get; }

        /// <summary>
        ///     The owner document.
        /// </summary>
        IDataSession Session { get; }

        /// <summary>
        ///     Returns the associated data factory info object.
        /// </summary>
        IDataFactoryInfo DataFactoryInfo { get; }


        /// <summary>
        ///     Compares the data object with another instance.
        /// </summary>
        /// <param name="obj">The data object to compare with.</param>
        /// <returns>Returns true if both objects are equal, otherwise false.</returns>
        bool IsSameAs( IDataObject obj );


        /// <summary>
        ///     Test whether the name is a valid data object name for a child object.
        /// </summary>
        /// <param name="name">The name of the data object.</param>
        /// <returns>Returns true if the name is a valid child name, otherwise false.</returns>
        bool IsDataObject( string name );


        /// <summary>
        ///     Checks if the data object contains a child object with a given name.
        /// </summary>
        /// <param name="name">The name of the child object.</param>
        /// <returns>
        ///     Returns true if the child object exists, otherwise false.
        ///     If the given name is not a valid child name, a data object exception will be thrown.
        /// </returns>
        bool HasDataObject( string name );


        /// <summary>
        ///     Returns a child object with the given name.
        /// </summary>
        /// <param name="name">The name of the child object.</param>
        /// <returns>
        ///     Returns the child object if it exists, otherwise null.
        ///     If the given name is not a valid child name, a data object exception will be thrown.
        /// </returns>
        IDataObject GetDataObject( string name );


        /// <summary>
        ///     Creates a child object with the given name. If min and max occurence are 1, or
        ///     min is 0 and max is 1 but an object of this type already exists, no new object will be created.
        ///     If the current element is a choice, already existing elements may be deleted.
        /// </summary>
        /// <param name="name">The name of the child object.</param>
        /// <returns>
        ///     Returns the child object if it could be created, otherwise null.
        ///     If the given name is not a valid child name, a data object exception will be thrown.
        /// </returns>
        IDataObject CreateDataObject( string name );


        /// <summary>
        ///     Creates a child object with the given name and type. If min and max occurence are 1, or
        ///     min is 0 and max is 1 but an object of this type already exists, no new object will be created.
        ///     If the current element is a choice, already existing elements may be deleted.
        /// </summary>
        /// <param name="name">The name of the child object.</param>
        /// <param name="type">The type of the child object.</param>
        /// <returns>
        ///     Returns the child object if it could be created, otherwise null.
        ///     If the given name is not a valid child name, a data object exception will be thrown.
        /// </returns>
        IDataObject CreateDataObject( string name, string type );


        /// <summary>
        ///     Deletes the child object with the given name. An exception is thrown is the object connot
        ///     be deleted. This would be if min and max occurs is both 1. This means the element is required.
        /// </summary>
        /// <param name="name"></param>
        void DeleteDataObject( string name );


        /// <summary>
        ///     Sets a reference.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="reference"></param>
        void SetReference( string name, IDataObject reference );


        /// <summary>
        ///     Returns a referenced object.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IDataObject GetReference( string name );


        /// <summary>
        ///     Test whether the name is a valid attribute name for the data object.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        /// <returns>Returns true if the name is a valid attribute name, otherwise false.</returns>
        bool IsAttribute( string name );


        /// <summary>
        ///     Checks if the data object contains an attribute with a given name.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        /// <returns>
        ///     Returns true if the attribute exists, otherwise false.
        ///     If the given name is not a valid attribute name, a data object exception will be thrown.
        /// </returns>
        bool HasAttribute( string name );


        /// <summary>
        ///     Returns an attribute with the given name.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        /// <returns>
        ///     Returns the attribute if it exists, otherwise null.
        ///     If the given name is not a valid attribute name, a data object exception will be thrown.
        /// </returns>
        IDataAttribute GetAttribute( string name );


        /// <summary>
        ///     Creates and returns a attribute with the given name.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        /// <returns>
        ///     Returns the new attribute. If the attribute already exists,
        ///     a data object exception will be thrown.
        /// </returns>
        IDataAttribute CreateAttribute( string name );


        /// <summary>
        ///     Deletes an attribute with the given name.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        /// <returns>
        ///     Returns the new attribute. If the attribute already exists,
        ///     a data object exception will be thrown.
        /// </returns>
        void DeleteAttribute( string name );


        /// <summary>
        ///     Reloads the data.
        ///     <param name="forceNotification">
        ///         If true, objct will be added to the notification list
        ///         even if it hasn't been modified.
        ///     </param>
        /// </summary>
        void Reload( bool forceNotification );
    }
}