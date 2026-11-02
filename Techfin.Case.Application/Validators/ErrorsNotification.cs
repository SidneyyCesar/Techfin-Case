namespace Techfin.Case.Application.Validators;

public class Notifications
{
    public Notifications()
    {
        messageError = string.Empty;
    }

    public Notifications(string message)
    {
        messageError = message;
        hasError = !string.IsNullOrEmpty(message);
    }
    public bool hasError { get; private set; }
    public string messageError { get; private set; }
}
