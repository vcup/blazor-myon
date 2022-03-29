using Markdig;
using Markdig.Renderers;
using Markdig.Parsers;
using Markdig.Syntax.Inlines;
using Markdig.Extensions.AutoIdentifiers;

namespace myon.Data.MarkdownParser;

public class AutoIDExtension : AutoIdentifierExtension, IMarkdownExtension
{
    public AutoIDExtension(AutoIdentifierOptions options) : base(options)
    {
    }

    private void HeadingBlock_ProcessInlinesEnd(InlineProcessor processor, Inline? inline)
    {

    }
}
