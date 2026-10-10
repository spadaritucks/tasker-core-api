namespace Tasker.Domain.Entities;

public class Sessions
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string AccessToken { get; set; }
    public string ExpiresIn { get; set; }
    
    public string DeviceId { get; set; }
    public string DeviceInfo { get; set; }
    public bool Revoke {get; set;}
    
    public User User { get; set; }
}