using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Tasker.Application.DTOs.Events;
using Tasker.Application.Exceptions;

namespace Tasker.Application.Services;

public class GoogleCalendarService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<GoogleCalendarService> _logger;

    public GoogleCalendarService(
        HttpClient httpClient,
        IConfiguration configuration, 
        ILogger<GoogleCalendarService> logger
        )
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<GoogleCalendarEventResponse?> CreateEvent(
        string accessToken,
        CreateGoogleCalendarEventRequest createGoogleCalendarEventRequest,
        CancellationToken cancellationToken,
        string calendarId = "primary")
    {
        var requestUri = _configuration["Authentication:Google:CalendarBaseUrl"]
                         ?? throw new InfrastructureException(
                             "A URL do Google Calendar não está configurada.");

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"{requestUri}/calendars/{Uri.EscapeDataString(calendarId)}/events");

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        request.Content = JsonContent.Create(createGoogleCalendarEventRequest);

        using var response = await _httpClient.SendAsync(
            request,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError($"O Google Calendar recusou a criação do evento: {error}");
            return null;
        }

        return await response.Content.ReadFromJsonAsync<GoogleCalendarEventResponse>(
            cancellationToken: cancellationToken);

    }


  
}
