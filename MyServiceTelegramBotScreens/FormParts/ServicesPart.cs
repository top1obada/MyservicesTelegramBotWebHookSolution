using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MyServicesTelegramBot.HelpLibrary;
using MyServicesTelegramBotApiConnect.Connect.Service;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace MyServicesTelegramBot.FormParts
{
    public static class clsServicesPart
    {

        public static async Task<Tuple<Message, string?>> ShowServices(ITelegramBotClient Bot, long ChatID,string To)
        {
          

            var LoadMessage = await Bot.SendTextMessageAsync(
                ChatID,
                await clsTranslateHelp.TranslateFromEnglish( "🔄 Loading services... Please wait a moment.",To ),
                replyMarkup: null
            );

            var Result = await clsServiceConnect.GetAllservices();

            if (Result.Item2 != null)
            {
                await Bot.EditMessageTextAsync(
                    ChatID,
                    LoadMessage.MessageId,
                    await clsTranslateHelp.TranslateFromEnglish("❌ Sorry, we encountered an issue while fetching services. Please try again later.",To)
                );
                return new Tuple<Message, string?>(null, Result.Item2);
            }

            if (Result.Item1.Count == 0)
            {
                await Bot.EditMessageTextAsync(
                    ChatID,
                    LoadMessage.MessageId,
                    await clsTranslateHelp.TranslateFromEnglish("📭 No services available at the moment. Please check back later!",To)
                );
                return new Tuple<Message, string?>(null, "There Is No Services Found");
            }

            var ServicesButons = new List<InlineKeyboardButton>();

            foreach (var service in Result.Item1)
            {
                ServicesButons.Add(InlineKeyboardButton.WithCallbackData(
                    await clsTranslateHelp.TranslateFromEnglish($"🔹 {service.ServiceName}",To),
                    $"{service.ServiceID}:Service"
                ));
            }

            // Organize buttons in columns of 2 for better layout
            var keyboardRows = new List<List<InlineKeyboardButton>>();
            for (int i = 0; i < ServicesButons.Count; i += 2)
            {
                var row = new List<InlineKeyboardButton>();
                row.Add(ServicesButons[i]);

                if (i + 1 < ServicesButons.Count)
                {
                    row.Add(ServicesButons[i + 1]);
                }

                keyboardRows.Add(row);
            }

            // Add back button row
            keyboardRows.Add(new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData(await clsTranslateHelp.TranslateFromEnglish("🔙 Back ",To), "Back")
            });

            var ServicesKeyBoard = new InlineKeyboardMarkup(keyboardRows);

            await Bot.DeleteMessageAsync(ChatID, LoadMessage.MessageId);

            var Message = await Bot.SendTextMessageAsync(
                ChatID,
              await clsTranslateHelp.TranslateFromEnglish("🎯 **Available Services**\n\nPlease choose one of the services below:",To),
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: ServicesKeyBoard
            );

            return new Tuple<Message, string?>(Message, null);
        }
    }
}