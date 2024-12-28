using System.Text.Json;
using LoggingService.Data;
using LoggingService.Models;

namespace LoggingService.LogProcessing;

public class LogProcessor : ILogProcessor
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public LogProcessor(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }
    public void ProcessLog(string log)
    {
        
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var repo = scope.ServiceProvider.GetRequiredService<ILogRepo>();
            var deserilizedlog = JsonSerializer.Deserialize<Log>(log);
            Console.WriteLine($"Log found: {deserilizedlog}");
            try
            {
                if (deserilizedlog != null) repo.AddLog(deserilizedlog);
                repo.SaveChanges();
                Console.WriteLine("--> Log added");
                
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not add Log to DB: {e.Message}");
            }
        }
    }

}