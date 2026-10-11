using System.Text.Json.Serialization;

namespace Tasker.Application.DTOs.Events;

public class GoogleCalendarEventResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("htmlLink")]
    public string? HtmlLink { get; set; }

    [JsonPropertyName("created")]
    public DateTimeOffset Created { get; set; }
}
