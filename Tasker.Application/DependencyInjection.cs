using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tasker.Application.Services;
using Tasker.Application.Usecases.AuthMeUsecase;
using Tasker.Application.Usecases.CreateEventUsecase;
using Tasker.Application.Usecases.GoogleCalendarConnectUsecase;
using Tasker.Application.Usecases.GoogleClientUsecase;
using Tasker.Application.Usecases.RegisterUserUsecase;


namespace Tasker.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IGoogleClientUsecase, GoogleClientUsecase>();
        services.AddScoped<IRegisterUserUsecase, RegisterUserUsecase>();
        services.AddScoped<IGoogleCalendarConnectUsecase, GoogleCalendarConnectUsecase>();
        services.AddScoped<ICreateEventUsecase, CreateEventUsecase>();
        services.AddScoped<IAuthMeUsecase, AuthMeUsecase>();
        services.AddHttpClient<GoogleCalendarService>();
        services.AddScoped<JwtTokenService>();  
        return services;
    }
}
