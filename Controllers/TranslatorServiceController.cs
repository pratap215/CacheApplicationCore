using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using TranslationApplicationCore.Services;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Newtonsoft.Json;


namespace TranslationApplicationCore
{
    [Route("api/[controller]")]
    [ApiController]
    public class TranslatorServiceController : ControllerBase
    {

        private readonly ILogger<TranslatorServiceController> _logger;
        private readonly ITranslatorService _translatorService;
        private IConfiguration _configuration;
        private readonly ICacheService _cacheService;
        public TranslatorServiceController(ILogger<TranslatorServiceController> logger, ITranslatorService translatorService, IConfiguration configuration, ICacheService cacheService)
        {

            _logger = logger;
            _translatorService = translatorService;
            _configuration = configuration;
            _cacheService = cacheService;
        }

        [HttpPost("GetAvailableLanguages")]
        public async Task<IActionResult> GetAvailableLanguages()
        {
            _logger.LogInformation("");
            var regionSpecifier = _configuration.GetSection("CognitiveAPI")["regionSpecifier"];
            var translatorApiKey = _configuration.GetSection("CognitiveAPI")["translatorApiKey"];
            var cacheKey = "availablelanguages";
            AvailableLanguage languageModels = null;
            var cachevalue = await _cacheService.GetCacheValueAsync(cacheKey);

            if (string.IsNullOrEmpty(cachevalue))
            {
                languageModels = await _translatorService.GetAvailableLanguages(regionSpecifier, translatorApiKey);

                string jsonString = JsonConvert.SerializeObject(languageModels);

                await _cacheService.SetCacheValueAsync(cacheKey, jsonString);
            }
            else
            {
                languageModels =  JsonConvert.DeserializeObject<AvailableLanguage>(cachevalue);
            }

            if (languageModels != null)
            {
                return new JsonResult(languageModels);
            }
            return null;

        }
        [HttpPost("BreakSentence")]
        public async Task<IActionResult> BreakSentence([FromBody] BreakSentenceModel breakSentenceModel)
        {
            _logger.LogInformation("");
            var regionSpecifier = _configuration.GetSection("CognitiveAPI")["regionSpecifier"];
            var translatorApiKey = _configuration.GetSection("CognitiveAPI")["translatorApiKey"];
            var breakSentenceResults = await _translatorService.BreakSentence(regionSpecifier, translatorApiKey, breakSentenceModel.sourceText);

            if (breakSentenceResults.Count > 0)
            {
                var breakSentenceResult = breakSentenceResults.FirstOrDefault();
                return new JsonResult(breakSentenceResult);
            }
            return null;
        }

        [HttpPost("DetectLanguage")]
        public async Task<IActionResult> DetectLanguage([FromBody] DetectLanguageModel detectLanguageModel)
        {
            _logger.LogInformation("");
            DetectLanguageResult detectLanguageResult = null;
            var regionSpecifier = _configuration.GetSection("CognitiveAPI")["regionSpecifier"];
            var translatorApiKey = _configuration.GetSection("CognitiveAPI")["translatorApiKey"];
            var detectLanguageResults = await _translatorService.DetectLanguage(regionSpecifier, translatorApiKey, detectLanguageModel.Text);
            if (detectLanguageResults.Count > 0)
            {
                detectLanguageResult = detectLanguageResults.FirstOrDefault(langInfo => langInfo.score >= 0.8 && langInfo.isTranslationSupported);
            }
            return new JsonResult(detectLanguageResult);
        }

        [HttpPost("TranslateData")]
        public async Task<IActionResult> TranslateData([FromBody] TranslatorServiceModel translatorServiceModel)
        {
            _logger.LogInformation($"TranslateData for cache key {translatorServiceModel.elementtextcontent}");

            var res = GetDistributedCacheData(translatorServiceModel.elementtextcontent).Result as JsonResult;
            var cachevalue = res != null && res.Value != null ? res.Value.ToString() : null;

            if (!string.IsNullOrEmpty(cachevalue) && cachevalue != "novalue")
            {
                return new JsonResult(cachevalue);
            }

            var regionSpecifier = _configuration.GetSection("CognitiveAPI")["regionSpecifier"];
            var translatorApiKey = _configuration.GetSection("CognitiveAPI")["translatorApiKey"];

            var translatedText = await _translatorService.Translate(regionSpecifier, translatorApiKey, translatorServiceModel.elementtextcontent, translatorServiceModel.sourceText, translatorServiceModel.languageCode,
                translatorServiceModel.asHtml);

            _logger.LogInformation($"   data from [TRANSLATOR API] for key : [ {translatorServiceModel.elementtextcontent} ] and value [{translatedText}]");
            var cacheModel = new CacheModel() { cacheKey = translatorServiceModel.elementtextcontent, cachevalue = translatedText };

            await SetDistributedCacheData(cacheModel);

            return new JsonResult(translatedText);
        }




        private async Task<IActionResult> GetDistributedCacheData(string cacheKey)
        {
            //port 6379
            // var cachevalue = await _distributedcache.GetRecordAsync<string>(cacheKey);

            var cachevalue = await _cacheService.GetCacheValueAsync(cacheKey);
            cachevalue = !string.IsNullOrEmpty(cachevalue) ? cachevalue : "novalue";
            _logger.LogInformation($"  data from [CACHE] - for key : [ {cacheKey} ] and value [{cachevalue}]");
            return new JsonResult(cachevalue);
        }



        private async Task<IActionResult> SetDistributedCacheData([FromBody] CacheModel cacheModel)
        {
            //var options = new DistributedCacheEntryOptions()
            //.SetAbsoluteExpiration(DateTime.Now.AddMinutes(100))
            //.SetSlidingExpiration(TimeSpan.FromMinutes(20));
            await _cacheService.SetCacheValueAsync(cacheModel.cacheKey, cacheModel.cachevalue);
            _logger.LogInformation($"Setting data for cache key : [ {cacheModel.cacheKey} ] and value [{cacheModel.cachevalue}]");
            return new JsonResult(cacheModel.cachevalue);
        }



    }
}