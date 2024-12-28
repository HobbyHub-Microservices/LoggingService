using LoggingService.Models;
using Microsoft.EntityFrameworkCore;

namespace LoggingService.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt)
    {
        
    }
    
    public DbSet<Log> Logs { get; set; }
}