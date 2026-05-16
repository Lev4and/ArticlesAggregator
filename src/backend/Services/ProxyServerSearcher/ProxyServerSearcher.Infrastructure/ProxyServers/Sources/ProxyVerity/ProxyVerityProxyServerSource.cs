using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using AngleSharp.Html.Parser;
using AngleSharp.XPath;
using Messaging.Messages.ProxyServerEvents.Models;
using Microsoft.Extensions.Logging;
using Observability.Abstracts;
using ProxyServerSearcher.Application.Abstracts.ProxyServers;
using ProxyServerSearcher.Application.Dtos.ProxyServers;
using ProxyServerSearcher.Infrastructure.ProxyServers.Constants;

namespace ProxyServerSearcher.Infrastructure.ProxyServers.Sources.ProxyVerity;

public class ProxyVerityProxyServerSource : IProxyServerSource
{
    private readonly ITracer<ProxyVerityProxyServerSource> _tracer;
    private readonly ILogger<ProxyVerityProxyServerSource> _logger;
    private readonly HtmlParser        _htmlParser = new();
    private readonly ProxyVerityClient _client;
    
    public ProxyVerityProxyServerSource(
        ITracer<ProxyVerityProxyServerSource> tracer, 
        ILogger<ProxyVerityProxyServerSource> logger,
        ProxyVerityClient client)
    {
        _tracer = tracer;
        _logger = logger;
        _client = client;
    }
    
    public async IAsyncEnumerable<IReadOnlyCollection<ProxyServerDto>> ProvideAsync(
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        using var operation =
            _tracer.StartOperation($"Provide proxy servers ({ProxyServerSourceConstants.ProxyVerity})");
        
        _logger.LogInformation("Provide proxy servers Source: {ProxyServersSource}", 
            ProxyServerSourceConstants.ProxyVerity);
        
        var currentPage = 1;
        var hasNextPage = false;

        do
        {
            var htmlPageResult = await _client.GetFreeProxyListHtmlPageAsync(currentPage, ct: ct);
            if (htmlPageResult.IsFailure)
            {
                yield break;
            }

            await using var htmlPageContent = htmlPageResult.Result!;
            
            using var htmlDocument    = await _htmlParser.ParseDocumentAsync(htmlPageContent, ct);
            var htmlDocumentNavigator = htmlDocument.CreateNavigator();
            
            var nextPageLink = 
                htmlDocumentNavigator
                    .SelectSingleNode(
                        "//ul[contains(@class, 'pagination')]/li[contains(@class, 'page-item active')]/following-sibling::*/a[contains(@class, 'page-link')]/@href")
                    ?.ToString();

            hasNextPage = !string.IsNullOrEmpty(nextPageLink);
            
            if (hasNextPage)
            {
                currentPage += 1;
            }
            
            var result = new List<ProxyServerDto>();
        
            foreach (var tableRowNode in htmlDocument.Body.SelectNodes(
                "//table[contains(@class, 'table')]/tbody/tr"))
            {
                var addressContent  = tableRowNode.ChildNodes.ElementAt(1).TextContent.Trim();
                var protocolContent = tableRowNode.ChildNodes.ElementAt(7).TextContent.Trim();
                
                var regex = new Regex(@"^(?<host>.+):(?<port>\d+)$");
                var match = regex.Match(addressContent.ToString()!);
                if (!match.Success) continue;
            
                var hostnameOrAddress = match.Groups["host"].Value;
                var port              = int.Parse(match.Groups["port"].Value);
                var protocol          = Enum.GetValues<ProxyServerProtocol>().First(protocol => 
                    string.Equals(protocol.ToString(), protocolContent, StringComparison.CurrentCultureIgnoreCase));
            
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