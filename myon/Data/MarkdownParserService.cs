using myon.Data.MarkdownParser;
using Markdig;
using Markdig.Extensions.AutoIdentifiers;

namespace myon.Data;

public class MarkdownParserService
{
    private readonly UniqueNumberService service;

    public MarkdownParserService(UniqueNumberService service)
    {
        this.service = service;

        var aId = new AutoIDExtension(AutoIDExtensionOption.EnableAll);
        this.pipeline = new MarkdownPipelineBuilder()
            .UseEmojiAndSmiley()
            .UseGridTables()
            .UseAutoIdentifiers(AutoIdentifierOptions.AutoLink)
            .Use(aId)
            .UseAdvancedExtensions()
            .Build();
        aId.WhenSettedId += id => this.MarkdownHeadings.Add(id);
    }

    public List<string> MarkdownHeadings { get; set; } = new();

    private readonly MarkdownPipeline pipeline;
    public string GetHtmlFormMarkdownFile(string path)
    {
        using (var fileReader = File.OpenText(path))
        {
            var markdown = fileReader.ReadToEnd();
            service.TryGetNumber(Path.GetFileNameWithoutExtension(path), out var n);
            pipeline.Extensions.Find<AutoIDExtension>()!.prefix = n.ToString();
            return Markdown.ToHtml(markdown, pipeline);
        }
    }
}
