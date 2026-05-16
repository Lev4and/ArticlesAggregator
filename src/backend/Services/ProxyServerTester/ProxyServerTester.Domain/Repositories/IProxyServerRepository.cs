using Database.Abstracts;
using ProxyServerTester.Domain.Entities;

namespace ProxyServerTester.Domain.Repositories;

public interface IProxyServerRepository : IRepository<ProxyServer, Guid>
{
    
}