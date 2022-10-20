using System.Collections.Generic;

namespace Scotec.XMLDatabase.Notification
{
    /// <summary>
    ///   Summary description for DataChangedObject.
    /// </summary>
    public class DataChangedObject : IDataChangedObject
    {
        private readonly EDataChangeType _changeType;
        private readonly IDataObject _dataObject;
        private readonly IDictionary<IDataObject, IDataChangedObject> _changedAttributes;


        #region Contructor

        public DataChangedObject( IDataObject dataObject, EDataChangeType changeType )
        {
            _dataObject = dataObject;
            _changeType = changeType;
            _changedAttributes = new Dictionary<IDataObject, IDataChangedObject>();
        }

        #endregion Contructor


        #region IDataChangedObject Members

        IDataObject IDataChangedObject.DataObject
        {
            get { return _dataObject; }
        }

        EDataChangeType IDataChangedObject.ChangeType
        {
            get { return _changeType; }
        }

        public ICollection<IDataChangedObject> Attributes { get { return _changedAttributes.Values; } }

        public IDictionary<IDataObject, IDataChangedObject> ChangedAttributes { get { return _changedAttributes; } }

        #endregion
    }
}
