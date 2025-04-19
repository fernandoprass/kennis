using Serilog;

public static class LoggerExtensions
{
    public static void LogInformation(string message, bool showOnConsole)
    {
        if (showOnConsole)
        {
            Log.Information(message);
        }
        else
        {
     //       Log.ForContext("ShowOnConsole", showOnConsole)
       //        .WriteTo.File("KennisDemo.log")
         //      .CreateLogger()
           //    .Information(message);
        }
    }
}
