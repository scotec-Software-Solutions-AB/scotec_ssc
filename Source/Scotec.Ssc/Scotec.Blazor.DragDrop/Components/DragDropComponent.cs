using Microsoft.AspNetCore.Components;

namespace Scotec.Blazor.DragDrop.Components;

public abstract class DragDropComponent : ComponentBase
{
    /// <summary>Gets or sets the child content.</summary>
    /// <value>The content of the child.</value>
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    /// <summary>Gets or sets the inline CSS style.</summary>
    /// <value>The style.</value>
    [Parameter]
    public string Style { get; set; } = string.Empty;

    /// <summary>Gets or sets the CSS classes.</summary>
    /// <value>The classes.</value>
    [Parameter]
    public string Class { get; set; } = string.Empty;

    /// <summary>Gets or sets the Id of the html5 element.</summary>
    /// <value>The id.</value>
    [Parameter]
    public string Id { get; set; } = string.Empty;

    protected virtual string GetClasses()
    {
        return string.Empty;
    }

    protected virtual string GetStyles()
    {
        return string.Empty;
    }

    protected virtual IDictionary<string, object> GetAttributes()
    {
        var attributes = new Dictionary<string, object>();

        var id = BuildIdAttribute();
        var classes = BuildClassAttribute();
        var styles = BuildStyleAttribute();

        if (!string.IsNullOrWhiteSpace(id))
        {
            attributes.Add("id", id);
        }

        if (!string.IsNullOrWhiteSpace(classes))
        {
            attributes.Add("class", classes);
        }

        if (!string.IsNullOrWhiteSpace(styles))
        {
            attributes.Add("style", styles);
        }

        return attributes;
    }

    private string BuildClassAttribute()
    {
        var classes = GetClasses() ?? string.Empty;
        return $"{Class} {classes}";
    }

    private string BuildStyleAttribute()
    {
        var styles = GetStyles() ?? string.Empty;
        return $"{Style} {styles}";
    }

    private string BuildIdAttribute()
    {
        return string.IsNullOrWhiteSpace(Id) ? string.Empty : Id;
    }
}
