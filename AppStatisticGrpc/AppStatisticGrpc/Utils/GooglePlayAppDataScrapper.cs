using System;
using System.Net;
using System.Linq;
using System.Text.RegularExpressions;
using AppStatisticGrpc.Exceptions;

namespace AppStatisticGrpc.Utils
{

    public class GooglePlayAppDataScrapper
    {
        private string html { get; set; }

        public GooglePlayAppDataScrapper(string appUrl)
        {
            html = getGooglePlayAppDataHtml(appUrl);
        }

        private string getGooglePlayAppDataHtml(string appUrl)
        {
            string appDataHtml = "";

            using (WebClient client = new WebClient())
            {
                try
                {
                    appDataHtml = client.DownloadString(appUrl);
                }
                catch (Exception e)
                {
                    throw new InvalidAppUrlException("Неверная ссылка на приложение", e);
                }
            }

            return appDataHtml;
        }

        public string getDownloadsStatistic()
        {
            string downloadsPattern = "Downloaded\\s(?<downloads>(\\d)*)\\s";
            Regex downloadsRegex = new Regex(downloadsPattern);
            Match downloadsMatch = downloadsRegex.Match(html);

            string downloadsValue = (
                from Group g in downloadsMatch.Groups
                where g.Name == "downloads"
                select g.Value
            ).ElementAt(0);

            return downloadsValue;
        }

        public string getAppName()
        {
            string namePattern = "(itemprop=\\\"name\\\">(\\s)*<span(\\s)*>)(?<name>([^<])*)";
            Regex nameRegex = new Regex(namePattern);
            Match nameMatch = nameRegex.Match(html);

            string nameValue = (
                from Group g in nameMatch.Groups
                where g.Name == "name"
                select g.Value
            ).ElementAt(0);

            return nameValue;
        }
    }
}