using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tasker.Application.Services;
using Tasker.Application.Usecases.GoogleClientUsecase;
using Tasker.Application.Usecases.RegisterUserUsecase;


namespace Tasker.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IGoogleClientUsecase, GoogleClientUsecase>();
        services.AddScoped<IRegisterUserUsecase, RegisterUserUsecase>();
        services.AddScoped<JwtTokenService>();  
        return services;
    }
}