using Microsoft.Extensions.Logging;
using Myce.Extensions;
using Myce.Wrappers.Contracts;

namespace Kennis.Domain
{
   public interface ILogService
   {
      bool LoadMessages(string language);
      void LogCritical(string category, string key, params object[] args);
      void LogError(string category, string key, params object[] args);
      void LogError(Exception exception, string category, string key, params object[] args);
      void LogInfo(string category, string key, params object[] args);
   }

   public class LogService : ILogService
   {
      private readonly ILoadService _loadService;
      private readonly ILogger<LogService> _logger;
      private Dictionary<string, Dictionary<string, string>> _logMessages;

      public bool IsMessagesLoaded => _logMessages.Count.EqualZero();

      public LogService(ILoadService loadService,
         ILogger<LogService> logger)
      {
         _loadService = loadService;
         _logger = logger;
         _logMessages = new Dictionary<string, Dictionary<string, string>>(1);
      }

      public bool LoadMessages(string language) {        
         _logMessages = _loadService.LogMessages(language);

         return _logMessages.Count.EqualZero();
      }

      private string GetMessage(string category, string key)
      {
         return _logMessages.ContainsKey(category) && _logMessages[category].ContainsKey(key)
                   ? _logMessages[category][key]
                   : $"[{key}]";
      }

      public void LogCritical(string category, string key, params object[] args)
      {
         _logger.LogCritical(GetMessage(category, key), args);
      }

      public void LogError(string category, string key, params object[] args)
      {
         _logger.LogError(GetMessage(category, key), args);
      }

      public void LogInfo(string category, string key, params object[] args)
      {
         _logger.LogInformation(GetMessage(category, key), args);
      }

      void ILogService.LogError(Exception exception, string category, string key, params object[] args)
      {
         _logger.LogError(exception, GetMessage(category, key), args);
      }
   }
}
