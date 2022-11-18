using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TgBot.DataSphere
{
    /// <summary>
    /// obsolete. Use BotDispatchingServiceBase instead
    /// </summary>
    public class BotServiceBase : BotAdapterBase
    {
        protected Dictionary<string, BotCommandBase> Commands { get; set; }

        public BotServiceBase()
            : this(string.Empty)
        { }

        public BotServiceBase(string token)
            : this(token, new())
        { }

        public BotServiceBase(string token, Dictionary<string, BotCommandBase> commands)
            : base(token)
        { 
            Commands = commands; 
        }

        public static BotServiceBase GetDefaultInstance() => new();

        public bool TryAddCommand(BotCommandBase command)
        {
            if (string.IsNullOrEmpty(command.Command) || Commands.ContainsKey(command.Command))
            {
                return false;
            }
            Commands.Add(command.Command, command);
            return true;
        }

        protected async override Task HandleMessage(Message message, Update executionContext)
        {
            var textMessage = message.Text;
            if (string.IsNullOrEmpty(textMessage))
                return;
            var space = textMessage.IndexOf(' ');
            string param;
            string command;
            if (space < 1)
            {
                command = textMessage;
                param = string.Empty;
            }
            else
            {
                command = textMessage.Substring(0, space);
                param = textMessage.Substring(space + 1);
            }
            await HandleCommand(command, param, executionContext);
        }

        protected virtual Task HandleCommand(string command, string? param, Update executionContext)
        {
            LogSimple($"command: {command}, param: {param}");
            if (Commands.ContainsKey(command))
            {
                Commands[command].Execute(param, executionContext, this);
            }
            return Task.CompletedTask;
        }
    }
}
