namespace RestApiDemo;

public class EmailNotificationService : INotificationService
{
    public string Notify(string message)
    {
        Console.WriteLine($"[Email] {message}");
        return $"[Email] {message}";
    }
}