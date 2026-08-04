using System;
using System.Collections.Generic;
using System.Text;
using Tasker.Application.DTOs.Auth;

namespace Tasker.Application.Usecases.GoogleClientUsecase
{
    public interface IGoogleClientUsecase
    {
        Task<OAuth2GoogleValidatorResponse> Execute(
            string idToken, 
            string clientId, 
            string deviceId, 
            string deviceInfo,
            CancellationToken cancellationToken
            );
    }
}