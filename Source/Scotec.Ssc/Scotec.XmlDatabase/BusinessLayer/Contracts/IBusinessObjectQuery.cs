using System.Collections.Generic;

namespace Scotec.XMLDatabase
{
    public enum ESearchType
    {
        Flat = 0,
        Deep = 1
    }

    public interface IBusinessObjectQuery
    {
        /// <summary>
        ///     Executes a implementation dependend query, e.g. XPath for XML-Reader/Writer
        /// </summary>
        /// <param name="query">The query to execute.</param>
        /// <returns>Set of data found.</returns>
        IList<IBusinessObject> Execute(string query);
    }
}