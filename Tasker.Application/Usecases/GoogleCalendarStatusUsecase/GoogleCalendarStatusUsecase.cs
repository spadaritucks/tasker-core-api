using Tasker.Application.Services;

namespace Tasker.Application.Usecases.GoogleCalendarStatusUsecase;

public class GoogleCalendarStatusUsecase : IGoogleCalendarStatusUsecase
{
    private readonly GoogleCalendarAccessTokenService _googleCalendarAccessTokenService;

    public GoogleCalendarStatusUsecase(
        GoogleCalendarAccessTokenService googleCalendarAccessTokenService)
    {
        _googleCalendarAccessTokenService = googleCalendarAccessTokenService;
    }

    public async Task<bool> Execute(Guid userId, CancellationToken cancellationToken)
    {
        var accessToken = await _googleCalendarAccessTokenService
            .GetValidAccessTokenAsync(userId, cancellationToken);

        return accessToken is not null;
    }
}
