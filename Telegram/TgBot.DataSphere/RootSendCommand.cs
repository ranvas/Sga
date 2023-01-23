using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TgBot.DataSphere
{
    public class RootSendCommand : BotCommandBase
    {
        public override string? Command { get; set; } = "root_send";

        public override Task Execute(string? param, Update executionContext, BotAdapterBase service)
        {
            var keys = GetKeys(param);
            if(keys.ContainsKey("type"))
                return ExecuteTyped(keys["type"], keys, executionContext, service);
            return Task.CompletedTask;
        }

        private async Task ExecuteTyped(string type, Dictionary<string, string> param, Update executionContext, BotAdapterBase service) 
        {
            switch (type)
            {
                case "send_message":
                    if (param.ContainsKey("message") && param.ContainsKey("tgid"))
                    {
                        await SendMessage(param["message"], param["tgid"], executionContext, service);
                    }
                    break;
                default:
                    break;
            }
        }

        private async Task SendMessage(string message, string tgId, Update executionContext, BotAdapterBase service)
        {
            if(long.TryParse(tgId, out var parsedId))
            {
                await service.SendTextMessage(tgId, message);
            }
            else
            {
                var sender = executionContext.Message?.Chat?.Id ?? 0;
                if(sender > 0)
                {
                    await service.SendTextMessage(sender, message);
                }
            }
        }

        private Dictionary<string, string> GetKeys(string? param)
        {
            if (string.IsNullOrEmpty(param))
                return new();
            var input = param.Split("--");
            var keys = new Dictionary<string, string>();
            foreach (var item in input)
            {
                if(string.IsNullOrEmpty(item)) continue;
                var split = item.Split(' ');
                if (split.Length > 1)
                    keys.Add(split[0].Trim().ToLower(), split[1].Trim());
                 else
                    keys.Add(split[0].Trim().ToLower(), string.Empty);
            }
            return keys;
        }
    }
}
