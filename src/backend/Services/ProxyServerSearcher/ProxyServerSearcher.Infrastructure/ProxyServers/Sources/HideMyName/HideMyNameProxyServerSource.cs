using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using AngleSharp.Html.Parser;
using AngleSharp.XPath;
using ProxyServerSearcher.Application.Abstracts.ProxyServers;
using ProxyServerSearcher.Application.Dtos.ProxyServers;
using ProxyServerSearcher.Domain.Enums;

namespace ProxyServerSearcher.Infrastructure.ProxyServers.Sources.HideMyName;

public class HideMyNameProxyServerSource : IProxyServerSource
{
    private readonly HideMyNameClient _client;
    private readonly HtmlParser       _htmlParser = new();

    public HideMyNameProxyServerSource(HideMyNameClient client)
    {
        _client = client;
    }
    
    public async IAsyncEnumerable<IReadOnlyCollection<ProxyServerDto>> ProvideAsync( 
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var offest = 0;
        var hasNextPage = false;
        
        do
        {
            var htmlPageContent  = await _client.GetProxyListHtmlPageAsync(offest, ct: ct);
            using var htmlDocument    = await _htmlParser.ParseDocumentAsync(htmlPageContent, ct);
            var htmlDocumentNavigator = htmlDocument.CreateNavigator();

            var nextPageLink = 
                htmlDocumentNavigator
                    .SelectSingleNode(
                        "//div[contains(@class, 'pagination')]/ul/li[contains(@class, 'next_array')]/a/@href")
                    ?.ToString();

            if (!string.IsNullOrEmpty(nextPageLink))
            {
                var regex = new Regex(@"\/proxy-list\/\?start=(?<offset>\d+)#list$");
                
                var match = regex.Match(nextPageLink);
                if (match.Success)
                {
                    hasNextPage = int.TryParse(match.Groups["offset"].Value, out var nextOffset);
                    
                    if (hasNextPage)
                    {
                        offest = nextOffset;
                    }
                }
            }
            
            var result = new List<ProxyServerDto>();

            foreach (var tableRowNode in htmlDocument.Body.SelectNodes(
                "//div[contains(@class, 'table_block')]//tbody/tr"))
            {
                var hostnameOrAddress = tableRowNode.ChildNodes.ElementAt(0).TextContent;
                var port                = int.Parse(tableRowNode.ChildNodes.ElementAt(1).TextContent);
                var protocol               = Enum.GetValues<ProxyServerProtocol>().First(protocol => 
                    string.Equals(
                        protocol.ToString(), 
                        tableRowNode.ChildNodes.ElementAt(4).TextContent.Split(", ").First(), 
                        StringComparison.CurrentCultureIgnoreCase));

                var proxyServer = new ProxyServerDto
                {
                    Protocol = protocol, HostnameOrAddress = hostnameOrAddress, Port = port,
                };
                
                result.Add(proxyServer);
            }
            
            yield return result.ToArray();
        } 
        while(hasNextPage);
    }

    public void Dispose()
    {
        _client.Dispose();
        
        GC.SuppressFinalize(this);
    }
}