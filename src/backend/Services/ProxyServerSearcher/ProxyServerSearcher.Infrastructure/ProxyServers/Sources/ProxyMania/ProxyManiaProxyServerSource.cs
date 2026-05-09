using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using AngleSharp.Html.Parser;
using AngleSharp.XPath;
using Microsoft.Extensions.Logging;
using Observability.Abstracts;
using ProxyServerSearcher.Application.Abstracts.ProxyServers;
using ProxyServerSearcher.Application.Dtos.ProxyServers;
using ProxyServerSearcher.Domain.Enums;
using ProxyServerSearcher.Infrastructure.ProxyServers.Constants;

namespace ProxyServerSearcher.Infrastructure.ProxyServers.Sources.ProxyMania;

public class ProxyManiaProxyServerSource : IProxyServerSource
{
    private readonly ITracer<ProxyManiaProxyServerSource> _tracer;
    private readonly ILogger<ProxyManiaProxyServerSource> _logger;
    private readonly HtmlParser _htmlParser = new();
    private readonly ProxyManiaClient _client;

    public ProxyManiaProxyServerSource(
        ITracer<ProxyManiaProxyServerSource> tracer,
        ILogger<ProxyManiaProxyServerSource> logger,
        ProxyManiaClient client)
    {
        _tracer = tracer;
        _logger = logger;
        _client = client;
    }
    
    public async IAsyncEnumerable<IReadOnlyCollection<ProxyServerDto>> ProvideAsync(
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        using var operation =
            _tracer.StartOperation($"Provide proxy servers ({ProxyServerSourceConstants.ProxyMania})");
        
        _logger.LogInformation("Provide proxy servers Source: {ProxyServersSource}", 
            ProxyServerSourceConstants.ProxyMania);
        
        var currentPage = 1;
        var hasNextPage = false;

        do
        {
            var htmlPageContent = await _client.GetFreeProxyListHtmlPageAsync(currentPage, ct: ct);
            using var htmlDocument = await _htmlParser.ParseDocumentAsync(htmlPageContent, ct);
            var htmlDocumentNavigator = htmlDocument.CreateNavigator();
            
            var pageNumberText = 
                htmlDocumentNavigator
                    .SelectSingleNode(
                        "//ul[contains(@class, 'pagination')]//a[contains(@class, 'page-link') and @data-ci-pagination-page and @rel='next']/@data-ci-pagination-page")
                    ?.ToString();

            hasNextPage = !string.IsNullOrEmpty(pageNumberText);
            
            if (hasNextPage)
            {
                currentPage += 1;
            }
            
            var result = new List<ProxyServerDto>();
        
            foreach (var tableRowNode in htmlDocument.Body.SelectNodes(
                "//table[contains(@class, 'table_proxychecker')]/tbody/tr"))
            {
                var addressContent  = tableRowNode.ChildNodes.ElementAt(1).TextContent;
                var protocolContent = tableRowNode.ChildNodes.ElementAt(5).TextContent;
                
                var regex = new Regex(@"^(?<host>.+):(?<port>\d+)$");
                var match = regex.Match(addressContent.ToString()!);
                if (!match.Success) continue;
            
                var hostnameOrAddress = match.Groups["host"].Value;
                var port                = int.Parse(match.Groups["port"].Value);
                var protocol               = Enum.GetValues<ProxyServerProtocol>().First(protocol => 
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