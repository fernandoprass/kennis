using Kennis.Domain;
using Kennis.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Myce.Extensions;
using Myce.Response.Messages;
using Myce.Wrappers.Contracts;
using YamlDotNet.Serialization;

namespace Kennis.Infrastructure.Logging;

public class LogService(ILogger<LogService> logger,
                        IFileWrapper fileWrapper,
                        IPathWrapper pathWrapper) : ILogService
{
   private readonly ILogger<LogService> _logger = logger;
   private readonly IFileWrapper _fileWrapper = fileWrapper;
   private readonly IPathWrapper _pathWrapper = pathWrapper;

   private Dictionary<string, Dictionary<string, string>> _logMessages = [];

   public bool IsMessagesLoaded => _logMessages.Count.EqualZero();

   public async Task<bool> LoadMessagesAsync(string language)
   {
      string filename = _pathWrapper.Combine(Const.Folder.LogMessages, $"{language}{Const.Extension.I18n}");
      string yaml = string.Empty;

      if (_fileWrapper.Exists(filename))
      {
         try
         {
            yaml = await _fileWrapper.ReadAllTextAsync(filename);
            var deserializer = new DeserializerBuilder().Build();
            _logMessages = deserializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(yaml);
         }
         catch (Exception ex)
         {
            LogError(ex, Const.LogMessageError, yaml);
         }
      }
      
      return _logMessages?.Count > 0;
   }

   private string GetMessage(LogCategory category, LogAction action)
   {
      return _logMessages.ContainsKey(category.GetDescription()) && _logMessages[category.GetDescription()].ContainsKey(action.GetDescription())
                ? _logMessages[category.GetDescription()][action.GetDescription()]
                : string.Empty;
   }

   public void LogMessagesFromValidator(string objectName, IEnumerable<Message> messages)
   {
      _logger.LogWarning(GetMessage(LogCategory.Validator, LogAction.RuleBroken), objectName);
      foreach (var message in messages)
      {
         _logger.LogError(GetMessage(LogCategory.Validator, LogAction.Message), message.Show());
      }
   }

   public void LogCritical(string message, params object[] args)
   {
      _logger.LogCritical(message, args);
   }

   public void LogCritical(LogCategory category, LogAction action, params object[] args)
   {
      _logger.LogCritical(GetMessage(category, action), args);
   }

   public void LogError(LogCategory category, LogAction action, params object[] args)
   {
      _logger.LogError(GetMessage(category, action), args);
   }

   public void LogError(Exception exception, LogCategory category, LogAction action, params object[] args)
   {
      _logger.LogError(exception, GetMessage(category, action), args);
   }

   public void LogError(Exception exception, string message, params object[] args)
   {
      _logger.LogError(exception, message, args);
   }

   public void LogInfo(LogCategory category, LogAction action, params object[] args)
   {
      _logger.LogInformation(GetMessage(category, action), args);
   }
   public void LogTrace(LogCategory category, LogAction action, params object[] args)
   {
      _logger.LogInformation(GetMessage(category, action), args);
   }

   public void LogWarning(LogCategory category, LogAction action, params object[] args)
   {
      _logger.LogWarning(GetMessage(category, action), args);
   }
}
