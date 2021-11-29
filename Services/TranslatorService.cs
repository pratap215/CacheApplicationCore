using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


namespace TranslationApplicationCore.Services
{
    public class TranslatorService : ITranslatorService
    {
        public async Task<string> Translate(string regionSpecifier, string translatorApiKey, string elementtextcontent, string sourceText, string languageCode, bool asHtml)
        {
            try
            {
                var textType = asHtml ? "html" : "plain";
                var path = $"translate?api-version=3.0&to={languageCode}&textType={textType}";
                System.Object[] body = new System.Object[] { new { Text = sourceText } };
                var requestBody = JsonConvert.SerializeObject(body);

                string uri = "https://api.cognitive.microsofttranslator.com";

                if (!string.IsNullOrEmpty(regionSpecifier))
                {
                    uri = "https://api${regionSpecifier}.cognitive.microsofttranslator.com";
                }

                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage())
                {
                    uri = $"{uri}/{path}";
                    request.Method = HttpMethod.Post;
                    request.RequestUri = new Uri(uri);
                    request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                    request.Headers.Add("Ocp-Apim-Subscription-Key", translatorApiKey);

                    var response = await client.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(responseBody))
                    {
                        var translatorResponseModelList = JsonConvert.DeserializeObject<List<TranslatorResponseModel>>(responseBody);
                        //dynamic result = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(responseBody), Formatting.Indented);
                        return translatorResponseModelList[0].translations[0].text;
                    }
                    return "novalue";
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }



        public async Task<List<BreakSentenceResult>> BreakSentence(string regionSpecifier, string translatorApiKey, string sourceText)
        {
            try
            {

                var path = $"breaksentence?api-version=3.0";
                System.Object[] body = new System.Object[] { new { Text = sourceText } };
                var requestBody = JsonConvert.SerializeObject(body);

                string uri = "https://api.cognitive.microsofttranslator.com";

                if (!string.IsNullOrEmpty(regionSpecifier))
                {
                    uri = "https://api${regionSpecifier}.cognitive.microsofttranslator.com";
                }

                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage())
                {
                    uri = $"{uri}/{path}";
                    request.Method = HttpMethod.Post;
                    request.RequestUri = new Uri(uri);
                    request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                    request.Headers.Add("Ocp-Apim-Subscription-Key", translatorApiKey);

                    var response = await client.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(responseBody))
                    {
                        var breakSentenceResultList = JsonConvert.DeserializeObject<List<BreakSentenceResult>>(responseBody);
                        return breakSentenceResultList;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public async Task<List<DetectLanguageResult>> DetectLanguage(string regionSpecifier, string translatorApiKey, string sourceText)
        {
            try
            {

                var path = $"detect?api-version=3.0";
                System.Object[] body = new System.Object[] { new { Text = sourceText } };
                var requestBody = JsonConvert.SerializeObject(body);

                string uri = "https://api.cognitive.microsofttranslator.com";

                if (!string.IsNullOrEmpty(regionSpecifier))
                {
                    uri = "https://api${regionSpecifier}.cognitive.microsofttranslator.com";
                }

                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage())
                {
                    uri = $"{uri}/{path}";
                    request.Method = HttpMethod.Post;
                    request.RequestUri = new Uri(uri);
                    request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                    request.Headers.Add("Ocp-Apim-Subscription-Key", translatorApiKey);

                    var response = await client.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(responseBody))
                    {
                        var detectLanguageResults = JsonConvert.DeserializeObject<List<DetectLanguageResult>>(responseBody);
                        return detectLanguageResults;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public async Task<AvailableLanguage> GetAvailableLanguages(string regionSpecifier, string translatorApiKey)
        {
            try
            {

                var path = $"languages?api-version=3.0&scope=dictionary";

                string uri = "https://api.cognitive.microsofttranslator.com";

                if (!string.IsNullOrEmpty(regionSpecifier))
                {
                    uri = "https://api${regionSpecifier}.cognitive.microsofttranslator.com";
                }
                
                var client = new HttpClient();
                using (var request = new HttpRequestMessage())
                {
                    uri = $"{uri}/{path}";
                    request.Method = HttpMethod.Get;
                    request.RequestUri = new Uri(uri);
                    request.Headers.Add("Ocp-Apim-Subscription-Key", translatorApiKey);
                    request.Method = HttpMethod.Get;
                    request.RequestUri = new Uri(uri);
                    var response = await client.SendAsync(request).ConfigureAwait(false);
                    string result = await response.Content.ReadAsStringAsync();
                    var deserializedOutput = JsonConvert.DeserializeObject<AvailableLanguage>(result);
                    return deserializedOutput;
                }

            }
            catch (Exception ex)
            {

                throw;
            }
        }


    }
}
