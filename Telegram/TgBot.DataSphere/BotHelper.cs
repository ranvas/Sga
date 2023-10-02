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
        public static long GetChatId(Update executionContext)
        {
            return executionContext.Message?.Chat?.Id ?? 0;
        }
        public static string GetUserName(Update executionContext)
        {
            var userName = executionContext.Message?.Chat?.Username ?? string.Empty;
            if (userName.StartsWith("@"))
            {
                userName = userName.Substring(1);
            }
            return userName.ToLower();
        }
    }
}
