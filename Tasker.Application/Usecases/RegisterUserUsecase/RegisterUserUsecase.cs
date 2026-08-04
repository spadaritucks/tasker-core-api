using Microsoft.AspNetCore.Identity;
using Tasker.Application.DTOs.User;
using Tasker.Application.Exceptions;
using Tasker.Domain.Entities;
using Tasker.Domain.Repositories;

namespace Tasker.Application.Usecases.RegisterUserUsecase;

public class RegisterUserUsecase : IRegisterUserUsecase
{
    private readonly IUserRepository _userRepository;

    public RegisterUserUsecase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public async Task<UserRegisterResponse> Execute(UserRegisterRequest userRegisterRequest, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetByEmailAsync(userRegisterRequest.Email, cancellationToken);

        if (existingUser is not null)
        {
            throw new ConflictException("E-mail já cadastrado");
        }
        

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = userRegisterRequest.Name,
            Email = userRegisterRequest.Email,
            CreatedAt = DateTime.UtcNow
        };
        

        await _userRepository.AddAsync(user, cancellationToken);

        return new UserRegisterResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            CreatedAt = user.CreatedAt
        };
    }
}