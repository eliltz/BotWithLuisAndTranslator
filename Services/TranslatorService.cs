using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace LuisBot.Services
{
    public static class TranslatorService
    {
        static string ApiKey = "e2e6074acad9425999d89ba796c85275";
        //static string targetLang = "en";

        public static async Task<string> TranslateText(string textToTranslate, string targetLanguage)
        {
            string output = string.Empty;
            try
            {
                var accessToken = await GetAuthenticationToken(ApiKey);
                output = await TranslateTextFromApi(textToTranslate, targetLanguage, accessToken);
                return output;
            }
            catch (Exception ex)
            {

                output = ex.Message;
            }
           
            return output;
        }

        static async Task<string> TranslateTextFromApi(string inputText, string language, string accessToken)
        {
            string url = "http://api.microsofttranslator.com/v2/Http.svc/Translate";
            string query = $"?text={System.Net.WebUtility.UrlEncode(inputText)}&to={language}&contentType=text/plain";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var response = await client.GetAsync(url + query);
                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return "Hata: " + result;

                var translatedText = XElement.Parse(result).Value;
                return translatedText;
            }
        }

        static async Task<string> GetAuthenticationToken(string key)
        {
            string endpoint = "https://api.cognitive.microsoft.com/sts/v1.0/issueToken";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);
                var response = await client.PostAsync(endpoint, null);
                var token = await response.Content.ReadAsStringAsync();
                return token;
            }
        }

    }
}