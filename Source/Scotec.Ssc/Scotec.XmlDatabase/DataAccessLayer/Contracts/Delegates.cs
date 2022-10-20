#region

using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;

#endregion


namespace Scotec.XMLDatabase
{
    public delegate void OnDataChangeEventHandler( IDataSession session, IDictionary<IDataObject, IDataChangedObject> changedObjects );

    public delegate void OnDirtyDataEventHandler( IDataDocument document );

    public delegate void OnDirtyDataSessionEventHandler( IDataSession session );

    public delegate void OnCloseDataSessionEventHandler( IDataSession session );

    public delegate void OnBevorDataUpdateEventHandler( XmlDocument xmlDoc, CancelEventArgs cancelArg );

    public delegate void OnAfterDataUpdateEventHandler( IDataDocument xmlDoc );
}
