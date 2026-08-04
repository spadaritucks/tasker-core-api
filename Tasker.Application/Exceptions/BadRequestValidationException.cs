namespace Tasker.Application.Exceptions;                                                                                                                                          


public class BadRequestValidationException : TaskerException
{
    private readonly List<string> _errors;

    public BadRequestValidationException(List<string> errorMessages) : base(string.Empty)
    {
        _errors = errorMessages;

    }

    public override List<string> GetErrors() => _errors;
}