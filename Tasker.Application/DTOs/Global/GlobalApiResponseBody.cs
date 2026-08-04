using System;
using System.Collections.Generic;
using System.Text;

namespace Tasker.Application.DTOs.Global
{
    public record GlobalApiResponseBody<T>
    {
        public bool Success { get; init; }
        public T? Data { get; init; } 
        public GlobalErrorApiBody? Error { get; init; }

        public MetaDataApiBody Metadata { get; init; } = new();


    }

    public record GlobalErrorApiBody
    {
        public string Code { get; init; } = string.Empty ;

        public string? Message { get; init; } = string.Empty;

        public List<string> Details { get; init; } = [];

    }

    public record MetaDataApiBody
    {
        public string Path { get; init; } = string.Empty;

        public DateTime Timestamp { get; init; } = new DateTime();


    }
}
