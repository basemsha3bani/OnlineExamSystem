using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ServicesClasseslibrary.WorkerServices.Logging
{
    public class LogBackGroundService : BackgroundService
    {

        private readonly IServiceScopeFactory scopes;

        public LogBackGroundService(IServiceScopeFactory scopes) { this.scopes = scopes; }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Yield before database work so application startup is not blocked.
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                try
                {
                    using var scope = scopes.CreateScope();
                    await scope.ServiceProvider.GetRequiredService<LogginqQueueProcessor>().Process(stoppingToken);
                
                }
                catch(Exception ex)
                {
                    // Log the exception or handle it as needed
                    Console.WriteLine($"Error in LogBackGroundService: {ex.Message}");
                }


            }
        }
    }
}
