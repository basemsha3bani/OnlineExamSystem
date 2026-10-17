public class QueueConfiguration()
{
    public string GetQueueHostConfiguration()
    {
        AppConfiguration appConfiguration = new AppConfiguration();
        return appConfiguration.QueueHost;
    }

}