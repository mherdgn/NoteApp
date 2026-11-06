using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.RegularExpressions;

namespace NoteApp.Helpers
{
    public static class HtmlHelpers
    {
        // HTML içeriğini truncate ederken tagleri bozmadan keser
        public static IHtmlContent TruncateHtml(this IHtmlHelper htmlHelper, string input, int maxLength = 100)
        {
            if (string.IsNullOrEmpty(input)) return new HtmlString(string.Empty);

            // HTML taglerini temizlemeden sadece metin uzunluğunu hesaplamak için strip
            string plainText = Regex.Replace(input, "<.*?>", string.Empty);

            if (plainText.Length <= maxLength)
                return new HtmlString(input);

            // Kesme noktası
            int end = maxLength;

            // HTML taglerini bozmadan kısalt
            string truncated = plainText.Substring(0, end) + "...";

            return new HtmlString(truncated);
        }
    }
}
