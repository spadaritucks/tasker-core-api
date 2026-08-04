using Tasker.Application.DTOs.Global;

namespace Tasker.Application.Factories;

public static class ApiResponseFactory
{
    public static GlobalApiResponseBody<T> Success<T>(T data, string path)
    {
        return new GlobalApiResponseBody<T>()
        {
            Success = true,
            Data = data,
            Error = null,
            Metadata = new MetaDataApiBody
            {
                Path = path,
                Timestamp = DateTime.UtcNow
            }
        };
    }
}