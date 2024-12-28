using LoggingService.Models;

namespace LoggingService.Data;

public interface ILogRepo
{
    bool SaveChanges();
    
    IEnumerable<Log> GetLogs();
    
    void AddLog(Log log);
    
    IEnumerable<Log> GetTodayLogs();
    
    
}