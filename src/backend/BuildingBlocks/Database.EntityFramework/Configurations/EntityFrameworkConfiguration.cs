using Database.EntityFramework.Enums;

namespace Database.EntityFramework.Configurations;

public class EntityFrameworkConfiguration : IEntityFrameworkConfiguration
{
    // ReSharper disable InconsistentNaming
    
    private const string DATABASE_TYPE = nameof(DATABASE_TYPE);
    
    // ReSharper restore InconsistentNaming
    
    public DatabaseType DatabaseType => Enum.Parse<DatabaseType>(
        Environment.GetEnvironmentVariable(DATABASE_TYPE) ?? nameof(DatabaseType.Postgres));

    public bool EnableDetailedErrors => true;
    
    public bool EnableSensitiveDataLogging => true;
}