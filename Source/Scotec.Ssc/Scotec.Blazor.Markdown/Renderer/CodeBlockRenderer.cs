using Markdig.Parsers;
using Markdig.Syntax;

namespace Scotec.Blazor.Markdown.Renderer;

/// <summary>
///     An HTML renderer for a <see cref="CodeBlock" /> and <see cref="FencedCodeBlock" />.
/// </summary>
/// <seealso cref="BlazorObjectRenderer{CodeBlock}" />
public class CodeBlockRenderer : BlazorObjectRenderer<CodeBlock>
{
    private HashSet<string> _blocksAsDiv;

    /// <summary>
    ///     Initializes a new instance of the <see cref="CodeBlockRenderer" /> class.
    /// </summary>
    public CodeBlockRenderer()
    {
    }

    public bool OutputAttributesOnPre { get; set; }

    /// <summary>
    ///     Gets a map of fenced code block infos that should be rendered as div blocks instead of pre/code blocks.
    /// </summary>
    public HashSet<string> BlocksAsDiv => _blocksAsDiv ??= new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    protected override void Write(BlazorRenderer renderer, CodeBlock obj)
    {
        if (_blocksAsDiv is not null && (obj as FencedCodeBlock)?.Info is string info &&
            _blocksAsDiv.Contains(info))
        {
            var infoPrefix = (obj.Parser as FencedCodeBlockParser)?.InfoPrefix ??
                             FencedCodeBlockParser.DefaultInfoPrefix;

            // We are replacing the HTML attribute `language-mylang` by `mylang` only for a div block
            // NOTE that we are allocating a closure here

            if (renderer.EnableHtmlForBlock)
            {
                renderer.OpenElement("div");

                renderer.AddAttributes(obj, cls => cls.StartsWith(infoPrefix, StringComparison.Ordinal)
                    ? cls.Substring(infoPrefix.Length)
                    : cls);
            }

            renderer.WriteLeafRawLines(obj, true, true, true);

            if (renderer.EnableHtmlForBlock)
            {
                renderer.CloseElement();
            }
        }
        else
        {
            if (renderer.EnableHtmlForBlock)
            {
                renderer.OpenElement("pre");

                if (OutputAttributesOnPre)
                {
                    renderer.AddAttributes(obj);
                }

                renderer.OpenElement("code");

                if (!OutputAttributesOnPre)
                {
                    renderer.AddAttributes(obj);
                }
            }

            renderer.WriteLeafRawLines(obj, true, true);

            if (renderer.EnableHtmlForBlock)
            {
                renderer.CloseElement()
                    .CloseElement();
            }
        }
    }
}