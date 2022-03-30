using Markdig;
using Markdig.Renderers;
using Markdig.Parsers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Markdig.Renderers.Html;

namespace myon.Data.MarkdownParser;

public class AutoIDExtension : IMarkdownExtension
{
    private readonly AutoIDExtensionOption option;

    private const string AutoIDExtensionKey = "AutoIDExtension";
    public string? prefix { get; set; }

    public AutoIDExtension(AutoIDExtensionOption option)
    {
        this.option = option;
    }

    public void Setup(MarkdownPipelineBuilder pipeline)
    {
        var headingBlockParser = pipeline.BlockParsers.Find<HeadingBlockParser>();
        if (headingBlockParser is not null)
        {
            headingBlockParser.Closed -= HeadingBlockParser_Close;
            headingBlockParser.Closed += HeadingBlockParser_Close;
        }
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
    }

    private void HeadingBlockParser_Close(BlockProcessor processor, Block block)
    {
        if (block is HeadingBlock headingBlock)
        {
            headingBlock.ProcessInlinesEnd += HeadingBlock_ProcessInlineEnd;
        }
    }

    private void HeadingBlock_ProcessInlineEnd(InlineProcessor processor, Inline? inline)
    {
        var SectionNOs = processor.Document.GetData(AutoIDExtensionKey) as Dictionary<int, int>;
        if (SectionNOs is null)
        {
            SectionNOs = new();
            processor.Document.SetData(AutoIDExtensionKey, SectionNOs);
        }

        var headingBlock = (processor.Block as HeadingBlock)!;
        if (headingBlock.Inline is null)
        {
            return;
        }

        var attributes = headingBlock.GetAttributes();

        #region Generate SectionNumber

        var level = headingBlock.Level;
        SectionNOs.TryAdd(level, 0);
        SectionNOs[headingBlock.Level]++;

        var sectionText = "";
        while (level > 0)
        {
            SectionNOs.TryGetValue(level, out var lastSectionNo);
            sectionText = string.IsNullOrEmpty(sectionText) ? lastSectionNo.ToString() : $"{lastSectionNo}-{sectionText}";
            level--;
        }

        #endregion

        if ((option & AutoIDExtensionOption.UseAutoPrefix) != 0 && prefix is not null)
            sectionText = $"{prefix}-{sectionText}";

        if ((option & AutoIDExtensionOption.StichOrginalId) != 0)
            sectionText = $"{sectionText}-{attributes.Id}";

        var v = (option & AutoIDExtensionOption.StichOrginalId);

        WhenSettedId(attributes.Id = sectionText);
    }

    public delegate void IdSettedIdHandler(string id);
    public event IdSettedIdHandler WhenSettedId;
}
