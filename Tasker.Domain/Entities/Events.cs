using Tasker.Domain.Enums;

namespace Tasker.Domain.Entities;

public class Events
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string? GoogleEventId { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public DateTime DateStart { get; set; }
    public DateTime DateEnd { get; set; }
    public string GoogleHtmlLink { get; set; }
    public SyncStatus SyncStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    
}