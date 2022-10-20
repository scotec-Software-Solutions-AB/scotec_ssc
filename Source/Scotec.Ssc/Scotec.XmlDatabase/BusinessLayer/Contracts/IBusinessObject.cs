using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Scotec.XMLDatabase
{
    /// <summary>
    ///   Typically the implementation of the IBusinessObject interface encapsulates
    ///   a data object (IDataObject) and would forward most of the calls to that object.
    /// </summary>
    public interface IBusinessObject : INotifyPropertyChanged
    {
        /// <summary>
        ///   The object name.
        /// </summary>
        string ObjectName { get; }

        /// <summary>
        ///   The parent property of the data object. Returns the direct parent.
        /// </summary>
        IBusinessObject Parent { get; }


        /// <summary>
        /// Gets a list of business objects referencing the current business object and matching one of the given types.
        /// </summary>
        IList<IBusinessObject> GetReverseLinks(IList<Type> types, ESearchType searchType = ESearchType.Flat);

        /// <summary>
        /// Gets a list of business objects referencing the current business object and matching the given types.
        /// </summary>
        IList<TBusinessObject> GetReverseLinks<TBusinessObject>(ESearchType searchType = ESearchType.Flat);

        /// Gets a business objects referencing the current business object and matching the given types.
        /// If the are more than one references, the mothod throws a BusinessException. The method return null, if no reference exists.
        TBusinessObject GetReverseLink<TBusinessObject>(ESearchType searchType = ESearchType.Flat);


        /// <summary>
        /// Gets a unique key for the object. The key is valid for the life time of the object only.
        /// Do not save or reuse the key.
        /// </summary>
        Guid Key { get; }


        /// <summary>
        ///   Checks whether the object contains valid data. Typically data are invalid
        ///   after the object has been deleted but there are still references to the object.
        /// </summary>
        bool DataAvailable { get; }


        /// <summary>
        ///   Getter for the owner document.
        /// </summary>
        IBusinessSession Session { get; }


        /// <summary>
        ///   Returns a direct or indirect parent of the given type.
        ///   Returns null, if none of the parents is of the specified type.
        /// </summary>
        T FindParent<T>() where T : IBusinessObject;


        /// <summary>
        ///   Compares the business object with another instance.
        /// </summary>
        /// <param name = "obj">The business object to compare with.</param>
        /// <returns>Returns true if both objects are equal, otherwise false.</returns>
        bool IsSameAs( IBusinessObject obj );


        /// <summary>
        ///   Reloads the data.
        /// </summary>
        void Reload( bool forceNotification );
    }
}
