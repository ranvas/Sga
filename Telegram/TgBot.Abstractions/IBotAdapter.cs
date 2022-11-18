﻿using Telegram.Bot.Types;

namespace TgBot.Abstractions
{
    public interface IBotAdapter
    {
        Task<User> GetMe();
        Task HandleUpdateAsync(Update update);
        Task SendTextMessage(long chatId, string text);
        Task SendTextMessage(string chatId, string text);
    }
}