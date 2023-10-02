using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TgBot.Abstractions;

namespace TgBot.DataSphere
{
    public class DispatchingRequest : IBotCommand<BotDispatchingServiceBase>
    {
        public virtual string Command { get; set; } = "default";

        public virtual async Task<string> Execute(string? param, Update executionContext, BotDispatchingServiceBase service)
        {
            var result = await service.Dispatcher.DispatchSimple<string>(Command);
            if (string.IsNullOrEmpty(result))
            {
                return string.Empty;
            }
            await service.SendTextMessage(BotHelper.GetChatId(executionContext), $"{result}");
            return result;
        }
    }
}
