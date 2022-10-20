using System.Collections.Generic;

namespace Scotec.XMLDatabase
{
    public enum EDataChangeType
    {
        Adding,
        Added,
        Modifying,
        Modified,
        Deleted,
        Deleting
    }

    public interface IDataChangedObject
    {
        /// <summary>
        /// The modified data object.
        /// </summary>
        IDataObject DataObject { get; }

        /// <summary>
        /// The type of modification.
        /// </summary>
        EDataChangeType ChangeType { get; }

        ICollection<IDataChangedObject> Attributes { get; }

        IDictionary<IDataObject, IDataChangedObject> ChangedAttributes { get; }
    }


    public interface IDataChangeNotifier
    {
        bool TriggerEnabled { get; set; }
        event OnDataChangeEventHandler OnDataChange;


        void AddChangedObject( IDataObject obj, EDataChangeType type );


        void Lock();


        void Unlock();
    }
}