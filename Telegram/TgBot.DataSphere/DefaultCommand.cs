using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TgBot.Abstractions;

namespace TgBot.DataSphere
{
    public class DefaultCommand : IBotCommand<IBotAdapter> 
    {
        public virtual string Command { get; set; } = "/default";

        public virtual async Task<string> ExecuteDispatched(string? param, Update executionContext, BotDispatchingServiceBase service)
        {
            var id = BotHelper.GetChatId(executionContext);
            var message = $"your ID: {id}";
            await service.SendTextMessage(id, message);
            return message;
        }

        public async Task<string> Execute(string? param, Update executionContext, IBotAdapter service)
        {
            if(service is BotDispatchingServiceBase)
            {
                return await ExecuteDispatched(param, executionContext, (BotDispatchingServiceBase)service);
            }
            return string.Empty;
        }
    }
}
