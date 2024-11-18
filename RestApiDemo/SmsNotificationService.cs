namespace RestApiDemo;

public class SmsNotificationService : INotificationService
{
    public string Notify(string message)
    {
        Console.WriteLine($"[SMS] {message}");
        return $"[SMS] {message}";
    }
}