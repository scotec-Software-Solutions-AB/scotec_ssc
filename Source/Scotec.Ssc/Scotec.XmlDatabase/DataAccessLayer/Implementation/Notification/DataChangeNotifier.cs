#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scotec.XMLDatabase.Attributes;

#endregion


namespace Scotec.XMLDatabase.Notification
{
    internal class DataChangeNotifier : IDataChangeNotifier
    {
        #region Private Attributes

        private readonly Stack<Dictionary<IDataObject, IDataChangedObject>> _changeLists = new Stack<Dictionary<IDataObject, IDataChangedObject>>();
        private readonly IDataSession _session;
        private Dictionary<IDataObject, IDataChangedObject> _globalChangeList = new Dictionary<IDataObject, IDataChangedObject>();
        private readonly Dictionary<IDataObject, IDataObject> _deletedObjectList = new Dictionary<IDataObject, IDataObject>();
        private int _lockCount;

        #endregion Private Attributes


        #region Constructor

        public DataChangeNotifier( IDataSession session )
        {
            _session = session;
        }

        #endregion Constructor


        #region IDataChangeNotifier Members

        public bool TriggerEnabled { get; set; } = true;

        public event OnDataChangeEventHandler OnDataChange;


        public void AddChangedObject( IDataObject obj, EDataChangeType type )
        {
            lock( _globalChangeList )
            {
                // Notification is only enabled if the lock count is atleast 1.
                if( _lockCount > 0 )
                {
                    // Add the changed object to the global change list.
                    switch( type )
                    {
                        case EDataChangeType.Added:
                        case EDataChangeType.Modified:
                        case EDataChangeType.Deleted:
                            AddChangedObject( _globalChangeList, obj, type );
                            break;
                        case EDataChangeType.Deleting:
                            AddDeletedObject( obj );
                            break;
                    }
                    // Add the changed object to the top most list (used for trigger).
                    if( TriggerEnabled )
                        AddChangedObject(_changeLists.Peek(), obj, type );
                }
            }
        }

        private void AddDeletedObject( IDataObject obj )
        {
            if(!_deletedObjectList.ContainsKey( obj ))
                _deletedObjectList.Add( obj, null );
        }

        private void AddChangedObject( IDictionary<IDataObject, IDataChangedObject> dictionary, IDataObject obj, EDataChangeType changeType )
        {
            var attribute = obj as IDataAttribute;

            if(attribute != null)
                AddChangedDataAttribute( dictionary, attribute, changeType );
            else
                AddChangedDataObject( dictionary, obj, changeType );
        }

        private void AddChangedDataObject(IDictionary<IDataObject, IDataChangedObject> dictionary, IDataObject obj, EDataChangeType changeType)
        {
            IDataChangedObject changedObject;
            var isDeleted = _deletedObjectList.ContainsKey( obj );
            
            if (dictionary.TryGetValue(obj, out changedObject))
            {
                var currentType = changedObject.ChangeType;

                switch (currentType)
                {
                    case EDataChangeType.Adding:
                        {
                            throw new Exception("An object cannot be added multiple times within one lock scope.");
                        }
                    case EDataChangeType.Added:
                        {
                            // Object has previously added but now it has been removed.
                            // No notification needed. Remove the object from list.
                            if (changeType == EDataChangeType.Deleted)
                                dictionary.Remove(obj);
                            break;
                        }
                    case EDataChangeType.Modifying:
                        {
                            //  This a spacial case. If a list contains elements marked as 
                            //  "modifying", the list won't contain any elements with other state
                            // than "modifying".
                            // Objects marked as "modifying" are used by rules only.
                            throw new Exception("An object cannot be modified multiple times within one lock scope.");
                        }
                    case EDataChangeType.Modified:
                        {
                            if (changeType == EDataChangeType.Deleted)
                                dictionary[obj] = new DataChangedObject(obj, changeType);
                            break;
                        }
                    case EDataChangeType.Deleting:
                        {
                            //  This a spacial case. If a list contains elements marked as 
                            //  "deleting", the list won't contain any elements with other state
                            // than "deleting".
                            // Objects marked as "deleting" are used by rules only.
                            throw new Exception("An object cannot be deleted twice.");
                        }
                    case EDataChangeType.Deleted:
                        {
                            if( changeType == EDataChangeType.Added )
                            {
                                //m_objects[obj] = new DataChangedObject(obj, EDataChangeType.Modified);
                                dictionary.Remove( obj );
                                _deletedObjectList.Remove( obj );
                            }
                            //else
                            //    throw new Exception("A delete object cannot be modified or deleted again.");
                            break;
                        }
                }
            }
            else 
            {
                if(!isDeleted || changeType == EDataChangeType.Deleting || changeType == EDataChangeType.Deleted || changeType == EDataChangeType.Added )
                    dictionary[obj] = new DataChangedObject(obj, changeType);
            }
        }

        private void AddChangedDataAttribute(IDictionary<IDataObject, IDataChangedObject> dictionary, IDataAttribute attribute, EDataChangeType changeType)
        {
            IDataChangedObject changedObject;
            
            // There may be no parent in the dictionary if we are in the "modifying" state.
            // This typically happens, if the dictionary is the global change list.
            if(dictionary.TryGetValue(attribute.Parent, out changedObject))
                AddChangedDataObject(changedObject.ChangedAttributes, (IDataObject)attribute, changeType);

        }


        public void Lock()
        {
            lock( _globalChangeList )
            {
                _lockCount++;
                if( TriggerEnabled )
                    _changeLists.Push(new Dictionary<IDataObject, IDataChangedObject>());
            }
        }


        public void Unlock()
        {
            lock( _globalChangeList )
            {
                try
                {
                    // Release the top most data change list and call the trigger.
                    // The trigger calls lock on the change notifier before running any rules.
                    if( TriggerEnabled )
                    {
                        var changeList = _changeLists.Pop();
                        if( changeList.Count > 0 )
                        {
                            Lock();
                            try
                            {
                                var triggerEnabled = TriggerEnabled;
                                if( changeList.Any( obj => obj.Value.ChangeType == EDataChangeType.Adding ) )
                                    TriggerEnabled = false;
                                (_session as IDataRuleTrigger).RunRules( changeList );
                                TriggerEnabled = triggerEnabled;
                            }
                            catch( Exception e )
                            {
                                throw new Exception( "An error occured in a data rule.", e );
                            }
                            finally
                            {
                                Unlock();
                            }
                        }
                    }
                }
                finally
                {
                    // The lock count must be decreased after the trigger has run.
                    // Otherwise the lockcount would go to zero twice (or more) and the 
                    // notification would be called multiple times.
                    --_lockCount;
                }

                // Do the overall notification if lock count is 0 and the list contains
                // at least one element.
                if( _lockCount == 0 && (_globalChangeList).Count > 0 )
                {
                    try
                    {
                        _deletedObjectList.Clear();
                        
                        var changeList = _globalChangeList;
                        _globalChangeList = new Dictionary<IDataObject, IDataChangedObject>();
                        if( OnDataChange != null )
                            OnDataChange( _session, changeList );
                    }
                    catch( Exception e )
                    {
                        throw new Exception( "An error occured in a data change listener.", e );
                    }
                }
            }
        }

        #endregion
    }
}
