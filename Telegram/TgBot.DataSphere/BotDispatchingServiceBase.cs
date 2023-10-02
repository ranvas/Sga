using Integrators.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TgBot.Abstractions;

namespace TgBot.DataSphere
{
    public class BotDispatchingServiceBase : BotAdapterBase, IBotAdapter
    {
        public IDispatcher Dispatcher;
        protected Dictionary<string, IBotCommand<IBotAdapter>> Requests { get; set; } = new();
        public BotDispatchingServiceBase(string token, IDispatcher dispatcher) : base(token)
        {
            Dispatcher = dispatcher;
            AddCommand(new DefaultCommand());
        }

        public void AddCommand(IBotCommand<IBotAdapter> command)
        {
            if (Requests.ContainsKey(command.Command))
            { 
                return;
            }
            Requests.Add(command.Command, command);
        }

        protected async override Task HandleMessage(Message message, Update executionContext)
        {
            var textMessage = message.Text;
            if (string.IsNullOrEmpty(textMessage))
                return;
            var space = textMessage.IndexOf(' ');
            string param;
            string command;
            if (space < 0)
            {
                command = textMessage;
                param = string.Empty;
            }
            else
            {
                command = textMessage.Substring(0, space);
                param = textMessage.Substring(space + 1);
            }
            await ExecuteCommand(command, param, executionContext);
        }

        protected async virtual Task<string> ExecuteCommand(string command, string? param, Update executionContext)
        {
            if (Requests.ContainsKey(command))
            {
                return await Requests[command].Execute(param, executionContext, this);
            }
            return string.Empty;
        }
    }
}
