namespace Scotec.Blazor.DragDrop.Components;

internal class Clipboard<TType>
{
    /// <summary>
    ///     Currently Active Item
    /// </summary>
    public TType ActiveItem { get; set; }

    /// <summary>
    ///     The item the active item is hovering above.
    /// </summary>
    public TType DragTargetItem { get; set; }

    /// <summary>
    ///     Holds a reference to the items of the dropzone in which the drag operation originated
    /// </summary>
    public IList<TType> Items { get; set; }

    public bool ShouldRender { get; set; } = true;

    // Notify subscribers that there is a need for rerender
    public EventHandler StateHasChanged { get; set; }

    /// <summary>
    ///     Resets the service to initial state
    /// </summary>
    public void Reset()
    {
        ShouldRender = true;
        ActiveItem = default;
        Items = null;
        DragTargetItem = default;

        StateHasChanged?.Invoke(this, EventArgs.Empty);
    }
}
