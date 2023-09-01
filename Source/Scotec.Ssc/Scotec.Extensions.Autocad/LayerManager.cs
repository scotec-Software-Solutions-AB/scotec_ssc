using System.Text.RegularExpressions;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;

namespace Scotec.Extensions.Autocad;

public class LayerManager : ILayerManager
{
    private bool _disposed;

    /// <inheritdoc />
    public LayerTableRecord GetDefaultLayer(Database database)
    {
        return GetLayer(database, "0")!;
    }

    /// <inheritdoc />
    public LayerTableRecord? GetLayer(Database database, string name)
    {
        return GetLayers(database, name).FirstOrDefault();
    }

    /// <inheritdoc />
    public IEnumerable<LayerTableRecord> GetLayers(Database database, string filter)
    {
        return GetLayers(database, new[] { filter }, null);
    }

    /// <inheritdoc />
    public IEnumerable<LayerTableRecord> GetLayers(Database database, string[]? include, string[]? exclude)
    {
        include ??= Array.Empty<string>();
        exclude ??= Array.Empty<string>();

        var includeRegex = include.Select(item => new Regex(item)).ToList();
        var excludeRegex = exclude.Select(item => new Regex(item)).ToList();

        var transaction = database.TransactionManager.TopTransaction;
        var layerTable = ((LayerTable)transaction.GetObject(database.LayerTableId, OpenMode.ForRead)).IncludingHidden;

        foreach (var layerId in layerTable)
        {
            var layer = (LayerTableRecord)transaction.GetObject(layerId, OpenMode.ForRead);
            if ((include.Length == 0 || includeRegex.Any(regex => regex.IsMatch(layer.Name))) && !excludeRegex.Any(regex => regex.IsMatch(layer.Name)))
            {
                yield return layer;
            }
        }
    }

    /// <inheritdoc />
    public void EraseLayers(Database database, IEnumerable<LayerTableRecord> layers)
    {
        using var transaction = database.TransactionManager.StartTransaction();
        var layerIds = new ObjectIdCollection(layers.Select(item => item.Id).ToArray());

        var blockTable = (BlockTable)transaction.GetObject(database.BlockTableId, OpenMode.ForRead);
        foreach (var blockTableRecordId in blockTable)
        {
            var block = (BlockTableRecord)transaction.GetObject(blockTableRecordId, OpenMode.ForRead);

            foreach (var entId in block)
            {
                var entity = (Entity)transaction.GetObject(entId, OpenMode.ForRead);
                if (layerIds.Contains(entity.LayerId))
                {
                    entity.UpgradeOpen();
                    entity.Erase();
                }
            }
        }

        var modelSpace = (BlockTableRecord)transaction.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(database), OpenMode.ForWrite);

        foreach (var entId in modelSpace)
        {
            var entity = (Entity)transaction.GetObject(entId, OpenMode.ForRead);
            if (layerIds.Contains(entity.LayerId))
            {
                entity.UpgradeOpen();
                entity.Erase();
            }
        }

#if BEBUG
            var layersToBeDeleted = layers.Select(item => item.Name).ToList();
#endif
        var countBeforePurge = layerIds.Count;
        database.Purge(layerIds);
        if (layerIds.Count != countBeforePurge)
        {
            //TODO: There are still some layers referenced, but they can be erased?!?!
            //TODO: Logging which layers couldn't be removed.
            //throw new Exception("Not all references to one or more layers could be removed.");
        }

#if BEBUG
            var layersAfterPurge = layers.Where(item => layerIds.Contains(item.Id)).Select(item => item.Name).ToList();
            var diff = layersToBeDeleted.Except(layersAfterPurge).ToList();
#endif
        foreach (var layer in layers)
        {
            layer.UpgradeOpen();
            layer.Erase(true);
        }

        transaction.Commit();
    }

    public LayerTableRecord CreateLayer(Database database, string name)
    {
        using var transaction = database.TransactionManager.StartTransaction();
        var layerTable = database.GetLayerTable();

        var layerTableRecord = new LayerTableRecord();

        layerTableRecord.Color = Color.FromColorIndex(ColorMethod.ByColor, 7);
        layerTableRecord.Name = name;

        layerTable.UpgradeOpen();
        layerTable.Add(layerTableRecord);
        transaction.AddNewlyCreatedDBObject(layerTableRecord, true);

        transaction.Commit();

        return layerTableRecord;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        GC.SuppressFinalize(this);
    }
}
