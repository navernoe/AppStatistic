using System.Text.RegularExpressions;
using System.Linq;
using System;

namespace AppStatisticApi.Utils
{
    public static class UrlFormatter
    {
        private static readonly string urlPattern = "(?<url>https:\\/\\/play\\.google\\.com\\/store\\/apps\\/details\\?id=([^%&])*)";
        private static Regex urlRegex = new Regex(urlPattern);

        public static string validate(string url)
        {
           Match urlMatch = urlRegex.Match(url);

            var urlList = (
                from Group g in urlMatch.Groups
                where g.Name == "url"
                select g.Value
            );

            if (urlList.Count() > 0)
            {
                url = urlList.ElementAt(0);
            }
            else
            {
                throw new ArgumentException();
            }

            return url;
        }
    }
}
