using Integrators.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TgBot.Abstractions
{
    public interface IBotDispatchingService<TBotAdapter, TDispatcher> 
        where TBotAdapter : IBotAdapter 
        where TDispatcher : IDispatcher
    {
        void AddCommand(IDispatchedCommand<TBotAdapter, TDispatcher> command);
        //Task LogMessage(string message);
        string BotName { get; set; }
    }
}
