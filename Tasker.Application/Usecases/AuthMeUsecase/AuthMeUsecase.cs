using Tasker.Application.DTOs.Auth;
using Tasker.Application.Exceptions;
using Tasker.Domain.Repositories;

namespace Tasker.Application.Usecases.AuthMeUsecase;

public class AuthMeUsecase : IAuthMeUsecase
{
    private readonly IUserRepository _userRepository;
    private readonly ISessionRepository _sessionRepository;

    public AuthMeUsecase(
        IUserRepository userRepository,
        ISessionRepository sessionRepository)
    {
        _userRepository = userRepository;
        _sessionRepository = sessionRepository;
    }

    public async Task<AuthMeResponse> AuthMe(
        Guid userId,
        string accessToken,
        DateTime expiresAt,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            throw new AuthException("Sessão não encontrada");
        }

        if (expiresAt <= DateTime.UtcNow)
        {
            throw new AuthException("Sessão expirada");
        }

        var session = await _sessionRepository.GetActiveByAccessTokenAsync(
            accessToken,
            cancellationToken);

        if (session is null || session.UserId != userId)
        {
            throw new AuthException("Sessão inválida");
        }

        var user = await _userRepository.GetById(userId, cancellationToken);

        if (user is null)
        {
            throw new AuthException("O usuário não existe");
        }

        var expiresIn = Math.Max(
            0,
            (int)Math.Ceiling((expiresAt - DateTime.UtcNow).TotalSeconds));

        return new AuthMeResponse
        {
            Authenticated = true,
            ExpiresIn = expiresIn,
            ExpiresAt = expiresAt,
            User = new UserProfile
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                CreatedAt = user.CreatedAt
            }
        };
    }
}
