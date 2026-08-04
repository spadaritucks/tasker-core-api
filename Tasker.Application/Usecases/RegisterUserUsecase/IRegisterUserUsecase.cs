using Tasker.Application.DTOs.User;

namespace Tasker.Application.Usecases.RegisterUserUsecase;

public interface IRegisterUserUsecase
{
    Task<UserRegisterResponse> Execute(UserRegisterRequest userRegisterRequest, CancellationToken cancellationToken);
}