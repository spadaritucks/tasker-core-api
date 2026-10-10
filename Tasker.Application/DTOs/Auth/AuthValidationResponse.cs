namespace Tasker.Application.DTOs.Auth;

public class AuthValidationResponse
{
    public string AccessToken { get; set; }
    public string TokenType { get; set; }
    public int ExpiresIn { get; set; }
}