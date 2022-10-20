#region

using System.Collections;
using System.Collections.Generic;
using Scotec.XMLDatabase.ChangeNotification;

#endregion


namespace Scotec.XMLDatabase.Rules
{
    public interface IBusinessRule
    {
    }

    public interface IBusinessRule<in TBO> : IBusinessRule where TBO : IBusinessObject
    {
        void Run(TBO businessObject, ICollection<KeyValuePair<IBusinessAttribute, EChangeNotificationType>>  attributes, EChangeNotificationType type);
    }

}
