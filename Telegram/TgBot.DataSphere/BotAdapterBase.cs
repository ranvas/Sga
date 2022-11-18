using Microsoft.VisualBasic;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TgBot.Abstractions;

namespace TgBot.DataSphere
{
    public class BotAdapterBase : IBotAdapter
    {
        TelegramBotClient _bot;
        protected CancellationTokenSource CancellationToken;
        public BotAdapterBase(string token)
        {
            _bot = new TelegramBotClient(token);
            CancellationToken = new CancellationTokenSource();
        }

        #region StartReceiving

        public void StartReceiving()
        {
            LogSimple("StartReceiving bot ");
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
            };
            _bot.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: CancellationToken.Token
            );
        }

        public void StartReceiving<T>() where T : class
        {
            LogSimple("StartReceiving bot ");
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
            };
            _bot.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: CancellationToken.Token
            );
        }

        Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            await HandleUpdateAsync(update, cancellationToken);
        }

        /// <summary>
        /// At most one of the optional parameters can be present in any given update.
        /// </summary>
        async Task HandleUpdateAsync(Update update, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;
            if (update.Message is { } message)
            {
                LogSimple("handle message");
                try
                {
                    await HandleMessage(message, update);
                }
                catch (Exception e)
                {
                    LogSimple(e.ToString());
                }
                return;
            }
            if (update.EditedMessage is { } editedMessage)
            {
                LogSimple("handle edited message");
                try
                {
                    await HandleEditedMessage(editedMessage, update);
                }
                catch (Exception e)
                {
                    LogSimple(e.ToString());
                }
                return;
            }
            LogSimple($"Unknown message type");
        }

        //async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        //{
        //    await HandleUpdate(update, cancellationToken);
        //}

        #endregion

        #region public

        public async Task<User> GetMe()
        {
            return await _bot.GetMeAsync();
        }

        public async Task HandleUpdateAsync(Update update)
        {
            await HandleUpdateAsync(update, CancellationToken.Token);
        }

        public async Task SendTextMessage(long chatId, string text, ParseMode mode)
        {
            _ = await _bot.SendTextMessageAsync(
                chatId: chatId,
                text: text,
                parseMode: mode,
                cancellationToken: CancellationToken.Token);
        }

        public async Task SendTextMessage(string chatId, string text, ParseMode mode)
        {
            _ = await _bot.SendTextMessageAsync(
                chatId: chatId,
                text: text,
                parseMode: mode,
                cancellationToken: CancellationToken.Token);
        }

        public async Task SendTextMessage(long chatId, string text)
        {
            await SendTextMessage(chatId, text, ParseMode.Html);
        }

        public async Task SendTextMessage(string chatId, string text)
        {
            await SendTextMessage(chatId, text, ParseMode.Html);
        }

        #endregion

        #region protected

        protected virtual async Task HandleMessage(Message message, Update executionContext)
        {
            if (message.Text is { } messageText)
            {
                LogSimple($"text: {messageText}");
            }
            if (message.Sticker is { } sticker)
            {
                await HandleSticker(sticker, executionContext);
            }
        }

        protected virtual async Task HandleEditedMessage(Message message, Update executionContext)
        {
            await HandleMessage(message, executionContext);
        }

        protected virtual Task HandleSticker(Sticker sticker, Update executionContext)
        {
            LogSimple($"sticker, emoji: {sticker.Emoji}");
            return Task.CompletedTask;
        }

        protected virtual void LogSimple(string message)
        {
            Console.WriteLine(message);
        }

        #endregion


    }
}