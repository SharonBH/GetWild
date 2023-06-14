using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Utilities
{
    public static class URLShortener
    {
        //private const string APIKey = "AIzaSyCN61XxMpDNPehKgNZlYkgkt4IdIgWLUbA";
        //AIzaSyCBKKcqmp1rxbPwil0lA8hsZH_Fw1dDvdQ
        //static string URL = @"https://www.googleapis.com/urlshortener/v1/url";

        public static string Shorten(int classid, int StudioId)
        {
            var url = GetURLforClassId(classid);
            string post = "{\"longUrl\": \"" + url + "\"}";
            string shortUrl = url;
            string GoogleAPIKey = App.Companies.FirstOrDefault(c => c.Studios.Any(s => s.Id == StudioId)).GoogleAPIKey;
            HttpWebRequest request =
                (HttpWebRequest) WebRequest.Create("https://www.googleapis.com/urlshortener/v1/url?key=" + GoogleAPIKey);

            try
            {
                request.ServicePoint.Expect100Continue = false;
                request.Method = "POST";
                request.ContentLength = post.Length;
                request.ContentType = "application/json";
                request.Headers.Add("Cache-Control", "no-cache");

                using (Stream requestStream = request.GetRequestStream())
                {
                    byte[] postBuffer = Encoding.ASCII.GetBytes(post);
                    requestStream.Write(postBuffer, 0, postBuffer.Length);
                }

                using (HttpWebResponse response = (HttpWebResponse) request.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader responseReader = new StreamReader(responseStream))
                        {
                            string json = responseReader.ReadToEnd();
                            shortUrl = Regex.Match(json, @"""id"": ?""(?<id>.+)""").Groups["id"].Value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // if Google's URL Shortner is down...
                //System.Diagnostics.Debug.WriteLine(ex.Message);
                //System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                Logger.WriteError(ex.Message);
                Logger.WriteError(ex.StackTrace);
            }
            return shortUrl;
        }


        public static string ShortenFB(int classid, int StudioId)
        {
            var longurl = GetURLforClassId(classid);
            string shortUrl = longurl;
            string GoogleAPIKey = App.Companies.FirstOrDefault(c => c.Studios.Any(s => s.Id == StudioId)).GoogleAPIKey;
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://firebasedynamiclinks.googleapis.com/v1/shortLinks?key=" + GoogleAPIKey);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))

            {
                //string json = "{\"longDynamicLink\":\"" + longurl + "\"}";
                string json = "{\"dynamicLinkInfo\":{\"domainUriPrefix\":\"https://pulsestudio.page.link\",\"link\": \"" + longurl + "\"}}";
                streamWriter.Write(json);
            }


            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))

            {
                var responseText = streamReader.ReadToEnd();
                dynamic data = JObject.Parse(responseText);
                shortUrl = data.shortLink;
            }
            return shortUrl;
        }

        private static string GetURLforClassId(int classid)
        {
            var request = HttpContext.Current.Request;
            var baseURL = $"{request.Url.Scheme}://{request.Url.Authority}{request.ApplicationPath.TrimEnd('/')}";
            return $"{baseURL}/Gym/GetDailyCalander?classid={classid}";
        }
    }
}
