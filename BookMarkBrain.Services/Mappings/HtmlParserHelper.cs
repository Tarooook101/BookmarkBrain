using HtmlAgilityPack;
using Microsoft.Extensions.Logging;

namespace BookMarkBrain.Services.Mappings;

public class HtmlParserHelper
{
    private readonly ILogger<HtmlParserHelper> _logger;

    public HtmlParserHelper(ILogger<HtmlParserHelper> logger)
    {
        _logger = logger;
    }

    public async Task<string> ExtractTweetContentAsync(string url)
    {
        try
        {
            var web = new HtmlWeb();
            var doc = await Task.Run(() => web.Load(url));

            // Twitter-specific scraping logic
            // Note: This is a simplified approach and might need adjustment
            // depending on Twitter's DOM structure
            var tweetContent = doc.DocumentNode
                .SelectSingleNode("//div[contains(@class, 'tweet-text')]")?.InnerText?.Trim();

            return tweetContent ?? string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting tweet content from URL: {Url}", url);
            return string.Empty;
        }
    }

    public async Task<(string Title, string Content)> ExtractWebContentAsync(string url)
    {
        try
        {
            var web = new HtmlWeb();
            var doc = await Task.Run(() => web.Load(url));

            var title = doc.DocumentNode
                .SelectSingleNode("//head/title")?.InnerText?.Trim();

            var content = doc.DocumentNode
                .SelectSingleNode("//body//article") ??
                doc.DocumentNode.SelectSingleNode("//body//main") ??
                doc.DocumentNode.SelectSingleNode("//body");

            return (title ?? string.Empty, content?.InnerText?.Trim() ?? string.Empty);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting web content from URL: {Url}", url);
            return (string.Empty, string.Empty);
        }
    }
}