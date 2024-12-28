using System.ComponentModel.DataAnnotations;

namespace LoggingService.Models;

public class Log
{

    [Key]
    [Required]
    public int Id { get; set; }
    
    [Required] 
    public LogLevel LogLevel { get; set; }

    [Required] 
    public required string  Message { get; set; }

    [Required] 
    public required string ServiceName { get; set; }

    
    public int? UserId { get; set; }

    [Required]
    public DateTime TimeStamp { get; set; }
    
}

public enum LogLevel
{
    Debug,
    Info,
    Warning,
    Error,
    Critical
}