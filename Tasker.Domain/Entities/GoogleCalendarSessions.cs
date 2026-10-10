namespace Tasker.Domain.Entities;

public class GoogleCalendarSessions
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public string GoogleSubject { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string Scope { get; set; } = string.Empty;
    public DateTime AccessTokenExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
