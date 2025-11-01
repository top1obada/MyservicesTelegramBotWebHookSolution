using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace MyServicesTelegramBot.FormParts
{
    public static class LanguagesPart
    {

        public static async Task<Message> ShowLanguages(ITelegramBotClient Bot, long ChatID)
        {
            string Text = "Please Choose Your Language: ";

            var LanguagesKeyBoard = new InlineKeyboardMarkup(

            new InlineKeyboardButton [] { InlineKeyboardButton.WithCallbackData("English", "en:Language"),
            InlineKeyboardButton.WithCallbackData("العربية", "ar:Language")}
                );


            var Message = await Bot.SendTextMessageAsync(chatId: ChatID, text: Text, replyMarkup: LanguagesKeyBoard);

            return Message;

        }

    }
}
