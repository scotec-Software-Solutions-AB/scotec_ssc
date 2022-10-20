#region

using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using Scotec.XMLDatabase.ChangeNotification;

#endregion


namespace Scotec.XMLDatabase
{
    public delegate void OnDirtyDocumentEventHandler( IBusinessDocument document );

    public delegate void OnDirtySessionEventHandler( IBusinessSession session );

    public delegate void OnCloseSessionEventHandler( IBusinessSession session );

    public delegate void OnBevorUpdateEventHandler( XmlDocument xmlDoc, CancelEventArgs cancelArg );

    public delegate void OnAfterUpdateEventHandler( IBusinessDocument xmlDoc );
}
