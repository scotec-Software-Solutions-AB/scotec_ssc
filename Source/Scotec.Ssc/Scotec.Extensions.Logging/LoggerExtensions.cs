using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Scotec.Extensions.Logging
{
    internal class LoggerExtensions
    {
    }

    namespace Scotec.Extensions.Logging
    {
        public static class LoggerExtensions
        {
            #region LogTrace

            public static void LogTrace(this ILogger logger, EventId eventId, Exception? exception,
                Func<string> messageBuilder, params object?[] args)
            {
                logger.Log(LogLevel.Trace, eventId, exception, messageBuilder, args);
            }

            public static void LogTrace(this ILogger logger, EventId eventId, Func<string> messageBuilder,
                params object?[] args)
            {
                logger.Log(LogLevel.Trace, eventId, messageBuilder, args);
            }

            public static void LogTrace(this ILogger logger, Exception? exception, Func<string> messageBuilder,
                params object?[] args)
            {
                logger.Log(LogLevel.Trace, exception, messageBuilder, args);
            }

            public static void LogTrace(this ILogger logger, Func<string> messageBuilder, params object?[] args)
            {
                logger.Log(LogLevel.Trace, messageBuilder, args);
            }

            #endregion

            #region LogDebug

            public static void LogDebug(this ILogger logger, EventId eventId, Exception? exception,
                Func<string> messageBuilder, params object?[] args)
            {
                logger.Log(LogLevel.Debug, eventId, exception, messageBuilder, args);
            }

            public static void LogDebug(this ILogger logger, EventId eventId, Func<string> messageBuilder,
                params object?[] args)
            {
                logger.Log(LogLevel.Debug, eventId, messageBuilder, args);
            }

            public static void LogDebug(this ILogger logger, Exception? exception, Func<string> messageBuilder,
                params object?[] args)
            {
                logger.Log(LogLevel.Debug, exception, messageBuilder, args);
            }

            public static void LogDebug(this ILogger logger, Func<string> messageBuilder, params object?[] args)
            {
                logger.Log(LogLevel.Debug, messageBuilder, args);
            }

            #endregion

            #region LogInformation

            public static void LogInformation(this ILogger logger, EventId eventId, Exception? exception,
                Func<string> messageBuilder, params object?[] args)
            {
                logger.Log(LogLevel.Information, eventId, exception, messageBuilder, args);
            }

            public static void LogInformation(this ILogger logger, EventId eventId, Func<string> messageBuilder,
                params object?[] args)
            {
                logger.Log(LogLevel.Information, eventId, messageBuilder, args);
            }

            public static void LogInformation(this ILogger logger, Exception? exception, Func<string> messageBuilder,
                params object?[] args)
            {
                logger.Log(LogLevel.Information, exception, messageBuilder, args);
            }

            public static void LogInformation(this ILogger logger, Func<string> messageBuilder, params object?[] args)
            {
                logger.Log(LogLevel.Information, messageBuilder, args);
            }

            #endregion

            #region LogWarning

            public static void LogWarning(this ILogger logger, EventId eventId, Exception? exception,
                Func<string> messageBuilder, params object?[] args)
            {
                logger.Log(LogLevel.Warning, eventId, exception, messageBuilder, args);
            }

            public static void LogWarning(this ILogger logger, EventId eventId, Func<string> messageBuilder,
                params object?[] args)
            {
                logger.Log(LogLevel.Warning, eventId, messageBuilder, args);
            }

            public static void LogWarning(this ILogger logger, Exception? exception, Func<string> messageBuilder,
                params object?[] args)
            {
                logger.Log(LogLevel.Warning, exception, messageBuilder, args);
            }

            public static void LogWarning(this ILogger logger, Func<string> messageBuilder, params object?[] args)
            {
                logger.Log(LogLevel.Warning, messageBuilder, args);
            }

            #endregion

            #region LogError

            public static void LogError(this ILogger logger, EventId eventId, Exception? exception,
                Func<string> messageBuilder, params object?[] args)
            {
                logger.Log(LogLevel.Error, eventId, exception, messageBuilder, args);
            }

            public static void LogError(this ILogger logger, EventId eventId, Func<string> messageBuilder,
                params object?[] args)
            {
                logger.Log(LogLevel.Error, eventId, messageBuilder, args);
            }

            public static void LogError(this ILogger logger, Exception? exception, Func<string> messageBuilder,
                params object?[] args)
            {
                logger.Log(LogLevel.Error, exception, messageBuilder, args);
            }

            public static void LogError(this ILogger logger, Func<string> messageBuilder, params object?[] args)
            {
                logger.Log(LogLevel.Error, messageBuilder, args);
            }

            #endregion

            #region LogCritical

            public static void LogCritical(this ILogger logger, EventId eventId, Exception? exception,
                Func<string> messageBuilder, params object?[] args)
            {
                logger.Log(LogLevel.Critical, eventId, exception, messageBuilder, args);
            }

            public static void LogCritical(this ILogger logger, EventId eventId, Func<string> messageBuilder,
                params object?[] args)
            {
                logger.Log(LogLevel.Critical, eventId, messageBuilder, args);
            }

            public static void LogCritical(this ILogger logger, Exception? exception, Func<string> messageBuilder,
                params object?[] args)
            {
                logger.Log(LogLevel.Critical, exception, messageBuilder, args);
            }

            public static void LogCritical(this ILogger logger, Func<string> messageBuilder, params object?[] args)
            {
                logger.Log(LogLevel.Critical, messageBuilder, args);
            }

            #endregion

            #region Log

            public static void Log(this ILogger logger, LogLevel logLevel, Func<string> messageBuilder,
                params object?[] args)
            {
                logger.Log(logLevel, 0, null, messageBuilder, args);
            }

            public static void Log(this ILogger logger, LogLevel logLevel, EventId eventId, Func<string> messageBuilder,
                params object?[] args)
            {
                logger.Log(logLevel, eventId, null, messageBuilder, args);
            }

            public static void Log(this ILogger logger, LogLevel logLevel, Exception? exception,
                Func<string> messageBuilder, params object?[] args)
            {
                logger.Log(logLevel, 0, exception, messageBuilder, args);
            }

            public static void Log(this ILogger logger, LogLevel logLevel, EventId eventId, Exception? exception,
                Func<string> messageBuilder, params object?[] args)
            {
                if (logger == null)
                {
                    throw new ArgumentNullException(nameof(logger));
                }

                if (logger.IsEnabled(logLevel))
                {
                    logger.Log(logLevel, eventId, exception, messageBuilder(), args);
                }
            }
            
            #endregion
        }
    }
}