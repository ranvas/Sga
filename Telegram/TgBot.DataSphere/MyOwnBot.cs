﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TgBot.DataSphere
{
    public class MyOwnBot : BotServiceBase
    {
        private long _root = 0;
        public MyOwnBot(string token, long root = 50789630) : base(token)
        {
            _root = root;
            Commands.TryAdd("/root_send", new RootSendCommand());
        }

        protected override sealed Task ExecuteCommand(string command, string? param, Update executionContext)
        {
            HandleCommandBefore(command, param, executionContext);
            if (GetChatId(executionContext) == _root)
                base.ExecuteCommand(command, param, executionContext);
            return HandleCommandAfter(command, param, executionContext);
        }

        protected virtual Task HandleCommandBefore(string command, string? param, Update executionContext)
        {
            return Task.CompletedTask;
        }

        protected virtual Task HandleCommandAfter(string command, string? param, Update executionContext)
        {
            return Task.CompletedTask;
        }

        protected long GetChatId(Update executionContext)
        {
            return executionContext.Message?.Chat?.Id ?? 0;
        }

    }
}