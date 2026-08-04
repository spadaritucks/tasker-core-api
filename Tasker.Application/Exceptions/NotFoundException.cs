namespace Tasker.Application.Exceptions;                                                                                                                                          



public class NotFoundException : TaskerException
{
    public NotFoundException(string errorMessage) : base(errorMessage)
    {}
    
    public override List<string> GetErrors() => new List<string> { Message };
}