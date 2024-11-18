namespace RestApiDemo;

public class PushNotificationService : INotificationService
{
    public string Notify(string message)
    {
        Console.WriteLine($"[Push] {message}");
        return $"[Push] {message}";
    }
}