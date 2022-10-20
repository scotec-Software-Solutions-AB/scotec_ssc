using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scotec.XMLDatabase
{
    public interface IBusinessDocumentFactory
    {
        IBusinessDocument GetNewDocument(Uri schema);

        IBusinessDocument GetNewDocument(Uri schema, string root);
    
    }
}
