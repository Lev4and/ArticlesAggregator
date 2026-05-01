using System.Net.Http.Headers;

namespace ProxyServerSearcher.Infrastructure.ProxyServers.Extensions;

public static class HttpRequestHeadersExtensions
{
    extension(HttpRequestHeaders headers)
    {
        public void UseDefaultHeaders()
        {
            headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
            headers.Add("Accept-Encoding", "gzip, deflate, br, zstd");
            headers.Add("Accept-Language", "ru,en;q=0.9");
            headers.Add("Cache-Control", "max-age=0");
            headers.Add("Connection", "keep-alive");
            headers.Add("Sec-Fetch-Dest", "document");
            headers.Add("Sec-Fetch-Mode", "navigate");
            headers.Add("Sec-Fetch-Site", "none");
            headers.Add("Sec-Fetch-User", "?1");
            headers.Add("Upgrade-Insecure-Requests", "1");
            headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/142.0.0.0 YaBrowser/25.12.0.0 Safari/537.36");
            headers.Add("sec-ch-ua", "\"Chromium\";v=\"142\", \"YaBrowser\";v=\"25.12\", \"Not_A Brand\";v=\"99\", \"Yowser\";v=\"2.5\"");
            headers.Add("sec-ch-ua-mobile", "?0");
            headers.Add("sec-ch-ua-platform", "\"Windows\"");
        }
    }
}