using System.Diagnostics;
using System.Net;
using Messaging.Messages.ProxyServerEvents.Models;
using Microsoft.Extensions.Logging;
using Observability.Abstracts;
using ProxyServerTester.Application.Abstracts.ProxyServers;
using ProxyServerTester.Application.Dtos.ProxyServers;
using ProxyServerTester.Domain.Dtos.ProxyServers;
using Result;

namespace ProxyServerTester.Infrastructure.ProxyServers;

public class ProxyServerTester : IProxyServerTester
{
    private readonly ITracer<ProxyServerTester> _tracer;
    private readonly ILogger<ProxyServerTester> _logger;
    
    public ProxyServerTester(
        ITracer<ProxyServerTester> tracer, 
        ILogger<ProxyServerTester> logger)
    {
        _tracer = tracer;
        _logger = logger;
    }
    
    public async Task<AppResult<ProxyServerTestResult>> TestAsync(ProxyServerDto proxyServer, 
        CancellationToken ct = default)
    {
        using var operation = _tracer.StartOperation("Proxy server test");
        
        _logger.LogInformation("Proxy server test Name: {ProxyServerName}", proxyServer.NormalizedName);

        if (proxyServer.Protocol is ProxyServerProtocol.Unspecified)
        {
            _logger.LogWarning(
                "Proxy server protocol not supported Protocol: {ProxyServerProtocol}", 
                    proxyServer.Protocol);

            return AppResult<ProxyServerTestResult>.Failure(AppErrorType.Validation,
                "Proxy server protocol not supported");
        }
        
        var proxyServerUrlBuilder = new UriBuilder
        {
            Scheme = proxyServer.Protocol.ToString().ToLower(),
            Host   = proxyServer.HostnameOrAddress,
            Port   = proxyServer.Port,
        };
        
        var httpClientHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
            AutomaticDecompression                    = DecompressionMethods.All,
            AllowAutoRedirect                         = true,
            UseProxy                                  = true,
            Proxy                                     = new WebProxy
            {
                Address     = proxyServerUrlBuilder.Uri,
                Credentials = proxyServer.Credentials is not null
                    ? new NetworkCredential(proxyServer.Credentials.Username, proxyServer.Credentials.Password)
                    : null
            }
        };

        using var httpClient = new HttpClient(httpClientHandler, disposeHandler: true);
        
        httpClient.BaseAddress = new Uri("http://api.ipify.org/");
        httpClient.Timeout     = TimeSpan.FromSeconds(60);
        
        httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
        httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br, zstd");
        httpClient.DefaultRequestHeaders.Add("Accept-Language", "ru,en;q=0.9");
        httpClient.DefaultRequestHeaders.Add("Cache-Control", "max-age=0");
        httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
        httpClient.DefaultRequestHeaders.Add("Host", "hide-my-name.com");
        httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "document");
        httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "navigate");
        httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Site", "none");
        httpClient.DefaultRequestHeaders.Add("Sec-Fetch-User", "?1");
        httpClient.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
        httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/142.0.0.0 YaBrowser/25.12.0.0 Safari/537.36");
        httpClient.DefaultRequestHeaders.Add("sec-ch-ua", "\"Chromium\";v=\"142\", \"YaBrowser\";v=\"25.12\", \"Not_A Brand\";v=\"99\", \"Yowser\";v=\"2.5\"");
        httpClient.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
        httpClient.DefaultRequestHeaders.Add("sec-ch-ua-platform", "\"Windows\"");
        
        var httpRequestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Get
        };

        try
        {
            using var reqOperation = _tracer.StartOperation("Send request message");
            
            var reqTimer = new Stopwatch();
            
            reqTimer.Start();
            
            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage, 
                HttpCompletionOption.ResponseHeadersRead, ct);

            reqTimer.Stop();
            
            reqOperation?.Stop();
            
            httpResponseMessage.EnsureSuccessStatusCode();
            
            using var resOperation = _tracer.StartOperation("Receive response message");

            var resTimer = new Stopwatch();
            
            resTimer.Start();
            
            await using var httpResponseStream = await httpResponseMessage.Content.ReadAsStreamAsync(ct);
            using var httpResponseStreamReader = new StreamReader(httpResponseStream);

            await httpResponseStreamReader.ReadToEndAsync(ct);
            
            resTimer.Stop();
            
            resOperation?.Stop();
            
            _logger.LogInformation(
                "Proxy server test completed Name: {ProxyServerName} ReqTime: {RequestTime} ms ResTime: {ResponseTime} ms", 
                    proxyServer.NormalizedName, reqTimer.ElapsedMilliseconds, resTimer.ElapsedMilliseconds);
            
            return AppResult<ProxyServerTestResult>.Success(
                new ProxyServerTestResult
                {
                    RequestTime  = reqTimer.ElapsedMilliseconds,
                    ResponseTime = resTimer.ElapsedMilliseconds,
                });
        }
        catch (HttpRequestException exception) 
            when (exception.HttpRequestError is HttpRequestError.ProxyTunnelError)
        {
            _logger.LogError(exception, "Proxy server test failed");
            
            return AppResult<ProxyServerTestResult>.Failure(AppErrorType.Failed, "Proxy tunnel error");
        }
        catch (HttpRequestException exception) 
            when (exception.HttpRequestError is HttpRequestError.ConnectionError)
        {
            _logger.LogError(exception, "Proxy server test failed");
            
            return AppResult<ProxyServerTestResult>.Failure(AppErrorType.Failed, "Connection error");
        }
        catch (OperationCanceledException exception)
        {
            _logger.LogError(exception, "Proxy server test failed");
            
            return AppResult<ProxyServerTestResult>.Failure(AppErrorType.Failed, "Operation canceled");
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Proxy server test failed");
            
            return AppResult<ProxyServerTestResult>.Failure(AppErrorType.Failed, exception.Message);
        }
    }
}