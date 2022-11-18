using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TgBot.Abstractions;

namespace TgBot.DataSphere
{
    public abstract class BotCommandBase : IBotCommand<BotAdapterBase>
    {
        public abstract string? Command { get; set; }
        public abstract Task Execute(string? param, Update executionContext, BotAdapterBase service);
    }
}
