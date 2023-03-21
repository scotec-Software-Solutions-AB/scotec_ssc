using Autodesk.AutoCAD.DatabaseServices;

namespace Scotec.Extensions.Autocad;

public interface ILayerManager
{
    /// <summary>
    ///     Gets the dafault layer (layer "0").
    /// </summary>
    LayerTableRecord GetDefaultLayer(Database database);

    /// <summary>
    ///     Returns a layer with the given name.
    /// </summary>
    LayerTableRecord? GetLayer(Database database, string name);

    /// <summary>
    ///     Returns all layers whose names match the given regular expression.
    /// </summary>
    IEnumerable<LayerTableRecord> GetLayers(Database database, string filter);

    /// <summary>
    ///     Return all layers whose name matches one of the regular expression from the include list
    ///     and does not match any regular expression from the exclude list. If a layer name matches any expression from both,
    ///     the include and exclude list, the layer will be excluded.
    /// </summary>
    IEnumerable<LayerTableRecord> GetLayers(Database database, string[]? include, string[]? exclude);

    /// <summary>
    ///     Deletes the given layers from the database.
    /// </summary>
    void EraseLayers(Database database, IEnumerable<LayerTableRecord> layers);

    /// <summary>
    ///     Creates a new layer with the given name.
    /// </summary>
    LayerTableRecord CreateLayer(Database database, string name);
}
