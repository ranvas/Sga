using Integrators.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TgBot.Abstractions
{
    public interface IDispatchedCommand<TBotService, TDispatcher> : IBotCommand<TBotService>
        where TBotService : IBotAdapter
        where TDispatcher : IDispatcher
    {
        Task<bool> ExecuteBeforeAsync(string? param, Update executionContext, TBotService service, TDispatcher dispatcher);
        Task<string> ExecuteAfterAsync(string? param, Update executionContext, TBotService service, TDispatcher dispatcher);
    }
}
