namespace XsltLive;

public static class Logger
{
    public static void LogError(string message, Exception exception = null)
    {
        var temp = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[{DateTime.Now:g}] - {message}");
        Console.ForegroundColor = temp;
        if (exception is not null)
        {
            Console.WriteLine(exception);
        }
    }

    public static void LogInfo(string message)
    {
        var temp = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine($"[{DateTime.Now:dd-MM-yyyy HH:mm:ss}] - {message}");
        Console.ForegroundColor = temp;
    }
}