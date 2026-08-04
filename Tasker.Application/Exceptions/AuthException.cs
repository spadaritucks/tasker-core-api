namespace Tasker.Application.Exceptions;                                                                                                                                          


public class AuthException : TaskerException
{
    public AuthException(string errorMessage) : base(errorMessage)
    {}
    
    public override List<string> GetErrors() => new List<string> { Message };
}