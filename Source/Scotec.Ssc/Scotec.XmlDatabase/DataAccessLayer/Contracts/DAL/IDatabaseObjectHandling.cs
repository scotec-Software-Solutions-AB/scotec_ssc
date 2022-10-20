#region

using System.Xml.Schema;

#endregion


namespace Scotec.XMLDatabase.DAL
{
    public interface IDatabaseObjectHandling
    {
        /// <summary>
        ///   Creates a new attribute. The parent object will be added to the notification list.
        /// </summary>
        /// <param name = "parent">The data object containing the attribute.</param>
        /// <param name = "parentType">The new attribute's name.</param>
        /// <param name="name"></param>
        IDatabaseAttribute CreateAttribute( IDatabaseObject parent, XmlSchemaType parentType, string name );


        /// <summary>
        ///   Changes the attibute value and adds the modified attribute to the notification list.
        /// </summary>
        /// <param name = "attribute"></param>
        /// <param name = "data"></param>
        void ModifyAttribute( IDatabaseAttribute attribute, object data );


        /// <summary>
        ///   Removes an attribute from a data object. In Xml it will be removed from the DOM, in databases,
        ///   the field will be set to null.
        /// </summary>
        /// <param name = "parent">The parent.</param>
        /// <param name = "name">The name of the attribute to delete.</param>
        void DeleteAttribute( IDatabaseObject parent, string name );


        /// <summary>
        ///   Creates a new child object of the given type.
        /// </summary>
        /// <param name = "parent">The parent might be either an IDatabaseObject or an IDatabaseList.</param>
        /// <param name = "type">The type of the new child object.</param>
        /// <returns></returns>
        IDatabaseObject CreateObject( IDatabaseObject parent, string type );


        /// <summary>
        ///   Creates a new child object of the given type at the specified position.
        /// </summary>
        /// <param name = "parent">The parent might be either an IDatabaseObject or an IDatabaseList.</param>
        /// <param name = "type">The type of the new child object.</param>
        /// <param name = "index">The position of the new object. The index might be ignored when using databases.</param>
        /// <returns></returns>
        IDatabaseObject CreateObjectAt( IDatabaseObject parent, int index, string type );


        /// <summary>
        ///   Sets a new reference
        /// </summary>
        /// <param name = "dataObject">The referencing database object.</param>
        /// <param name = "reference">The new referenced object.</param>
        /// <param name = "name">Name of the reference.</param>
        void SetReference( IDatabaseObject dataObject, IDatabaseObject reference, string name );


        /// <summary>
        ///   Append a new object reference to a reference list.
        /// </summary>
        /// <param name = "refList"></param>
        /// <param name = "reference"></param>
        /// <returns></returns>
        int AppendReference( IDatabaseRefList refList, IDatabaseObject reference );


        /// <summary>
        ///   Inserts a reference object at the given position.
        /// </summary>
        /// <param name = "refList"></param>
        /// <param name="reference"></param>
        /// <param name = "index"></param>
        /// <returns></returns>
        int InsertReferenceAt( IDatabaseRefList refList, IDatabaseObject reference, int index );


        /// <summary>
        ///   Removes a reference at the given index.
        /// </summary>
        /// <param name = "refList"></param>
        /// <param name = "index"></param>
        /// <returns></returns>
        int RemoveReferenceAt( IDatabaseRefList refList, int index );


        /// <summary>
        ///   Removes all references from a reference list.
        /// </summary>
        /// <param name = "refList"></param>
        void RemoveAllReferences( IDatabaseRefList refList );


        /// <summary>
        ///   Reloads a database object
        /// </summary>
        /// <param name = "databaseObject"></param>
        /// <param name="forceNotification"></param>
        /// <returns></returns>
        IDatabaseObject Reload( IDatabaseObject databaseObject, bool forceNotification );


        /// <summary>
        ///   Removes the object at the specified index from the the list.
        /// </summary>
        /// <param name = "list"></param>
        /// <param name = "index"></param>
        /// <returns></returns>
        int DeleteAt( IDatabaseList list, int index );


        /// <summary>
        ///   Deletes an object.
        /// </summary>
        /// <param name = "data">The object to delete.</param>
        void DeleteObject( IDatabaseObject data );
    }
}
