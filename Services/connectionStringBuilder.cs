// Services/ConnectionStringBuilder.cs
public static class ConnectionStringBuilder
{
    public static string BuildConnectionString(DatabaseConfig dbConfig)
    {
        return dbConfig.DbType switch
        {
            "psql" => $"Server={dbConfig.DbIP};Port={dbConfig.DbPort};Database={dbConfig.DbDatabase};User Id={dbConfig.DbUser};Password={dbConfig.DbPass};",
            "mssql" => $"Server={dbConfig.DbIP},{dbConfig.DbPort};Database={dbConfig.DbDatabase};User Id={dbConfig.DbUser};Password={dbConfig.DbPass};",
            "oracle" => $"User Id={dbConfig.DbUser};Password={dbConfig.DbPass};Data Source={dbConfig.DbIP}:{dbConfig.DbPort}/{dbConfig.DbDatabase};",
            _ => throw new NotSupportedException($"Unsupported database type: {dbConfig.DbType}")
        };
    }
}