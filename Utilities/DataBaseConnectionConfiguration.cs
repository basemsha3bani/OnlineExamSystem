public class DataBaseConnectionConfiguration
{
    public string GetConnectionString()
    {
        AppConfiguration appConfiguration = new AppConfiguration();
        return appConfiguration.ConnectionString;

    }
}
