using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TcddBiletBot.TelegramBot
{
    public class Send
    {

        public async Task Message(ITelegramBotClient client, Update update, string messageText)
        {

#if DEBUG
            await client.SendTextMessageAsync(
                 chatId: update.Message.Chat.Id,
                 text: messageText
                 );

#else
            if (update.Message.Chat.Id == 521611981 || update.Message.Chat.Id == 1311218759)
            {
                var botClient = new TelegramBotClient("5194916816:AAHFHM-qULbrsZvsAokjgkbKVwgLgiQ8rhM");

                List<int> telegramID = new List<int>() { 521611981, 1311218759 };

                foreach (var item in telegramID)
                {
                    await botClient.SendTextMessageAsync(item, messageText);
                }
            }
            else
            {
                await client.SendTextMessageAsync(
                     chatId: update.Message.Chat.Id,
                     text: messageText
                     );
            }
#endif

        }

    }
}
