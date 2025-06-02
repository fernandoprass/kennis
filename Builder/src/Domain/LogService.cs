using Kennis.Builder.Constants;
using Microsoft.Extensions.Logging;
using Myce.Extensions;
using Myce.Wrappers.Contracts;
using Myce.Extensions;
using System.Text.Json;
using System;
using YamlDotNet.Core.Tokens;
using System.ComponentModel;
using System.Reflection;

namespace Kennis.Domain
{
   public interface ILogService
   {
      bool LoadMessages(string language);

      void LogCritical(string message, params object[] args);
      void LogCritical(LogCategory category, LogAction action, params object[] args);
      void LogError(LogCategory category, LogAction action, params object[] args);
      void LogError(Exception exception, LogCategory category, LogAction action, params object[] args);
      void LogInfo(LogCategory category, LogAction action, params object[] args);
      void LogTrace(LogCategory category, LogAction action, params object[] args);
   }

   public class LogService(
         ILogger<LogService> logger,
         IFileWrapper fileWrapper,
         IPathWrapper pathWrapper) : ILogService
   {
      private readonly ILogger<LogService> _logger = logger;
      private readonly IFileWrapper _fileWrapper = fileWrapper;
      private readonly IPathWrapper _pathWrapper = pathWrapper;
      private Dictionary<string, Dictionary<string, string>> _logMessages;

      public bool IsMessagesLoaded => _logMessages.Count.EqualZero();

      public bool LoadMessages(string language)
      {
         string filename = _pathWrapper.Combine(Const.Folder.LogMessages, $"{language}{Const.Extension.I18n}");
         string jsonContent = string.Empty;

         if (_fileWrapper.Exists(filename))
         {
            try
            {
               jsonContent = _fileWrapper.ReadAllText(filename);

               _logMessages = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(jsonContent);
            }
            catch (Exception ex)
            {
               LogError(ex, LogCategory.JsonFile, LogAction.DeserializeFailed, jsonContent);
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

      public void LogInfo(LogCategory category, LogAction action, params object[] args)
      {
         _logger.LogInformation(GetMessage(category, action), args);
      }
      public void LogTrace(LogCategory category, LogAction action, params object[] args)
      {
         _logger.LogTrace(GetMessage(category, action), args);
      }
   }
}
