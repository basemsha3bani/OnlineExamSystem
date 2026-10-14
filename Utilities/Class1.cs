using Microsoft.Extensions.Configuration;

public class AppConfiguration
{
    internal readonly string _connectionString = string.Empty;
    private readonly string _QueueConfiguration;

    internal AppConfiguration()
    {
        var configurationBuilder = new ConfigurationBuilder();
        var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
        configurationBuilder.AddJsonFile(path, false);

        var root = configurationBuilder.Build();
        _connectionString = root.GetSection("ConnectionStrings").GetSection("DbCoreConnectionString").Value;
       _QueueConfiguration = root.GetSection("RabbitMq").GetSection("Host").Value;

    }
    internal string ConnectionString
    {
        get => _connectionString;
    }
    internal string QueueConfiguration
    {
        get => _QueueConfiguration;
    }

}
