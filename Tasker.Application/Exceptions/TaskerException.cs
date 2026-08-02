using System.Net;

namespace Tasker.API.Middleware;

public abstract class TaskerException : Exception
{
    public TaskerException(string errorMessage) : base(errorMessage)
    {
    }

    public abstract List<string> GetErrors();
}