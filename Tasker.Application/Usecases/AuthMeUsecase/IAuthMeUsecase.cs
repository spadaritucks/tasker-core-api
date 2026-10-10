using Tasker.Application.DTOs.Auth;

namespace Tasker.Application.Usecases.AuthMeUsecase;

public interface IAuthMeUsecase
{
    Task<AuthMeResponse> AuthMe(
        Guid userId,
        string accessToken,
        DateTime expiresAt,
        CancellationToken cancellationToken);
}
