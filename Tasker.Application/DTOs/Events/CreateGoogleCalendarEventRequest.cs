using System.Text.Json.Serialization;

namespace Tasker.Application.DTOs.Events;

public class CreateGoogleCalendarEventRequest
{
    [JsonPropertyName("start")]
    public EventStartAttributes Start { get; set; }
    
    [JsonPropertyName("end")]
    public EventEndAttributes End { get; set; }
    
    [JsonPropertyName("summary")]
    public string Summary {get; set;}
    
    [JsonPropertyName("description")]
    public string Description { get; set; }
}

public class EventStartAttributes
{
    [JsonPropertyName("timeZone")]
    public string TimeZone { get; set; }
    
    [JsonPropertyName("dateTime")]
    public DateTimeOffset DateTime { get; set; }
}

public class EventEndAttributes
{
    [JsonPropertyName("timeZone")]
    public string TimeZone { get; set; }
    
    [JsonPropertyName("dateTime")]
    public DateTimeOffset DateTime { get; set; }
}