using System.ComponentModel.DataAnnotations;

namespace Stopwatch;

public class JwtConfiguration
{
    [Required] 
    public string SigningKey { get; set; } = String.Empty;
}