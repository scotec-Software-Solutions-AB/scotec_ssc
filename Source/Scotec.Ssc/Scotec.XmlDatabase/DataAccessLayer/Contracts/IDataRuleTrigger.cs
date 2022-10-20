using System.Collections.Generic;

namespace Scotec.XMLDatabase
{
    public interface IDataRuleTrigger
    {
        void RegisterRule( IDataRule rule );


        void UnregisterRule( IDataRule rule );


        void RunRules( IDictionary<IDataObject, IDataChangedObject> changedObjects );
    }
}
