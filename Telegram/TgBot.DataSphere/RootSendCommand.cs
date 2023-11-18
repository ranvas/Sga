using Integrators.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TgBot.Abstractions;

namespace TgBot.DataSphere
{
    public class RootSendCommand : DefaultCommand
    {
        public override string Command { get; set; } = "/root_send";

        public async override Task<bool> ExecuteBeforeAsync(string? param, Update executionContext, IBotAdapter service, IDispatcher dispatcher)
        {
            _ = await base.ExecuteBeforeAsync(param, executionContext, service, dispatcher);
            return false;
        }

        public override async Task<string> ExecuteAsync(string? param, Update executionContext, IBotAdapter service)
        {
            if (!string.IsNullOrEmpty(param))
                return param;
            if (service is BotDispatchingServiceBase)
            {
               return await((BotDispatchingServiceBase)service).RootCommandConfig(param ?? "");
            }
            return string.Empty;
        }

    }
}
