using System;
using System.Collections.Generic;
using System.Text;

namespace Tasker.Application.DTOs.Auth
{
    public record OAuth2GoogleValidatorResponse
    {
        public string AccessToken { get; init; } = string.Empty;
        public string TokenType { get; init; } = "Bearer";
        public int ExpiresIn { get; init; }
    }
}
