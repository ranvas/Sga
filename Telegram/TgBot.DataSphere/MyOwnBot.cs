using Integrators.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TgBot.DataSphere
{
    public class MyOwnBot : BotDispatchingServiceBase
    {
        public readonly long LogsId;
        public string BotName { get; set; } = "Случайный бот";
        public List<long> Roots { get; set; }
        public MyOwnBot(string token, long logs, List<long> roots, string name, IDispatcher dispatcher) : base(token, dispatcher)
        {
            Roots = roots;
            LogsId = logs;
            BotName = name;
            AddCommand(new RootSendCommand());
        }

        public MyOwnBot(string token, List<long> roots, IDispatcher dispatcher) : this(token, 0, roots, "Случайный бот", dispatcher) { }

        public async Task LogMessage(string message)
        {
            if (LogsId == 0) { return; }
            await SendTextMessage(LogsId, message);
        }

        protected override async sealed Task<string> ExecuteCommand(string command, string? param, Update executionContext)
        {
            if (Roots.Contains(BotHelper.GetChatId(executionContext)))
            {
                return await base.ExecuteCommand(command, param, executionContext);
            }
            var granted = await HandleCommandBefore(command, param, executionContext);
            if (granted)
            {
                var result = await base.ExecuteCommand(command, param, executionContext);
                await HandleCommandAfter(command, param, executionContext, result);
            }
            return string.Empty;
        }

        protected async virtual Task<bool> HandleCommandBefore(string command, string? param, Update executionContext)
        {
            var name = BotHelper.GetUserName(executionContext);
            var id = BotHelper.GetChatId(executionContext);
            await LogMessage($"@{name}({id})->{BotName}: {command} {param}");
            return false;
        }

        protected async virtual Task HandleCommandAfter(string command, string? param, Update executionContext, string message)
        {
            if (string.IsNullOrEmpty(message))
            { return; }
            var name = BotHelper.GetUserName(executionContext);
            var id = BotHelper.GetChatId(executionContext);
            await LogMessage($"{BotName}->@{name}({id}): {message}");
        }
    }
}
