#region

using System;
using System.Collections;
using System.Collections.Generic;
using Scotec.XMLDatabase.Attributes;

#endregion


namespace Scotec.XMLDatabase.Rules
{
    internal class DataRuleTrigger : IDataRuleTrigger
    {
        #region Private Attributes

        private readonly IDictionary<string, RuleContainer> _containers = new Dictionary<string, RuleContainer>();

        #endregion Private Attributes


        #region Constructor

        #endregion Constructor


        #region IDataRuleTrigger Members

        void IDataRuleTrigger.RegisterRule( IDataRule rule )
        {
            foreach( var s in rule.HandledTypes )
            {
                RuleContainer container;
                if( _containers.TryGetValue( s, out container ) == false )
                {
                    container = new RuleContainer();
                    _containers.Add( s, container );
                }
                container.AddRule( rule );
            }
        }


        void IDataRuleTrigger.UnregisterRule( IDataRule rule )
        {
            foreach( var s in rule.HandledTypes )
            {
                RuleContainer container;
                if( _containers.TryGetValue( s, out container ) )
                {
                    container.RemoveRule( rule );
                    if( container.IsEmpty )
                        _containers.Remove( s );
                }
            }
        }


        void IDataRuleTrigger.RunRules( IDictionary<IDataObject, IDataChangedObject> changedObjects )
        {
            foreach (var dco in changedObjects.Values)
                RunRules(dco);
        }

        #endregion


        private void RunRules( IDataChangedObject dco )
        {
            var data = dco.DataObject;

            // In some cases, an object with state added or modified has deen deleted in another rule that run before.
            // One case could be, that an object or attribute has been created together with its parent and the parent has been deleted immediately after the creation.
            // In this case the child could still have change type "added" but no parent.
            // The easiest solution is to ignore such objects. However, I have to find a better way.
            var changeType = dco.ChangeType;
            if( data.Parent == null && changeType != EDataChangeType.Deleted )
                return;


            var schemaTypeName = (string)data.DataFactoryInfo["Schema.TypeName"];
            // Remove the trailing string "Type" from the name.
            schemaTypeName = schemaTypeName.Substring( 0, schemaTypeName.Length - 4 );

            // All generated implementation projects have the extension ".BLE". 
            //var qualifiedName = schemaTypeName + "," + (string)data.DataFactoryInfo["Schema.Name"] + ".BLE";
            var qualifiedName = schemaTypeName + "," + (string)data.DataFactoryInfo["Schema.Name"];

            var type = Type.GetType( qualifiedName, true );

            while( type != null )
            {
                // Run the container for the current type and run its rules.
                RuleContainer container;
                if( _containers.TryGetValue( type.FullName, out container ) )
                {
                        container.RunRules( dco );
                }

                type = type.BaseType;
            }
        }
    }
}
