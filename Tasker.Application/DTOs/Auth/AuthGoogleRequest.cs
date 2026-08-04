using System;
using System.Collections.Generic;
using System.Text;

namespace Tasker.Application.DTOs.Auth
{
    public record AuthGoogleRequest
    {
        public string IdToken { get; init; } = string.Empty;
        public string DeviceId { get; init; } = string.Empty;
        public string DeviceInfo { get; init; } = string.Empty;
    }
}
