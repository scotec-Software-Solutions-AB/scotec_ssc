#region

using System;
using System.Collections.Generic;
using System.Linq;

#endregion


namespace Scotec.XMLDatabase.Rules
{
    internal class BusinessRuleTrigger : IBusinessRuleTrigger
    {
        #region Private Atributes

        private readonly IBusinessDocument _businessDocument;
        private readonly IDataDocument _dataDocument;
        private readonly IDictionary<Type, IDictionary<IBusinessRule, IDataRule>> _rules = new Dictionary<Type, IDictionary<IBusinessRule, IDataRule>>();

        #endregion Private Attributes


        #region Constructor

        public BusinessRuleTrigger( IBusinessDocument businessDocument, IDataDocument dataDocument )
        {
            _dataDocument = dataDocument;
            _businessDocument = businessDocument;
        }

        #endregion COnstructor


        #region IBusinessRuleTrigger Members

        public void RegisterRule<T>( IBusinessRule<T> rule ) where T : IBusinessObject
        {
            IDataRule dataRule = new Rule<T>( _businessDocument as BusinessDocument, rule );

            IDictionary<IBusinessRule, IDataRule> rules;
            if( !_rules.TryGetValue( typeof( T ), out rules ) )
            {
                rules = new Dictionary<IBusinessRule, IDataRule>();
                _rules.Add( typeof( T ), rules );
            }

            rules.Add( rule, dataRule );

            ((IDataRuleTrigger)_dataDocument).RegisterRule( dataRule );
        }


        public void UnregisterRule<T>( IBusinessRule<T> rule ) where T : IBusinessObject
        {
            IDictionary<IBusinessRule, IDataRule> rules;
            if( !_rules.TryGetValue( typeof( T ), out rules ) )
                return;

            IDataRule dataRule;
            if( !rules.TryGetValue( rule, out dataRule ) )
                return;

            ((IDataRuleTrigger)_dataDocument).UnregisterRule( dataRule );
            
            rules.Remove( rule );
            if(rules.Count == 0 )
                _rules.Remove( typeof( T ) );
        }


        public void RegisterRules( IEnumerable<IBusinessRule> businessRules )
        {
            foreach( var businessRule in businessRules )
            {
                var interfaces = businessRule.GetType().GetInterfaces();
                foreach( var theInterface in interfaces )
                {
                    if( theInterface.Name != "IBusinessRule`1" )
                        continue;

                    var run = theInterface.GetMethod( "Run" );
                    var parameterType = run.GetParameters()[0].ParameterType;

                    var registerRule = GetType().GetMethod( "RegisterRule" );
                    var registerRuleBound = registerRule.MakeGenericMethod( new[] {parameterType} );

                    registerRuleBound.Invoke( this, new[] {businessRule} );
                }
            }
        }


        public void UnregisterAllRules()
        {
            var rules = (from d in _rules.Values
                from r in d.Keys
                select r).ToList();

            foreach( var rule in rules )
            {
                var interfaces = rule.GetType().GetInterfaces();
                foreach( var theInterface in interfaces )
                {
                    // run is null for the base interface (non generic).
                    var run = theInterface.GetMethod( "Run" );
                    if(run == null)
                        continue;
                    var parameterType = run.GetParameters()[0].ParameterType;

                    var unregisterRule = GetType().GetMethod( "UnregisterRule" );
                    var unregisterRuleBound = unregisterRule.MakeGenericMethod( new[] {parameterType} );

                    unregisterRuleBound.Invoke( this, new object[] {rule} );
                }
            }
        }

        #endregion
    }
}