using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TgBot.DataSphere
{
    public abstract class BotCommandBase
    {
        public abstract string? Command { get; set; }
        public abstract Task Execute(string? param, Update executionContext, BotAdapterBase service);
    }
}
