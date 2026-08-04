namespace Tasker.Application.Exceptions;

public class InfrastructureException : TaskerException
{
    public InfrastructureException(string errorMessage) : base(errorMessage)
    {
    }

    public override List<string> GetErrors() => new() { Message };
}
