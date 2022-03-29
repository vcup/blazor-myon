using myon.Data.MarkdownParser;
using Markdig;

namespace myon.Data;

public class MarkdownParserService
{
    private readonly AutoIDExtension autoIDExtension = new(Markdig.Extensions.AutoIdentifiers.AutoIdentifierOptions.Default);

    public string GetHtmlFormMarkdownFile(string path)
    {
        using (var fileReader = File.OpenText(path))
        {
            var markdown = fileReader.ReadToEnd();
            return Markdown.ToHtml(markdown);
        }
    }
}
