namespace Tasker.API.Middleware;

public class ConflictException : TaskerException
{
    public ConflictException(string errorMessage) : base(errorMessage)
    {}
    
    public override List<string> GetErrors() => new List<string> { Message };
}