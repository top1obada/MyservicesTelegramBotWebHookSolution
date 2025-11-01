using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace MyServicesTelegramBot.HelpLibrary
{
    public static class clsTranslateHelp
    {
        // استخدام واجهة Google Translate المجانية
        public static async Task<string> TranslateAsync(string text, string from, string to)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;
            if (from == to) return text;

            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                client.Timeout = TimeSpan.FromSeconds(30);

                // Properly encode the text for URL
                var encodedText = HttpUtility.UrlEncode(text);
                var url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={from}&tl={to}&dt=t&q={encodedText}";

                var response = await client.GetStringAsync(url);

                // Parse the JSON response properly
                return ParseGoogleTranslateResponse(response, text);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Translation error: {ex.Message}");
            }

            return text;
        }

        private static string ParseGoogleTranslateResponse(string jsonResponse, string originalText)
        {
            try
            {
                // The response is a JSON array where the first element contains translation segments
                using var doc = JsonDocument.Parse(jsonResponse);
                var root = doc.RootElement;

                // The structure is: [[["translated_text","original_text",null,null,1]],null,"source_lang"]
                if (root.ValueKind == JsonValueKind.Array && root.GetArrayLength() > 0)
                {
                    var mainArray = root[0]; // Get the first array

                    if (mainArray.ValueKind == JsonValueKind.Array)
                    {
                        var translatedText = new StringBuilder();

                        // Iterate through all translation segments
                        foreach (var segment in mainArray.EnumerateArray())
                        {
                            if (segment.ValueKind == JsonValueKind.Array && segment.GetArrayLength() > 0)
                            {
                                var translatedSegment = segment[0]; // First element is the translated text
                                if (translatedSegment.ValueKind == JsonValueKind.String)
                                {
                                    var segmentText = translatedSegment.GetString();
                                    if (!string.IsNullOrEmpty(segmentText))
                                    {
                                        translatedText.Append(segmentText);
                                    }
                                }
                            }
                        }

                        var result = translatedText.ToString();
                        return string.IsNullOrEmpty(result) ? originalText : result;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"JSON parsing error: {ex.Message}");
                // Fallback to manual parsing
            }

            // Manual parsing as fallback
            try
            {
                var result = new StringBuilder();
                var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(jsonResponse));

                bool inTranslation = false;
                int depth = 0;

                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.StartArray)
                    {
                        depth++;
                    }
                    else if (reader.TokenType == JsonTokenType.EndArray)
                    {
                        depth--;
                    }
                    else if (reader.TokenType == JsonTokenType.String && depth >= 2)
                    {
                        // The first string at depth 2 is usually the translated text
                        var value = reader.GetString();
                        if (!string.IsNullOrEmpty(value) && value != originalText)
                        {
                            result.Append(value);
                        }
                    }
                }

                var manualResult = result.ToString();
                if (!string.IsNullOrEmpty(manualResult))
                {
                    return manualResult;
                }
            }
            catch
            {
                // If manual parsing also fails
            }

            return originalText;
        }

        // دالة مساعدة للتعامل مع النصوص الخاصة
        public static string EscapeSpecialChars(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            // Handle special characters that might break translation
            return text
                .Replace("\\", "\\\\")  // Escape backslashes
                .Replace("`", "\\`")    // Escape backticks
                .Replace("\"", "\\\"")  // Escape quotes
                .Replace("\n", "\\n")   // Preserve newlines as escaped
                .Replace("\t", "\\t")   // Preserve tabs as escaped
                .Replace("\r", "\\r");  // Preserve carriage returns as escaped
        }

        // دالة لإعادة النص إلى حالته الأصلية بعد الترجمة
        public static string UnescapeSpecialChars(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            return text
                .Replace("\\\\", "\\")  // Unescape backslashes
                .Replace("\\`", "`")    // Unescape backticks
                .Replace("\\\"", "\"")  // Unescape quotes
                .Replace("\\n", "\n")   // Restore newlines
                .Replace("\\t", "\t")   // Restore tabs
                .Replace("\\r", "\r");  // Restore carriage returns
        }

        // نسخة محسنة من الترجمة مع معالجة الأحرف الخاصة
        public static async Task<string> TranslateWithEscapeAsync(string text, string from, string to)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;
            if (from == to) return text;

            // Escape special characters before translation
            var escapedText = EscapeSpecialChars(text);
            var translated = await TranslateAsync(escapedText, from, to);

            // Unescape special characters after translation
            return UnescapeSpecialChars(translated);
        }

        // ترجمة من الإنجليزية لأي لغة
        public static async Task<string?> TranslateFromEnglish(string text, string to)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;
            if (to == "en") return text;

            return await TranslateWithEscapeAsync(text, "en", to);
        }

        // ترجمة من العربية لأي لغة
        public static async Task<string?> TranslateFromArabic(string text, string to)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;
            if (to == "ar") return text;

            return await TranslateWithEscapeAsync(text, "ar", to);
        }
    }
}