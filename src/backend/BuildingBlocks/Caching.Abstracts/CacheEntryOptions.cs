namespace Caching.Abstracts;

public record CacheEntryOptions
{
    public TimeSpan? SlidingExpiration { get; init; }
    
    public DateTimeOffset? AbsoluteExpiration { get; init; }
}