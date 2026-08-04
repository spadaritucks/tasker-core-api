namespace Tasker.Application.DTOs.Auth;

public class JwtTokenResponse
{
    public string AccessToken { get; init; } = string.Empty;                                                                                                                      
    public string TokenType { get; init; } = "Bearer";                                                                                                                            
    public int ExpiresIn { get; init; }   
}