using System.Diagnostics;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Scotec.Web.ImageServer;

public class ScotecImage : ComponentBase
{
    [Inject] public IImageServer? ImageServer { get; set; }

    [Parameter] public string? ImageData { get; set; }

    [Parameter] public int? Height { get; set; }

    [Parameter] public int? Width { get; set; }

    [Parameter] public string? Path { get; set; }

    [Parameter] public string? Alt { get; set; }

    [Parameter] public string? Id { get; set; }

    [Parameter] public string? Class { get; set; }

    [Parameter] public string? Style { get; set; }

    [Parameter] public ImageFormat? Format { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        base.BuildRenderTree(builder);

        var index = 0;

        builder.OpenElement(index++, "img");

        if (!string.IsNullOrWhiteSpace(Id))
        {
            builder.AddAttribute(index++, "id", Id);
        }

        if (!string.IsNullOrWhiteSpace(Alt))
        {
            builder.AddAttribute(index++, "alt", Alt);
        }

        if (!string.IsNullOrWhiteSpace(Class))
        {
            builder.AddAttribute(index++, "class", Class);
        }

        if (!string.IsNullOrWhiteSpace(Style))
        {
            builder.AddAttribute(index++, "Style", Style);
        }

        if (Height != null)
        {
            builder.AddAttribute(index++, "height", $"{Height}");
        }

        if (Width != null)
        {
            builder.AddAttribute(index++, "width", $"{Width}");
        }

        //builder.AddAttribute(index++, "src", ImageData != null ? ResizeImage() : Path ?? string.Empty);
        builder.AddAttribute(index++, "src", ImageData ?? string.Empty);

        builder.CloseElement();
        builder.AddMarkupContent(index, "\r\n");
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && !string.IsNullOrEmpty(Path))
        {
            await LoadImageAsync(Path, Width, Height);
        }
    }

    private async Task LoadImageAsync(string path, int? width, int? height)
    {
        try
        {
            Debug.Assert(ImageServer != null, nameof(ImageServer) + " != null");
            var imageInfo = await ImageServer.GetImageInfoAsync(path, width, height);
            if (imageInfo == null)
            {
                ImageData = string.Empty;
            }
            else
            {
                var memoryStream = new MemoryStream();
                await imageInfo.Value.Image.CopyToAsync(memoryStream);
                memoryStream.TryGetBuffer(out var buffer);

                var imageFormat = imageInfo.Value.Format.ToString().ToLower();

                ImageData = $"data:image{imageFormat};base64,{Convert.ToBase64String(buffer.Array ?? Array.Empty<byte>(), 0, (int)memoryStream.Length)}";
            }
        }
        catch (Exception)
        {
            ImageData = string.Empty;
        }
        finally
        {
            await InvokeAsync(StateHasChanged);
        }
    }
}
