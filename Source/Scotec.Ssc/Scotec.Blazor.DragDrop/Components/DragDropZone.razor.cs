using System.Drawing;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Scotec.Blazor.DragDrop.Components;

public partial class DragDropZone<TItem> : DragDropComponent, IDisposable
{
    [Inject] private Clipboard<TItem> Clipboard { get; set; }

    [Parameter] public IList<TItem> Items { get; set; }

    /// <summary>Gets or sets the item template.</summary>
    /// <value>The template.</value>
    [Parameter]
    public RenderFragment<TItem> Template { get; set; }

    [Parameter] public bool CanDrag { get; set; } = true;
    [Parameter] public bool CanDrop { get; set; } = true;

    [Parameter] public EventCallback<DropEventArgs<TItem>> OnItemDrop { get; set; }

    [Parameter] public EventCallback OnItemDropEnd { get; set; }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            // TODO release managed resources here
        }
    }

    protected override IDictionary<string, object> GetAttributes()
    {
        var attributes = base.GetAttributes();
        if (CanDrag)
        {
            attributes.Add("ondragstart", "event.dataTransfer.setData('text', event.target.id);");
        }

        return attributes.Where(item => !string.IsNullOrWhiteSpace(item.Value.ToString())).ToDictionary(item => item.Key, item => item.Value);
    }

    protected override bool ShouldRender()
    {
        return Clipboard.ShouldRender;
    }

    protected override string GetClasses()
    {
        return base.GetClasses();
    }

    protected override string GetStyles()
    {
        return base.GetStyles();
    }

    ~DragDropZone()
    {
        Dispose(false);
    }

    private string GetClassesForDropzone()
    {
        return string.Empty;
    }

    private string GetClassesForDraggable(TItem value)
    {
        return string.Empty;
    }

    private void OnDragStart(TItem item)
    {
        if (!CanDrag)
        {
            return;
        }

        Clipboard.ActiveItem = item;

        StateHasChanged();
    }

    private void OnDrop(DragEventArgs args)
    {
        if (!CanDrop)
        {
            return;
        }

        if (Clipboard.ActiveItem is null)
        {
            return;
        }

        OnItemDrop.InvokeAsync(new DropEventArgs<TItem>(Clipboard.ActiveItem, new Point((int)args.ClientX, (int)args.ClientY))).GetAwaiter().GetResult();

        StateHasChanged();
    }

    private void OnDragEnd()
    {
        Clipboard.ActiveItem = default;
        Clipboard.DragTargetItem = default;

        OnItemDropEnd.InvokeAsync().GetAwaiter().GetResult();
    }

    private void OnDragEnter(TItem item)
    {
        if (item.Equals(Clipboard.DragTargetItem))
        {
            return;
        }

        Clipboard.DragTargetItem = item;

        StateHasChanged();
    }

    private void OnDragLeave()
    {
        Clipboard!.DragTargetItem = default;

        StateHasChanged();
    }

    private object GetClassesForItem(TItem item)
    {
        return item.Equals(Clipboard.ActiveItem) ? "no-pointer-events" : "";
    }
}

public class DropEventArgs<TItem>(TItem item, Point position)
{
    public TItem Item { get; } = item;
    public Point Position { get; } = position;
}
