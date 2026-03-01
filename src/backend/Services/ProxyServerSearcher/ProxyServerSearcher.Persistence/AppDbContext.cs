using Database.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace ProxyServerSearcher.Persistence;

public class AppDbContext : BaseDbContext
{
    public AppDbContext()
    {
        
    }

    public AppDbContext(DbContextOptions options) : base(options)
    {
        
    }
}