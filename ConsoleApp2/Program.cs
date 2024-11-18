// See https://aka.ms/new-console-template for more information

Program.asyncLocal.Value = "Hello";
Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] asyncLocal value {Program.asyncLocal.Value}");
Task.Run( () => {
    Program.asyncLocal.Value = "one";
    Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] asyncLocal value {Program.asyncLocal.Value}");

    Task.Run(() => {
        Console.WriteLine($"\t[{Thread.CurrentThread.ManagedThreadId}] asyncLocal value {Program.asyncLocal.Value}");
    });
});

Task.Run(() => {
    Thread.Sleep(2000);
    Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] asyncLocal value {Program.asyncLocal.Value}");
    Task.Run(() => {
        Console.WriteLine($"\t\t[{Thread.CurrentThread.ManagedThreadId}] asyncLocal value {Program.asyncLocal.Value}");
    });
});

Thread.Sleep( 5000 );




public partial class Program
{
    public static AsyncLocal<string> asyncLocal = new AsyncLocal<string>();
}


