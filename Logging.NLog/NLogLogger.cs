using Logging.Abstractions;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Logging.NLog
{
    public class NLogLogger : IExtendedLogger
    {
        #region fields, properties and constructor

        private int _truncateDataStrings;
        private Logger? _logger;
        private string _loggerName;
        private IDictionary<string, object>? _globalProperties;
        public Logger Logger => _logger ?? (_logger = LogManager.GetLogger(_loggerName));
        private string LoggerName
        {
            get
            {
                return _loggerName;
            }
            set
            {
                _loggerName = value;
                if (_logger != null)
                    //если идет попытка поменять имя, то логгер тоже меняем
                    _logger = LogManager.GetLogger(_loggerName);
            }
        }

        public NLogLogger()
        {
            _loggerName = "defaultLogger";
            //альтернативный способ конфигурирования
            //var config = new NLog.Config.LoggingConfiguration();
            //var logfile = new NLog.Targets.FileTarget() { FileName = $@"Logs\{loggerName}.txt", Name = "logfile", Layout = @"${date} ${machinename} ${level} ${message} ${event-properties:item=data} ${exception:format=toString,Data:maxInnerExceptionLevel=3} " };
            //config.LoggingRules.Add(new NLog.Config.LoggingRule("*", LogLevel.Debug, logfile));
            //NLog.LogManager.Configuration = config;
        }

        #endregion

        #region Public Actions

        public IExtendedLogger TruncateDataStrings(int length = 9999)
        {
            _truncateDataStrings = length;
            return this;
        }

        public IExtendedLogger ConfigureName(string loggerName)
        {
            LoggerName = loggerName;
            return this;
        }

        public IExtendedLogger AddProperty(string key, object value)
        {
            if (_globalProperties == null)
                _globalProperties = new Dictionary<string, object>();
            if (!_globalProperties.Keys.Contains(key))
            {
                _globalProperties.Add(key, value);
            }
            else
            {
                _globalProperties[key] = value;
            }
            return this;
        }

        public bool TryReadProperty(string key, out object? value)
        {
            if (_globalProperties != null)
            {
                return _globalProperties.TryGetValue(key, out value);
            }
            else
            {
                value = default;
                return false;
            }
        }

        public IExtendedLogger RemoveProperty(string key)
        {
            if (_globalProperties != null && _globalProperties.Keys.Contains(key))
            {
                _globalProperties.Remove(key);
            }
            return this;
        }

        public IExtendedLogger ClearAllProperties(string[]? instead = null)
        {
            if (_globalProperties == null)
                return this;
            if (instead == null)
            {
                _globalProperties.Clear();
                return this;
            }
            var temp = new Dictionary<string, object>();
            foreach (var key in instead)
            {
                if (_globalProperties.ContainsKey(key))
                {
                    temp.Add(key, _globalProperties[key]);
                }
            }
            _globalProperties = temp;
            return this;
        }

        #endregion

        #region Log
        public void Debug(string message)
        {
            Log(LogLevel.Debug, message);
        }

        public void DebugWithData(object data, string message, string property = "data")
        {
            LogWithData(LogLevel.Debug, data, message, null, property);
        }

        public void Error(Exception exception, string message)
        {
            Log(LogLevel.Error, message, exception);
        }

        public void Error(string message, Exception exception)
        {
            Error(exception, message);
        }

        public void Fatal(Exception exception, string message)
        {
            Log(LogLevel.Fatal, message, exception);
        }

        public void Info(string message)
        {
            Log(LogLevel.Info, message);
        }

        public void InfoWithData(object data, string message, string property = "data")
        {
            LogWithData(LogLevel.Info, data, message, null, property);
        }

        public void Warn(string message)
        {
            Log(LogLevel.Warn, message);
        }

        public void WarnWithData(object data, string message, string property = "data")
        {
            LogWithData(LogLevel.Warn, data, message, null, property);
        }

        private void LogWithData(LogLevel level, object data, string message, Exception? exception = null, string property = "data")
        {
            string dataString;
            var info = new LogEventInfo(level, _loggerName, message);
            try
            {
                if (data != null)
                {
                    dataString = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                    if (_truncateDataStrings > 0 && dataString.Length > _truncateDataStrings)
                    {
                        dataString = dataString.Substring(0, _truncateDataStrings);
                    }
                }
                else
                    dataString = "null";
            }
            catch (Exception)
            {
                dataString = "SerializationFailed";
                info.Properties.Add("SerializationFailed", data.GetType());
            }
            if (exception != null)
            {
                //exception as object
                info.Exception = exception;
                //exception as sting
                info.Properties.Add("exceptionString", exception.ToString());
            }
            info.Properties[property] = dataString;
            Log(info);
        }

        private void Log(LogLevel level, string message, Exception? exception = null)
        {
            var info = new LogEventInfo(level, _loggerName, message);
            if (exception != null)
            {
                //exception as object
                info.Exception = exception;
                var exceptionString = exception.ToString();
                if (exceptionString.Length > 10000)
                    exceptionString = exceptionString.Substring(0, 10000);
                //exception as sting
                info.Properties.Add("exceptionString", exceptionString);
            }
            Log(info);
        }

        private void Log(LogEventInfo info)
        {
            if (_globalProperties != null)
            {
                foreach (var property in _globalProperties)
                {
                    info.Properties.Add(property.Key, property.Value);
                }
            }
            info.LoggerName = LoggerName;
            Logger.Log(info);
        }
        #endregion
    }
}