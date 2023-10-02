using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TgBot.Abstractions;

namespace TgBot.DataSphere
{
    public class RootSendCommand : IBotCommand<IBotAdapter>
    {
        public string Command { get; set; } = "/root_send";

        public async Task<string> Execute(string? param, Update executionContext, IBotAdapter service)
        {
            var keys = GetKeys(param);
            if (keys.ContainsKey("type"))
            {
                return await ExecuteTyped(keys["type"], keys, executionContext, service);
            }
            var user = await service.GetMe();
            var message = $"bot ID: {user.Id}, your ID: {BotHelper.GetChatId(executionContext)}";
            await SendMessage(message, executionContext, service);
            return message;
        }

        private async Task<string> ExecuteTyped(string type, Dictionary<string, string> param, Update executionContext, IBotAdapter service)
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
            return string.Empty;
        }

        private async Task SendMessage(string message, Update executionContext, IBotAdapter service)
        {
            var id = BotHelper.GetChatId(executionContext);
            await SendMessage(message, id, executionContext, service);
        }

        private async Task SendMessage(string message, string tgId, Update executionContext, IBotAdapter service)
        {
            if (long.TryParse(tgId, out var parsedId))
            {
                await service.SendTextMessage(tgId, message);
            }
        }

        private async Task SendMessage(string message, long tgId, Update executionContext, IBotAdapter service)
        {
            if (tgId > 0)
            {
                await service.SendTextMessage(tgId, message);
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
                if (string.IsNullOrEmpty(item)) continue;
                var i = item.IndexOf(' ');
                if (i > 0)
                {
                    var name = item.Substring(0, i);
                    var paramString = item.Substring(i).Trim();
                    keys.Add(name, paramString);
                }
                else
                {
                    var name = item.Substring(0);
                    keys.Add(name, string.Empty);
                }
            }
            return keys;
        }
    }
}
