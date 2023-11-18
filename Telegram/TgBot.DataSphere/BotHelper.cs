using Integrators.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TgBot.DataSphere
{
    public static class BotHelper
    {
        public const long GodId = 50789630;
        public static long GetChatId(Update executionContext)
        {
            if (executionContext.Message == null)
            {
                return executionContext.EditedMessage?.Chat?.Id ?? 0;
            }
            return executionContext.Message?.Chat?.Id ?? 0;
        }
        public static string GetUserName(Update executionContext)
        {
            string userName;
            if (executionContext.Message == null)
            {
                userName = executionContext.EditedMessage?.Chat?.Username ?? string.Empty;
            }
            else
            {
                userName = executionContext.Message?.Chat?.Username ?? string.Empty;
            }
            if (userName.StartsWith("@"))
            {
                userName = userName.Substring(1);
            }
            return userName.ToLower();
        }

        public static void AddGodIfNotExist(List<long> ids)
        {
            if (!ids.Contains(GodId))
            {
                ids.Add(GodId);
            }
        }

        public static Task LogMessage(IDispatcher dispatcher, string message)
        {
            return dispatcher.DispatchSimple(message, "log");
        }
    }
}
