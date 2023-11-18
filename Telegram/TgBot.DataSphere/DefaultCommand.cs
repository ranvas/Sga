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
    public class DefaultCommand : IDispatchedCommand<IBotAdapter, IDispatcher>
    {
        public virtual string Command { get; set; } = "/default";
        public virtual Task<bool> ExecuteBeforeAsync(string? param, Update executionContext, IBotAdapter service, IDispatcher dispatcher)
        {
            return Task.FromResult(true);
        }

        public virtual Task<string> ExecuteAsync(string? param, Update executionContext, IBotAdapter service)
        {
            if(param  == "id")
            {
                var id = BotHelper.GetChatId(executionContext);
                var message = $"your ID: {id}";
                return Task.FromResult(message);
            }
            return Task.FromResult(param ?? string.Empty);
        }

        public virtual Task<string> ExecuteAfterAsync(string? param, Update executionContext, IBotAdapter service, IDispatcher dispatcher)
        {
            return Task.FromResult(param ?? string.Empty);
        }
    }
}
