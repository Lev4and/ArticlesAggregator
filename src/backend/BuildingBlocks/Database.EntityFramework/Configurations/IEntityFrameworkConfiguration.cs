using Database.EntityFramework.Enums;

namespace Database.EntityFramework.Configurations;

public interface IEntityFrameworkConfiguration
{
    DatabaseType DatabaseType { get; }
    
    bool EnableDetailedErrors { get; }
    
    bool EnableSensitiveDataLogging { get; }
}