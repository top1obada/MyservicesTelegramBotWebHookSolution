using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyServicesTelegramBot.HelpLibrary;
using MyServicesTelegramBotApiConnect.Connect.ServicePart;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace MyServicesTelegramBot.FormParts
{
    public static class clsServicePartsPart
    {

        public static async Task<Message> ShowServiceParts(int ServiceID, ITelegramBotClient Bot, long ChatID,string To)
        {
            var LoadMessage = await Bot.SendTextMessageAsync(
                ChatID,
                await clsTranslateHelp.TranslateFromEnglish( "🔄 Loading service components...",To),
                replyMarkup: null
            );

            var Result = await clsServicePartConnect.GetAllserviceParts(ServiceID);

            if (Result.Item2 != null)
            {
                await Bot.EditMessageTextAsync(
                    ChatID,
                    LoadMessage.MessageId,
                    await clsTranslateHelp.TranslateFromEnglish("❌ Unable to load service components. Please try again later.",To)
                );
                return null;
            }

            if (!Result.Item1.Any())
            {
                await Bot.EditMessageTextAsync(
                    ChatID,
                    LoadMessage.MessageId,
                    await clsTranslateHelp.TranslateFromEnglish("📭 No components available for this service.",To)
                );
                return null;
            }

            // Create organized keyboard layout (2 columns for better visual appeal)
            var servicePartButtons = new List<InlineKeyboardButton[]>();
            var allParts = Result.Item1.ToList();

            for (int i = 0; i < allParts.Count; i += 2)
            {
                var row = new List<InlineKeyboardButton>();

                // First button in row
                row.Add(InlineKeyboardButton.WithCallbackData(
                    await clsTranslateHelp.TranslateFromEnglish($"⚙️ {allParts[i].ServicePartName}",To),
                    $"{allParts[i].ServicePartID}:ServicePart"
                ));

                // Second button in row if exists
                if (i + 1 < allParts.Count)
                {
                    row.Add(InlineKeyboardButton.WithCallbackData(
                        await clsTranslateHelp.TranslateFromEnglish($"⚙️ {allParts[i + 1].ServicePartName}",To),
                        $"{allParts[i + 1].ServicePartID}:ServicePart"
                    ));
                }

                servicePartButtons.Add(row.ToArray());
            }

            // Add navigation button to go back
            servicePartButtons.Add(new[]
            {
                InlineKeyboardButton.WithCallbackData( await clsTranslateHelp.TranslateFromEnglish("🔙 Back to Services",To), "Back")
            });

            var servicePartsKeyboard = new InlineKeyboardMarkup(servicePartButtons);

            try
            {
                await Bot.DeleteMessageAsync(ChatID, LoadMessage.MessageId);

                var message = await Bot.SendTextMessageAsync(
                    ChatID,
                    await clsTranslateHelp.TranslateFromEnglish("🛠️ **Available Components**\n\nPlease select a component:",To),
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: servicePartsKeyboard
                );

                return message;
            }
            catch (Exception ex)
            {
                await Bot.EditMessageTextAsync(
                    ChatID,
                    LoadMessage.MessageId,
                    await clsTranslateHelp.TranslateFromEnglish("⚠️ Sorry, something went wrong. Please try again.",To)
                );
                return null;
            }
        }
    }
}