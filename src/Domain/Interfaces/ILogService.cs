using Myce.Response.Messages;

namespace Kennis.Domain.Interfaces;

public interface ILogService
{
   Task<bool> LoadMessagesAsync(string language);
   void LogMessagesFromValidator(string objectName, IEnumerable<Message> messages);

   void LogCritical(string message, params object[] args);
   void LogCritical(LogCategory category, LogAction action, params object[] args);
   void LogError(LogCategory category, LogAction action, params object[] args);
   void LogError(Exception exception, LogCategory category, LogAction action, params object[] args);
   void LogInfo(LogCategory category, LogAction action, params object[] args);
   void LogTrace(LogCategory category, LogAction action, params object[] args);
   void LogWarning(LogCategory category, LogAction action, params object[] args);
}
