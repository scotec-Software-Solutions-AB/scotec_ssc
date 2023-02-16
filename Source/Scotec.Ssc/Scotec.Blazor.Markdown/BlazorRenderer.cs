﻿using System.Net;
using Markdig.Helpers;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.RenderTree;
using Scotec.Blazor.Markdown.Renderer.Inline;
using System.Web;
using CodeBlockRenderer = Scotec.Blazor.Markdown.Renderer.CodeBlockRenderer;
using HeadingRenderer = Scotec.Blazor.Markdown.Renderer.HeadingRenderer;
using HtmlBlockRenderer = Scotec.Blazor.Markdown.Renderer.HtmlBlockRenderer;
using ListRenderer = Scotec.Blazor.Markdown.Renderer.ListRenderer;
using ParagraphRenderer = Scotec.Blazor.Markdown.Renderer.ParagraphRenderer;
using QuoteBlockRenderer = Scotec.Blazor.Markdown.Renderer.QuoteBlockRenderer;
using ThematicBreakRenderer = Scotec.Blazor.Markdown.Renderer.ThematicBreakRenderer;

namespace Scotec.Blazor.Markdown;

public class BlazorRenderer : Markdig.Renderers.RendererBase
{
    private readonly RenderTreeBuilder _builder;
    private int _line;

    public BlazorRenderer(RenderTreeBuilder builder)
    {
        _builder = builder;

        // Default block renderers
        ObjectRenderers.Add(new CodeBlockRenderer());
        ObjectRenderers.Add(new ListRenderer());
        ObjectRenderers.Add(new HeadingRenderer());
        ObjectRenderers.Add(new HtmlBlockRenderer());
        ObjectRenderers.Add(new ParagraphRenderer());
        ObjectRenderers.Add(new QuoteBlockRenderer());
        ObjectRenderers.Add(new ThematicBreakRenderer());

        ///Default inline renderers
        ObjectRenderers.Add(new AutolinkInlineRenderer());
        ObjectRenderers.Add(new CodeInlineRenderer());
        ObjectRenderers.Add(new DelimiterInlineRenderer());
        ObjectRenderers.Add(new EmphasisInlineRenderer());
        ObjectRenderers.Add(new LineBreakInlineRenderer());
        ObjectRenderers.Add(new HtmlEntityInlineRenderer());
        ObjectRenderers.Add(new LinkInlineRenderer());
        ObjectRenderers.Add(new LiteralInlineRenderer());

        //// Extension renderers
        //ObjectRenderers.Add(new TableRenderer());
        //ObjectRenderers.Add(new TaskListRenderer());

        EnableHtmlForBlock = true;
        EnableHtmlForInline = true;
        EnableHtmlEscape = true;
    }

    internal bool ImplicitParagraph { get; set; }
    internal bool EnableHtmlForBlock { get; set; }
    internal bool EnableHtmlForInline { get; set; }
    internal bool EnableHtmlEscape { get; set; }
    public bool UseNonAsciiNoEscape { get; set; }
    public Uri BaseUrl { get; set; }
    public Func<string, string> LinkRewriter { get; set; }

    internal string UrlEncode(string url)
    {
        //return HttpUtility.UrlEncode(url);
        return Uri.EscapeDataString(url);
    }


    /// <summary>Render the markdown object in a XamlWriter.</summary>
    /// <param name="markdownObject"></param>
    /// <returns></returns>
    public override object Render(MarkdownObject markdownObject)
    {
        if (markdownObject is MarkdownDocument)
        {
            Write(markdownObject);
            return this;
        }

        Write(markdownObject);
        return this;
    }

    internal BlazorRenderer OpenElement(string elementName)
    {
        _builder.OpenElement(_line++, elementName);
        return this;
    }

    
    internal BlazorRenderer AddLineBreak()
    {
        _builder.AddContent(_line++, NewLine.LineFeed.AsString());
        return this;
    }
    internal BlazorRenderer AddContent(string content)
    {
        _builder.AddContent(_line++, content);
        return this;
    }

    internal BlazorRenderer AddMarkupContent(string content)
    {
        _builder.AddMarkupContent(_line++, content);
        return this;
    }

    public BlazorRenderer AddContent(StringSlice slice)
    {
        AddContent(slice.AsSpan());
        return this;
    }

    public BlazorRenderer AddMarkupContent(StringSlice slice)
    {
        AddMarkupContent(slice.AsSpan());
        return this;
    }

    internal BlazorRenderer AddContent(ReadOnlySpan<char> content)
    {
        AddContent(content.ToString());
        return this;
    }

    internal BlazorRenderer AddMarkupContent(ReadOnlySpan<char> content)
    {
        AddMarkupContent(content.ToString());
        return this;
    }


    internal BlazorRenderer CloseElement()
    {
        _builder.CloseElement();
        return this;
    }

    //internal void WriteLeafRawLines(LeafBlock leafBlock, bool writeEndOfLines, bool escape, bool softEscape = false)
    //{
    //    throw new NotImplementedException();
    //}

    /// <summary>
    ///     Writes the inlines of a leaf inline.
    /// </summary>
    /// <param name="leafBlock">The leaf block.</param>
    /// <returns>This instance</returns>
    public BlazorRenderer WriteLeafInline(LeafBlock leafBlock)
    {
        Inline inline = leafBlock.Inline;

        while (inline != null)
        {
            Write(inline);
            inline = inline.NextSibling;
        }

        return this;
    }

    /// <summary>
    /// Writes the lines of a <see cref="LeafBlock"/>
    /// </summary>
    /// <param name="leafBlock">The leaf block.</param>
    /// <param name="writeEndOfLines">if set to <c>true</c> write end of lines.</param>
    /// <param name="escape">if set to <c>true</c> escape the content for HTML</param>
    /// <param name="softEscape">Only escape &lt; and &amp;</param>
    /// <returns>This instance</returns>
    public BlazorRenderer WriteLeafRawLines(LeafBlock leafBlock, bool writeEndOfLines, bool escape, bool softEscape = false)
    {
        var slices = leafBlock.Lines.Lines;
        if (slices is not null)
        {
            for (int i = 0; i < slices.Length; i++)
            {
                ref StringSlice slice = ref slices[i].Slice;
                if (slice.Text is null)
                {
                    break;
                }

                if (!writeEndOfLines && i > 0)
                {
                    AddContent(string.Empty);
                }

                ReadOnlySpan<char> span = slice.AsSpan();
                if (escape)
                {
                    //TODO: Use softEscape
                    //AddContent(span, softEscape);
                    AddContent(span);
                }
                else
                {
                    AddContent(span);
                }

                if (writeEndOfLines)
                {
                    AddLineBreak();
                }
            }
        }

        return this;
    }


    /// <summary>
    ///     Writes the attached <see cref="Markdig.Renderers.Html.HtmlAttributes" /> on the specified <see cref="MarkdownObject" />.
    /// </summary>
    /// <param name="markdownObject">The object.</param>
    /// <param name="classFilter">A class filter used to transform a class into another class at writing time</param>
    /// <returns></returns>
    public BlazorRenderer AddAttributes(MarkdownObject markdownObject, Func<string, string> classFilter = null)
    {
        return AddAttributes(markdownObject.TryGetAttributes(), classFilter);
    }

    /// <summary>
    ///     Writes the specified <see cref="Markdig.Renderers.Html.HtmlAttributes" />.
    /// </summary>
    /// <param name="attributes">The attributes to render.</param>
    /// <param name="classFilter">A class filter used to transform a class into another class at writing time</param>
    /// <returns>This instance</returns>
    public BlazorRenderer AddAttributes(HtmlAttributes attributes, Func<string, string> classFilter = null)
    {
        if (attributes is null)
        {
            return this;
        }

        if (attributes.Id != null)
        {
            _builder.AddAttribute(_line++, attributes.Id);
        }

        if (attributes.Classes is { Count: > 0 })
        {
            _builder.AddAttribute(_line++, "class",
                string.Join(" ",
                    attributes.Classes.Select(cssClass => classFilter != null ? classFilter(cssClass) : cssClass)));
        }

        if (attributes.Properties is { Count: > 0 })
        {
            foreach (var property in attributes.Properties)
            {
                _builder.AddAttribute(_line++, property.Key, property.Value ?? string.Empty);
            }
        }

        return this;
    }
}