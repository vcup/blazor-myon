using myon.Data.MarkdownParser;
using Markdig;

namespace myon.Data;

public class MarkdownParserService
{
    public readonly UniqueNumberService service;

    private static readonly MarkdownPipelineBuilder pipelineBuilder = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .UseGridTables()
        .UseEmojiAndSmiley();

    public MarkdownParserService(UniqueNumberService service)
    {
        this.service = service;
        MarkdownHeadings = new();

        var autoID = new AutoIDExtension(AutoIDExtensionOption.EnableAll);
        pipeline = pipelineBuilder.Use(autoID).Build();
    }

    public Dictionary<string, string> MarkdownHeadings { get; set; }

    private readonly MarkdownPipeline pipeline;

    public string GetHtmlFormMarkdownFile(string path)
    {
        using (var fileReader = File.OpenText(path))
        {
            // 如果使用私有字段与构造函数共享该对象
            // 则会发生无法预料的行为，过于折磨
            // 第一个对 autoID.WhenSettedID 事件附加处理的实例会一直被调用
            // 其他实例对该事件附加处理程序这不会生效
            var autoID = pipeline.Extensions.Find<AutoIDExtension>()!;

            service.TryGetNumber(Path.GetFileNameWithoutExtension(path), out var n);
            autoID.prefix = n.ToString();

            autoID.WhenSettedId += addHeading;

            var markdown = fileReader.ReadToEnd();
            var result = Markdown.ToHtml(markdown,pipeline);
            
            autoID.WhenSettedId -= addHeading;

            return result;
        }
    }

    private void addHeading(string id, string heading) => MarkdownHeadings.TryAdd(id, heading);
}
