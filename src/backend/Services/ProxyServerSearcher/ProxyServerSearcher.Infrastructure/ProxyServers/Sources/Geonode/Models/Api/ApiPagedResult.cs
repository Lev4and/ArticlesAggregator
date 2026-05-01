using Newtonsoft.Json;

namespace ProxyServerSearcher.Infrastructure.ProxyServers.Sources.Geonode.Models.Api;

public record ApiPagedResult<TItem>
{
    [JsonProperty("data")]
    public TItem[] Data { get; }

    [JsonProperty("page")]
    public int Page { get; }

    [JsonProperty("limit")]
    public int Limit { get; }

    [JsonProperty("total")]
    public int Total { get; }

    public ApiPagedResult(TItem[] data, int page, int limit, int total)
    {
        Data = data;
        Page = page;
        Limit = limit;
        Total = total;
    }
}