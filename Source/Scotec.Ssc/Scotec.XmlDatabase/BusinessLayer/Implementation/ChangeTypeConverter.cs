#region

using Scotec.XMLDatabase.ChangeNotification;

#endregion


namespace Scotec.XMLDatabase
{
    internal static class ChangeTypeConverter
    {
        public static EChangeNotificationType Convert( EDataChangeType type )
        {
            switch( type )
            {
                case EDataChangeType.Adding:
                    return EChangeNotificationType.Adding;
                case EDataChangeType.Added:
                    return EChangeNotificationType.Added;
                case EDataChangeType.Deleted:
                    return EChangeNotificationType.Deleted;
                case EDataChangeType.Deleting:
                    return EChangeNotificationType.Deleting;
                case EDataChangeType.Modified:
                    return EChangeNotificationType.Modified;
                case EDataChangeType.Modifying:
                    return EChangeNotificationType.Modifying;
            }
            throw new BusinessException( EBusinessError.Document, "Invalid enum value." );
        }


        public static EDataChangeType Convert( EChangeNotificationType type )
        {
            switch( type )
            {
                case EChangeNotificationType.Adding:
                    return EDataChangeType.Adding;
                case EChangeNotificationType.Added:
                    return EDataChangeType.Added;
                case EChangeNotificationType.Deleted:
                    return EDataChangeType.Deleted;
                case EChangeNotificationType.Deleting:
                    return EDataChangeType.Deleting;
                case EChangeNotificationType.Modified:
                    return EDataChangeType.Modified;
                case EChangeNotificationType.Modifying:
                    return EDataChangeType.Modifying;
            }
            throw new BusinessException( EBusinessError.Document, "Invalid enum value." );
        }
    }
}
