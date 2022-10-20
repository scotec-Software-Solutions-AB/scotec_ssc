using System.Collections.Generic;

namespace Scotec.XMLDatabase.Rules
{
    public interface IBusinessRuleTrigger
    {
        void RegisterRule<TBO>( IBusinessRule<TBO> rule ) where TBO : IBusinessObject;


        void UnregisterRule<TBO>( IBusinessRule<TBO> rule ) where TBO : IBusinessObject;


        void RegisterRules( IEnumerable<IBusinessRule> businessRules );


        void UnregisterAllRules();
    }
}
