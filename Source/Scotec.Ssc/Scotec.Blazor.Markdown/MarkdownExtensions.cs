using Markdig;

namespace Scotec.Blazor.Markdown;

public static class MarkdownExtensions
{
    /// <summary>
    ///     Uses all extensions supported by <c>Markdig.Wpf</c>.
    /// </summary>
    /// <param name="pipeline">The pipeline.</param>
    /// <returns>The modified pipeline</returns>
    public static MarkdownPipelineBuilder UseSupportedExtensions(this MarkdownPipelineBuilder pipeline)
    {
        if (pipeline == null)
        {
            throw new ArgumentNullException(nameof(pipeline));
        }

        return pipeline
               .UseEmphasisExtras()
               .UseGridTables()
               .UsePipeTables()
               .UseTaskLists()
               .UseAutoLinks();
    }
}
