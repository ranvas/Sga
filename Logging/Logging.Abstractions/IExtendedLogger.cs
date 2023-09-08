using System;

namespace Logging.Abstractions
{
    public interface IExtendedLogger
    {
        void Debug(string message);
        void DebugWithData(object data, string message, string property = "data");
        void Info(string message);
        void InfoWithData(object data, string message, string property = "data");
        void Warn(string message);
        void WarnWithData(object data, string message, string property = "data");
        void Error(Exception exception, string message);
        void Error(string message, Exception exception);
        void Fatal(Exception exception, string message);
        bool TryReadProperty(string key, out object? value);
        IExtendedLogger AddProperty(string key, object value);
        IExtendedLogger ConfigureName(string loggerName);
        IExtendedLogger RemoveProperty(string key);
        IExtendedLogger ClearAllProperties(string[]? instead = null);
        IExtendedLogger TruncateDataStrings(int length = 9999);
    }
}