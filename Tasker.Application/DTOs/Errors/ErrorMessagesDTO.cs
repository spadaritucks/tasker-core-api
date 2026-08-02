namespace Tasker.API.DTOs.Errors;

public class ErrorMessagesDTO
{
    public List<string> Errors { get;  private set; }

    public ErrorMessagesDTO(string message)
    {
        Errors = [message];
    }
    
    public ErrorMessagesDTO(List<string> messages)
    {
        Errors = messages;
    }
}