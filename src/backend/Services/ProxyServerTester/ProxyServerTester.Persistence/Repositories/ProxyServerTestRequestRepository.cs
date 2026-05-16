using Database.EntityFramework.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Observability.Abstracts;
using ProxyServerTester.Domain.Dtos.ProxyServers;
using ProxyServerTester.Domain.Entities;
using ProxyServerTester.Domain.Repositories;

namespace ProxyServerTester.Persistence.Repositories;

public class ProxyServerTestRequestRepository : AppDbContextRepository<ProxyServerTestRequest, Guid>, 
    IProxyServerTestRequestRepository
{
    public ProxyServerTestRequestRepository(
        ITracer<EntityFrameworkRepository<AppDbContext, ProxyServerTestRequest, Guid>> tracer, 
        ILogger<EntityFrameworkRepository<AppDbContext, ProxyServerTestRequest, Guid>> logger, 
        AppDbContext dbContext) : 
        base(tracer, logger, dbContext)
    {
        
    }

    public async Task<ProxyServerTestRequestDto?> GetAsync(Guid id, CancellationToken ct = default)
    {
        using var operation = Tracer.StartOperation("Get proxy server test request");
        
        Logger.LogInformation("Get proxy server test request Id: {ProxyTestRequestId}", id);
        
        return await DbContext.Set<ProxyServerTestRequest>().AsNoTracking()
            .Include(request => request.Server!)
            .Select(request => new ProxyServerTestRequestDto
            {
                Id          = request.Id,
                ProxyServer = new ProxyServerDto
                {
                    Id                = request.Server!.Id,
                    NormalizedName    = request.Server.NormalizedName,
                    Protocol          = request.Server.Protocol,
                    HostnameOrAddress = request.Server.HostnameOrAddress,
                    Port              = request.Server.Port,
                    Credentials       = request.Server.Credentials,
                }
            })
            .FirstOrDefaultAsync(request => request.Id == id, ct);
    }
}