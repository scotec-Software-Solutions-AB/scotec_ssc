#region

using System;
using System.Collections.Generic;
using System.Linq;
using Scotec.XMLDatabase.ChangeNotification;

#endregion


namespace Scotec.XMLDatabase
{
    public class DataChangedEventArgs : EventArgs
    {
        private readonly IDictionary<IBusinessObject, IChangeNotificationObject> _changes;

        private readonly Dictionary<Type, ICollection<IChangeNotificationObject>> _typedChanges = new Dictionary<Type, ICollection<IChangeNotificationObject>>();

        public DataChangedEventArgs( IBusinessSession session, IDictionary<IBusinessObject, IChangeNotificationObject> changes )
        {
            Session = session;
            _changes = changes;
        }

        public IBusinessSession Session { get; private set; }

        public ICollection<IChangeNotificationObject> GetChanges<TBusinessObjectType>()
        {
            ICollection<IChangeNotificationObject> changes;
            if( !_typedChanges.TryGetValue( typeof( TBusinessObjectType ), out changes ) )
            {
                changes = (from c in _changes.Values
                           where c.BusinessObject is TBusinessObjectType
                           select c).ToList();

                _typedChanges.Add( typeof( TBusinessObjectType ), changes );
            }

            return changes;
        }

        public ICollection<TBusinessObjectType> GetChanges<TBusinessObjectType>( EChangeType changeType )
        {
            return (from c in GetChanges<TBusinessObjectType>()
                    where CompareChangeType( c.ChangeType, changeType )
                    select (TBusinessObjectType)c.BusinessObject).ToList();
        }

        public EChangeType GetChangeState(IBusinessObject bo)
        {
            IChangeNotificationObject changeNotificationObject;

            if(!_changes.TryGetValue( bo, out changeNotificationObject ))
                return EChangeType.None;

            return Convert( changeNotificationObject.ChangeType );
        }

        private static bool CompareChangeType( EChangeNotificationType changeNotificationType, EChangeType changeType )
        {
            return (changeNotificationType == EChangeNotificationType.Added && changeType == EChangeType.Added ||
                    changeNotificationType == EChangeNotificationType.Modified && changeType == EChangeType.Modified ||
                    changeNotificationType == EChangeNotificationType.Deleted && changeType == EChangeType.Deleted);
        }


        private static EChangeType Convert(EChangeNotificationType type)
        {
            switch (type)
            {
                case EChangeNotificationType.Added:
                    return EChangeType.Added;
                case EChangeNotificationType.Modified:
                    return EChangeType.Modified;
                case EChangeNotificationType.Deleted:
                    return EChangeType.Deleted;
            }

            return EChangeType.None;
        }

    }
}