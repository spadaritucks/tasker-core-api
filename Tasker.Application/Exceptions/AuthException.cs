namespace Tasker.API.Middleware;

public class AuthException : TaskerException
{
    public AuthException(string errorMessage) : base(errorMessage)
    {}
    
    public override List<string> GetErrors() => new List<string> { Message };
}