using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyServicesTelegramBotApiConnect.Connect.ServicePartDetails;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using MyServicesTelegramBotDTO.ObjectsDTO.ServicePartDTO;
using MyServicesTelegramBot.HelpLibrary;

namespace MyServicesTelegramBot.FormParts
{
    public static class clsServicePartDetailsTitlePart
    {
        public static async Task<Message> ShowServicePartDetailsTitles(int ServicePartID, ITelegramBotClient Bot, long ChatID,string To)
        {
            var LoadMessage = await Bot.SendTextMessageAsync(
                ChatID,
                await clsTranslateHelp.TranslateFromEnglish("🎯 **Loading Service Details...**",To),
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: null
            );

            var Result = await clsServicePartDetailsConnect.GetServicePartDetailsTitles(ServicePartID);

            if (Result.Item2 != null)
            {
                await Bot.EditMessageTextAsync(
                    ChatID,
                    LoadMessage.MessageId,
                    await clsTranslateHelp.TranslateFromEnglish
                    ("❌ **Service Unavailable**\n\nWe're unable to load the service details at this moment. Please try again later.",To),
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
                );
                return null;
            }

            if (!Result.Item1.Any())
            {
                await Bot.EditMessageTextAsync(
                    ChatID,
                    LoadMessage.MessageId,
                     await clsTranslateHelp.TranslateFromEnglish
                     ("📭 **No Additional Details**\n\nThere are no additional details available for this component at the moment.",To),
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
                );
                return null;
            }

            // Create organized keyboard layout with better text handling
            var detailsButtons = new List<InlineKeyboardButton[]>();
            var allDetails = Result.Item1.ToList();

            // Single column layout for better readability with long text
            foreach (var detail in allDetails)
            {
                var displayText = FormatDisplayText(detail.Title);
                detailsButtons.Add(new[]
                {
                    InlineKeyboardButton.WithCallbackData(
                        await clsTranslateHelp.TranslateFromEnglish( $"📋 {displayText}",To),
                        $"{detail.ServicePartDetailsID}:ServicePartDetailTitle"
                    )
                });
            }

            // Add navigation buttons
            detailsButtons.Add(new[]
            {
                InlineKeyboardButton.WithCallbackData( await clsTranslateHelp.TranslateFromEnglish("◀️ Back to Components",To), "Back"),
                InlineKeyboardButton.WithCallbackData( await clsTranslateHelp.TranslateFromEnglish("🏠 Main Menu",To), "MainMenu")
            });

            var detailsKeyboard = new InlineKeyboardMarkup(detailsButtons);

            try
            {
                await Bot.DeleteMessageAsync(ChatID, LoadMessage.MessageId);

                var message = await Bot.SendTextMessageAsync(
                    ChatID,
                    await clsTranslateHelp.TranslateFromEnglish($"📖 **Service Component Details**\n\n" +
                    $"🔍 **{allDetails.Count} detail{(allDetails.Count > 1 ? "s" : "")} available**\n\n" +
                    "Select a detail to view more information:",To),
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: detailsKeyboard
                );

                return message;
            }
            catch (Exception ex)
            {
                await Bot.EditMessageTextAsync(
                    ChatID,
                    LoadMessage.MessageId,
                     await clsTranslateHelp.TranslateFromEnglish
                     ("⚠️ **Temporary Issue**\n\nWe're experiencing a temporary problem. Please try again in a moment.",To),
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
                );
                return null;
            }
        }

        private static string FormatDisplayText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return "Untitled";

            // Clean up the text
            return text.Trim();
        }
    }
}