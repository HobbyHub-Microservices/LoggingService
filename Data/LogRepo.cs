using LoggingService.Models;

namespace LoggingService.Data;

public class LogRepo : ILogRepo
{
    private readonly AppDbContext _dbContext;

    public LogRepo(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public bool SaveChanges()
    {
        return (_dbContext.SaveChanges() >= 0);
    }

    public IEnumerable<Log> GetLogs()
    {
        return _dbContext.Logs.ToList();
    }

    public void AddLog(Log log)
    {
        ArgumentNullException.ThrowIfNull(log);
        _dbContext.Logs.Add(log);
    }

    public IEnumerable<Log> GetTodayLogs()
    {
        return _dbContext.Logs.Where(log => log.TimeStamp == DateTime.Today.Date).ToList();
    }
}