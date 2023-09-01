using System.Text.RegularExpressions;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;

namespace Scotec.Extensions.Autocad
{
    public static class DatabaseExtensions
    {
        // We only want to retrieve objects of type DBObject or objects derived from DBObject.
        private static readonly RXClass BaseClass = RXObject.GetClass(typeof(DBObject)).MyParent;

        /// <summary>
        ///     Returns the block table of the database.
        /// </summary>
        /// <remarks>
        ///     This extension method uses the top transaction of the current database. Therefore, at least one open transaction
        ///     must exist.
        /// </remarks>
        public static BlockTable GetBlockTable(this Database database, OpenMode openMode = OpenMode.ForRead)
        {
            var transaction = GetTopTransaction(database);

            return (BlockTable)transaction.GetObject(database.BlockTableId, openMode);
        }

        /// <summary>
        ///     Returns the layer table of the database.
        /// </summary>
        /// <remarks>
        ///     This extension method uses the top transaction of the current database. Therefore, at least one open transaction
        ///     must exist.
        /// </remarks>
        public static LayerTable GetLayerTable(this Database database)
        {
            var transaction =GetTopTransaction(database);
            return (LayerTable)transaction.GetObject(database.LayerTableId, OpenMode.ForRead);
        }


        /// <summary>
        ///     Returns the draw order table of the model space of the current database.
        /// </summary>
        /// <remarks>
        ///     This extension method uses the top transaction of the current database. Therefore, at least one open transaction
        ///     must exist.
        /// </remarks>
        public static DrawOrderTable GetDrawOrderTable(this Database database, OpenMode openMode = OpenMode.ForRead)
        {
            var transaction = database.TransactionManager.TopTransaction;
            var modelSpace = GetModelSpace(database);

            return (DrawOrderTable)transaction.GetObject(modelSpace.DrawOrderTableId, openMode);
        }

        /// <summary>
        ///     Returns the model space of the current database.
        /// </summary>
        /// <remarks>
        ///     This extension method uses the top transaction of the current database. Therefore, at least one open transaction
        ///     must exist.
        /// </remarks>
        public static BlockTableRecord GetModelSpace(this Database database, OpenMode openMode = OpenMode.ForRead)
        {
            var transaction = database.TransactionManager.TopTransaction;
            var modelSpace = (BlockTableRecord)transaction.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(database), openMode);

            return modelSpace;
        }

        /// <summary>
        /// </summary>
        /// <param name="database"></param>
        /// <returns></returns>
        public static IEnumerable<ObjectId> GetObjectIds(this Database database)
        {
            var modelSpace = GetModelSpace(database);
            foreach (var objectId in modelSpace)
            {
                yield return objectId;
            }
        }

        /// <summary>
        ///     Enumerates all ObjectIds for objects of the specified type as well as its derived types.
        ///     <code> database.GetObjectIds&lt;Line&gt;()</code> returns all object IDs for line objects.
        ///     <code> database.GetObjectIds&lt;Curve&gt;()</code> returns all object IDs for objects of type Curve and its
        ///     subclasses like Line or Polyline.
        /// </summary>
        /// <remarks>
        ///     This extension method uses the top transaction of the current database. Therefore, at least one open transaction
        ///     must exist.
        /// </remarks>
        public static IEnumerable<ObjectId> GetObjectIds<TDbObject>(this Database database) where TDbObject : DBObject
        {
            var modelSpace = GetModelSpace(database);
            foreach (var objectId in modelSpace)
            {
                if (IsOfType<TDbObject>(objectId))
                {
                    yield return objectId;
                }
            }
        }

        /// <summary>
        ///     Check if the object ID points to a class of the given type.
        /// </summary>
        public static bool IsOfType<TDbObject>(this ObjectId objectId) where TDbObject : DBObject
        {
            var rxClass = RXObject.GetClass(typeof(TDbObject));

            return rxClass == objectId.ObjectClass
                   || GetBaseClasses(objectId.ObjectClass).Any(item => item == rxClass);
        }

        /// <summary>
        ///     Enumerates all objects of the specified type as well as its derived types.
        ///     <code> database.GetObject&lt;Line&gt;()</code> returns all objects of the type Line.
        ///     <code> database.GetObject&lt;Curve&gt;()</code> returns all objects of the type Curve and its subclasses like Line
        ///     or Polyline.
        /// </summary>
        /// <remarks>
        ///     This extension method uses the top transaction of the current database. Therefore, at least one open transaction
        ///     must exist.
        /// </remarks>
        public static IEnumerable<TDbObject> GetObjects<TDbObject>(this Database database, OpenMode openMode = OpenMode.ForRead)
            where TDbObject : DBObject
        {
            var transaction = database.TransactionManager.TopTransaction;
            foreach (var objectId in GetObjectIds<TDbObject>(database))
            {
                yield return (TDbObject)transaction.GetObject(objectId, openMode);
            }
        }

        /// <summary>
        ///     Returns all object IDs whose Dfx name matches the given regular expression.
        /// </summary>
        public static IEnumerable<ObjectId> GetObjectIds(this Database database, string dfxNamePattern)
        {
            var regex = new Regex(dfxNamePattern);
            var modelSpace = GetModelSpace(database);
            foreach (var objectId in modelSpace)
            {
                if (regex.IsMatch(objectId.ObjectClass.DxfName))
                {
                    yield return objectId;
                }
            }
        }

        /// <summary>
        ///     Returns all object IDs whose Dfx name is contained in the passed list.
        /// </summary>
        public static IEnumerable<ObjectId> GetObjectIds(this Database database, string[] dfxNames)
        {
            var modelSpace = GetModelSpace(database);
            foreach (var objectId in modelSpace)
            {
                if (dfxNames.Contains(objectId.ObjectClass.DxfName))
                {
                    yield return objectId;
                }
            }
        }

        /// <summary>
        ///     Returns all Dfx names of the objects contained in the database.
        /// </summary>
        /// <remarks>
        ///     Objects contained in blocks are not taken into account.
        /// </remarks>
        public static IList<string> CollectTypes(this Database database)
        {
            var modelSpace = GetModelSpace(database);

            var types = new List<string>();

            foreach (var objectId in modelSpace)
            {
                if (!types.Contains(objectId.ObjectClass.DxfName))
                {
                    types.Add(objectId.ObjectClass.DxfName);
                }
            }

            return types;
        }



        /// <summary>
        ///     Returns the base class of the given RX class.
        /// </summary>
        private static IEnumerable<RXClass> GetBaseClasses(RXClass rxClass)
        {
            var parent = rxClass.MyParent;
            while (parent != null && parent != BaseClass)
            {
                yield return parent;
                parent = parent.MyParent;
            }
        }

        /// <summary>
        ///     Returns the top transaction of the given database.
        /// </summary>
        /// <exception cref="DatabaseExtensionException"></exception>
        private static Transaction GetTopTransaction(Database database)
        {
            return database.TransactionManager.TopTransaction
                   ?? throw new DatabaseExtensionException("No open transaction could be found for the current database.");
        }
    }
}
