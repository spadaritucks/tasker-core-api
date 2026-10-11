using Tasker.Domain.Repositories;

namespace Tasker.Application.Usecases.GoogleCalendarStatusUsecase;

public class GoogleCalendarStatusUsecase : IGoogleCalendarStatusUsecase
{
    private readonly IGoogleCalendarSessionsRepository _googleCalendarSessionsRepository;

    public GoogleCalendarStatusUsecase(
        IGoogleCalendarSessionsRepository googleCalendarSessionsRepository
    )
    {
        _googleCalendarSessionsRepository = googleCalendarSessionsRepository;
    }

    public async Task<bool> Execute(Guid userId, CancellationToken cancellationToken)
    {
        var userConnect = await _googleCalendarSessionsRepository.GetByUserIdAsync(userId, cancellationToken);

        if (userConnect == null)
        {
            return false;
        }

        return true;
    }
}