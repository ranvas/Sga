using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TgBot.Abstractions
{
    public interface IBotAdapter
    {
        Task<User> GetMe();
        Task HandleUpdateAsync(Update update);
        Task SendTextMessage(long chatId, string text);
        Task SendTextMessage(string chatId, string text);
        Task SendTextMessage(Update executionContext, string message);
        Task SendTextMessage(long chatId, string text, ParseMode mode);
        Task SendTextMessage(string chatId, string text, ParseMode mode);
    }
}