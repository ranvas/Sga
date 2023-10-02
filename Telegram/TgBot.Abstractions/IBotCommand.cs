using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TgBot.Abstractions
{
    public interface IBotCommand<T> where T : IBotAdapter
    {
        public string Command { get; set; }
        public Task<string> Execute(string? param, Update executionContext, T service);
    }
}
