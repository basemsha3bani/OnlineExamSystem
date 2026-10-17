using Microsoft.Extensions.Configuration;

internal class AppConfiguration
{
    internal readonly string _connectionString = string.Empty;
    private readonly string _QueueHost;
    private readonly string _QueuePort;

    internal AppConfiguration()
    {
        var configurationBuilder = new ConfigurationBuilder();
        var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
        configurationBuilder.AddJsonFile(path, false);
        configurationBuilder.AddEnvironmentVariables();

        var root = configurationBuilder.Build();
        var RabbitMqSection = root.GetSection("RabbitMq");
        _connectionString = root.GetSection("ConnectionStrings").GetSection("DbCoreConnectionString").Value;
       // _QueuePort = RabbitMqSection.GetSection("Port").Value;
        _QueueHost = RabbitMqSection.GetSection("Host").Value;

    }
    internal string ConnectionString
    {
        get => _connectionString;
    }
    internal string QueueHost
    {
        get => _QueueHost;
    }
    //internal string QueuePort
    //{
    //    get => _QueuePort;
    //}

}
