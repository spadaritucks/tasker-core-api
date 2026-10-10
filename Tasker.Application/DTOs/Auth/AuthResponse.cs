namespace Tasker.Application.DTOs.Auth;

public class AuthResponse
{
    public bool Authenticated  { get; set; }
    public int ExpiresIn { get; set; }
}