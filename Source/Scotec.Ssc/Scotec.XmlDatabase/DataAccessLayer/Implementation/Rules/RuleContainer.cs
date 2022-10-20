#region

using System.Collections.Generic;
using Scotec.XMLDatabase.Attributes;

#endregion


namespace Scotec.XMLDatabase.Rules
{
    internal class RuleContainer
    {
        private readonly IDictionary<IDataRule, IDataRule> _rules = new Dictionary<IDataRule, IDataRule>();


        public void AddRule( IDataRule rule )
        {
            _rules[rule] = rule;
        }


        public void RemoveRule( IDataRule rule )
        {
            _rules.Remove( rule );
        }


        public void RunRules(IDataChangedObject dco)
        {
            foreach( var rule in _rules.Values )
                rule.Run( dco );
        }


        public bool IsEmpty
        {
            get { return (_rules.Count == 0); }
        }
    }
}
