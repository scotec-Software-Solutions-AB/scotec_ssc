#region

using System.Collections.Generic;

#endregion


namespace Scotec.XMLDatabase
{
    public interface IDataRule
    {
        void Run(IDataChangedObject dco);

        IList<string> HandledTypes { get; }
    }
}
