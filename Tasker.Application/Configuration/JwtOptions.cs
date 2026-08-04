namespace Tasker.Application.Configuration;

public class JwtOptions
{
    public required string Issuer { get; set; }                                                                                                                                   
    public required string Audience { get; set; }                                                                                                                                 
    public required string SigningKey { get; set; }                                                                                                                               
    public int ExpirationMinutes { get; set; }    
}