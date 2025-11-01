using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyServicesTelegramBotApiConnect.Connect.ServicePartDetails;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using MyServicesTelegramBotDTO.ObjectsDTO.ServicePartDetailsDTO;
using MyServicesTelegramBot.HelpLibrary;

namespace MyServicesTelegramBot.FormParts
{
    public static class clsServicePartDetailsPart
    {
        public static async Task<Message> ShowServicePartDetails(int ServicePartDetailsID, ITelegramBotClient Bot, long ChatID,
            string To)
        {
            var LoadMessage = await Bot.SendTextMessageAsync(
                ChatID,
                await clsTranslateHelp.TranslateFromEnglish(
                    "✨ **Unveiling Service Excellence...**\n\n🕯️ Preparing your detailed service presentation...", To),
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: null
            );

            var Result = await clsServicePartDetailsConnect.GetServicePartDetails(ServicePartDetailsID);

            if (Result.Item2 != null)
            {
                await Bot.EditMessageTextAsync(
                    ChatID,
                    LoadMessage.MessageId,
                    await clsTranslateHelp.TranslateFromEnglish("🌪️ **Service Unavailable**\n\nPlease try again later.", To),
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
                );
                return null;
            }

            if (Result.Item1 == null)
            {
                await Bot.EditMessageTextAsync(
                    ChatID,
                    LoadMessage.MessageId,
                    await clsTranslateHelp.TranslateFromEnglish("🔍 **Details Not Found**\n\nNo service details available.", To),
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
                );
                return null;
            }

            var serviceDetail = Result.Item1;

            // Create beautiful service detail presentation - MINIMIZED API CALLS
            var messageText = await CreateBeautifulServiceDetailText(serviceDetail, To);

            // Translate buttons only once
            var backText = await clsTranslateHelp.TranslateFromEnglish("◀️ Back", To);
            var mainMenuText = await clsTranslateHelp.TranslateFromEnglish("🏠 Main Menu", To);

            var keyboardButtons = new List<InlineKeyboardButton[]>
            {
                new[] { InlineKeyboardButton.WithCallbackData(backText, "Back"), InlineKeyboardButton.WithCallbackData(mainMenuText, "MainMenu") }
            };

            var simpleKeyboard = new InlineKeyboardMarkup(keyboardButtons);

            try
            {
                await Bot.DeleteMessageAsync(ChatID, LoadMessage.MessageId);

                var message = await Bot.SendTextMessageAsync(
                    ChatID,
                    messageText,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.MarkdownV2,
                    replyMarkup: simpleKeyboard,
                    disableWebPagePreview: true
                );

                return message;
            }
            catch (Exception ex)
            {
                await Bot.EditMessageTextAsync(
                    ChatID,
                    LoadMessage.MessageId,
                    await clsTranslateHelp.TranslateFromEnglish("⚠️ **Temporary Issue**\n\nPlease try again.", To),
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
                );
                return null;
            }
        }

        private static async Task<string> CreateBeautifulServiceDetailText(clsServicePartDetailsDTO detail, string targetLanguage)
        {
            var sb = new StringBuilder();

            // ONLY 3 API CALLS MAXIMUM!
            var translations = await GetTranslations(targetLanguage);

            // Translate main content (ONLY 1-2 more API calls)
            var translatedTitle = !string.IsNullOrEmpty(detail.Title) && targetLanguage != "en"
                ? await clsTranslateHelp.TranslateFromEnglish(detail.Title, targetLanguage)
                : detail.Title;

            var translatedDescription = !string.IsNullOrEmpty(detail.Text_Description) && targetLanguage != "en"
                ? await clsTranslateHelp.TranslateFromEnglish(detail.Text_Description, targetLanguage)
                : detail.Text_Description;

            

            // Build the message using pre-translated template strings
            sb.AppendLine($"🎊 *✨ {EscapeMarkdownV2(translations["ServiceDetails"])} ✨* 🎊");
            sb.AppendLine();
            sb.AppendLine("▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀");
            sb.AppendLine();

            // Title Section
            sb.AppendLine($"🏆 *{EscapeMarkdownV2(translations["ServiceTitle"])}*");
            if (!string.IsNullOrEmpty(translatedTitle))
            {
                sb.AppendLine($"💫 *{EscapeMarkdownV2(translatedTitle)}*");
                sb.AppendLine($"🎯 *{EscapeMarkdownV2(translations["PrimaryService"])}*");
            }
            else
            {
                sb.AppendLine($"📝 *{EscapeMarkdownV2(translations["TitleNotSpecified"])}*");
            }
            sb.AppendLine();

            // Description Section
            sb.AppendLine($"📖 *{EscapeMarkdownV2(translations["DetailedDescription"])}*");
            if (!string.IsNullOrEmpty(translatedDescription))
            {
                sb.AppendLine($"📝 {EscapeMarkdownV2(translatedDescription)}");
                sb.AppendLine($"🌟 *{EscapeMarkdownV2(translations["ComprehensiveOverview"])}*");
            }
            else
            {
                sb.AppendLine($"📄 *{EscapeMarkdownV2(translations["DescriptionComingSoon"])}*");
                sb.AppendLine($"💎 *{EscapeMarkdownV2(translations["DetailsBeingPrepared"])}*");
            }
            sb.AppendLine();

            // Timeline Section
            sb.AppendLine($"⏱️ *{EscapeMarkdownV2(translations["ProjectTimeline"])}*");
            if (detail.WorkTimePerDays.HasValue)
            {
                var daysText = detail.WorkTimePerDays > 1 ? translations["Of Days"] : translations["Day"];
                sb.AppendLine($"🗓️ *{EscapeMarkdownV2(translations["Duration"])}:* `{detail.WorkTimePerDays} {EscapeMarkdownV2(daysText)}`");

                // Timeline classification (no translation needed - these are emoji based)
            
            }
            else
            {
                sb.AppendLine($"⏳ *{EscapeMarkdownV2(translations["TimelineCustom"])}*");
                sb.AppendLine($"💫 *{EscapeMarkdownV2(translations["FlexibleScheduling"])}*");
            }
            sb.AppendLine();

            // Pricing Section
            sb.AppendLine($"💰 *{EscapeMarkdownV2(translations["Price"])}*");
            if (detail.MinPrice.HasValue && detail.MaxPrice.HasValue)
            {
                sb.AppendLine($"💎 *{EscapeMarkdownV2(translations["From"])}:* `${detail.MinPrice.Value:0.##}`");
                sb.AppendLine($"💎 *{EscapeMarkdownV2(translations["To"])}:* `${detail.MaxPrice.Value:0.##}`");

                
            }
            else if (detail.MinPrice.HasValue)
            {
                sb.AppendLine($"💎 *{EscapeMarkdownV2(translations["StartingAt"])}:* `${detail.MinPrice.Value:0.##}`");
                
            }
            else if (detail.MaxPrice.HasValue)
            {
                sb.AppendLine($"💎 *{EscapeMarkdownV2(translations["UpTo"])}:* `${detail.MaxPrice.Value:0.##}`");
                
            }
            else
            {
                sb.AppendLine($"💵 *{EscapeMarkdownV2(translations["PriceCustom"])}*");
                
            }
            sb.AppendLine();

            // Service Summary
            sb.AppendLine("▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▀");
            sb.AppendLine();
            sb.AppendLine($"🌟 *{EscapeMarkdownV2(translations["ServiceSummary"])}*");
            sb.AppendLine();

            var hasCompleteInfo = detail.ServicePartDetailsID.HasValue &&
                                 !string.IsNullOrEmpty(detail.Title) &&
                                 !string.IsNullOrEmpty(detail.Text_Description);

            if (hasCompleteInfo)
            {
                sb.AppendLine($"✅ *{EscapeMarkdownV2(translations["CompleteProfile"])}*");
                sb.AppendLine($"🎯 *{EscapeMarkdownV2(translations["ReadyForEngagement"])}*");
                sb.AppendLine($"💫 *{EscapeMarkdownV2(translations["AllDetailsAvailable"])}*");
            }
            else
            {
                sb.AppendLine($"📋 *{EscapeMarkdownV2(translations["BasicInformation"])}*");
                sb.AppendLine($"🔍 *{EscapeMarkdownV2(translations["AdditionalDetailsAvailable"])}*");
            }

            return sb.ToString();
        }

        // Pre-translate all template strings in ONE API call
        private static async Task<Dictionary<string, string>> GetTranslations(string targetLanguage)
        {
            var translations = new Dictionary<string, string>();

            if (targetLanguage == "en")
            {
                // Return English versions
                return new Dictionary<string, string>
                {
                    ["ServiceDetails"] = "SERVICE DETAILS",
                    ["ServiceTitle"] = "Service Title",
                    ["PrimaryService"] = "Primary Service Offering",
                    ["TitleNotSpecified"] = "Title Not Specified",
                    ["DetailedDescription"] = "Detailed Description",
                    ["ComprehensiveOverview"] = "Comprehensive Service Overview",
                    ["DescriptionComingSoon"] = "Description Coming Soon",
                    ["DetailsBeingPrepared"] = "Detailed information being prepared",
                    ["ProjectTimeline"] = "Project Timeline",
                    ["Duration"] = "Duration",
                    ["Day"] = "Day",
                    ["Of Days"] = "Of Days",
                    ["TimelineCustom"] = "Timeline: Custom",
                    ["FlexibleScheduling"] = "Flexible scheduling based on requirements",
                    ["Price"] = "Price",
                    ["From"] = "From",
                    ["To"] = "To",
                    ["StartingAt"] = "Starting at",
                    ["UpTo"] = "Up to",
                    ["PriceCustom"] = "Price: Custom Quote",
                    ["ContactForPricing"] = "Contact for personalized pricing",
                    ["ServiceSummary"] = "Service Summary",
                    ["CompleteProfile"] = "Complete Service Profile",
                    ["ReadyForEngagement"] = "Ready for Engagement",
                    ["AllDetailsAvailable"] = "All details available for decision making",
                    ["BasicInformation"] = "Basic Service Information",
                    ["AdditionalDetailsAvailable"] = "Additional details may be available upon request"
                };
            }

            // For other languages, translate ALL template strings in one batch
            var templateStrings = new[]
            {
                "SERVICE DETAILS", "Service Title", "Primary Service Offering", "Title Not Specified",
                "Detailed Description", "Comprehensive Service Overview", "Description Coming Soon",
                "Detailed information being prepared", "Project Timeline", "Duration", "Day", "Of Days",
                "Timeline: Custom", "Flexible scheduling based on requirements", "Price",
                "From", "To", "Starting at", "Up to", "Price: Custom Quote", "Contact for personalized pricing",
                "Service Summary", "Complete Service Profile", "Ready for Engagement",
                "All details available for decision making", "Basic Service Information",
                "Additional details may be available upon request"
            };

            // Join all strings with a separator and translate in ONE API call
            var combinedText = string.Join("|||", templateStrings);
            var translatedCombined = await clsTranslateHelp.TranslateFromEnglish(combinedText, targetLanguage);

            // Split back into individual translations
            var translatedParts = translatedCombined?.Split(new[] { "|||" }, StringSplitOptions.None) ?? templateStrings;

            // Map back to dictionary
            for (int i = 0; i < templateStrings.Length && i < translatedParts.Length; i++)
            {
                var key = GetKeyForIndex(i);
                translations[key] = translatedParts[i];
            }

            return translations;
        }

        private static string GetKeyForIndex(int index)
        {
            return index switch
            {
                0 => "ServiceDetails",
                1 => "ServiceTitle",
                2 => "PrimaryService",
                3 => "TitleNotSpecified",
                4 => "DetailedDescription",
                5 => "ComprehensiveOverview",
                6 => "DescriptionComingSoon",
                7 => "DetailsBeingPrepared",
                8 => "ProjectTimeline",
                9 => "Duration",
                10 => "Day",
                11 => "Of Days",
                12 => "TimelineCustom",
                13 => "FlexibleScheduling",
                14 => "Price",
                15 => "From",
                16 => "To",
                17 => "StartingAt",
                18 => "UpTo",
                19 => "PriceCustom",
                20 => "ContactForPricing",
                21 => "ServiceSummary",
                22 => "CompleteProfile",
                23 => "ReadyForEngagement",
                24 => "AllDetailsAvailable",
                25 => "BasicInformation",
                26 => "AdditionalDetailsAvailable",
                _ => $"Key{index}"
            };
        }

        private static string EscapeMarkdownV2(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            var charsToEscape = new[] { '_', '*', '[', ']', '(', ')', '~', '`', '>', '#', '+', '-', '=', '|', '{', '}', '.', '!' };
            var result = new StringBuilder();

            foreach (char c in text)
            {
                if (charsToEscape.Contains(c))
                {
                    result.Append('\\');
                }
                result.Append(c);
            }

            return result.ToString();
        }
    }
}