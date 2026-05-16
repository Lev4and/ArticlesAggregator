using Mediator;
using ProxyServerAggregator.Domain.Dtos.ProxyServers;
using Result;

namespace ProxyServerAggregator.Application.UseCases.ProxyServers.Queries.Get;

public record GetProxyServerQuery(Guid Id) : IQuery<AppResult<ProxyServerDto>>;