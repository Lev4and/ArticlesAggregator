using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Messaging.Messages.ProxyServerEvents.Models;

[JsonConverter(typeof(StringEnumConverter))]
public enum ProxyServerProtocol
{
    Unspecified,
    Http,
    Https,
    Socks4,
    Socks5
}