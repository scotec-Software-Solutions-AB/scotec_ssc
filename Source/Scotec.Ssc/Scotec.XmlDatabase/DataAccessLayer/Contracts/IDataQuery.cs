#region

using System.Collections.Generic;

#endregion


namespace Scotec.XMLDatabase
{
    public enum EDataSearchType
    {
        Flat = 0,
        Deep = 1
    }

    public interface IDataQuery
    {
        /// <summary>
        ///   Executes a implementation dependend query, e.g. XPath for XML-Reader/Writer
        /// </summary>
        /// <param name = "query">The query to execute.</param>
        /// <returns>Set of data found.</returns>
        IDataSet Execute( string query );

        IDataSet GetDirectReverseLinks(IList<string> types, EDataSearchType searchType);

    }
}
