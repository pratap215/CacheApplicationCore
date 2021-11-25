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
using CacheApplicationCore.Services;


namespace CacheApplicationCore
{
    [Route("api/[controller]")]
    [ApiController]
    public partial class RedisCacheController : ControllerBase
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IDistributedCache _distributedcache;
        private readonly ILogger<RedisCacheController> _logger;
        private readonly ICacheService _cacheService;
        public RedisCacheController(IMemoryCache memoryCache, IDistributedCache cache, ILogger<RedisCacheController> logger, ICacheService cacheService)
        {
            _memoryCache = memoryCache;
            _distributedcache = cache;
            _logger = logger;
            _cacheService = cacheService;
        }

        //[HttpGet("GetCacheData")]
        //public IActionResult GetCacheData(string cacheKey)
        //{
        //    //checks if cache entries exists
        //    _memoryCache.TryGetValue(cacheKey, out string cachevalue);
        //    cachevalue = !string.IsNullOrEmpty(cachevalue) ? cachevalue : "novalue";
        //    _logger.LogInformation($"     [Getting] data for cache key : [{cacheKey}] and value: [{cachevalue}]");
        //    _logger.LogInformation("");
        //    return new JsonResult(cachevalue);
        //}


        //[HttpPost("SetCacheData")]
        //public IActionResult SetCacheData([FromBody] CacheModel cacheModel)
        //{

        //    //setting up cache options
        //    var cacheExpiryOptions = new MemoryCacheEntryOptions
        //    {
        //        AbsoluteExpiration = DateTime.Now.AddDays(30),
        //        Priority = CacheItemPriority.High,
        //        SlidingExpiration = TimeSpan.FromDays(30)
        //    };
        //    //setting cache entries
        //    _memoryCache.Set(cacheModel.cacheKey, cacheModel.cachevalue, cacheExpiryOptions);
        //    _logger.LogInformation($"[Setting] data with cache key : [{cacheModel.cacheKey}] and value: [{cacheModel.cachevalue}]");

        //    return new JsonResult(cacheModel.cachevalue);
        //}



        //[HttpGet("ClearCache")]
        //public IActionResult ClearCache()
        //{
        //    _memoryCache.Dispose();
        //    return Ok("done");
        //}

        [HttpGet("GetCacheData")]
        public IActionResult GetCacheData(string cacheKey)
        {
            //checks if cache entries exists
            var res = GetDistributedCacheData(cacheKey).Result as JsonResult;
            var cachevalue = res.Value.ToString();
            return new JsonResult(cachevalue);
        }

        [HttpPost("SetCacheData")]
        public IActionResult SetCacheData([FromBody] CacheModel cacheModel)
        {
            //setting cache entries
            var result = SetDistributedCacheData(cacheModel).Result;
            return new JsonResult(cacheModel.cachevalue);
        }


        private async Task<IActionResult> GetDistributedCacheData(string cacheKey)
        {
            //port 6379
            // var cachevalue = await _distributedcache.GetRecordAsync<string>(cacheKey);

            var cachevalue = await _cacheService.GetCacheValueAsync(cacheKey);
            cachevalue = !string.IsNullOrEmpty(cachevalue) ? cachevalue : "novalue";
            _logger.LogInformation($"   Getting data for cache key : [ {cacheKey} ] and value [{cachevalue}]");
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