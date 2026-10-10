namespace Tasker.Application.DTOs.Auth;

public class AuthMeResponse
{
    public bool Authenticated { get; init; }
    public int ExpiresIn { get; init; }
    public DateTime ExpiresAt { get; init; }
    public UserProfile User { get; init; } = new();
}

public class UserProfile
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}
