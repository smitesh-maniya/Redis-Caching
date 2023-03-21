using CachingWithRedisDemo.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.VisualBasic;
using Newtonsoft.Json;

namespace CachingWithRedisDemo.Middleware
{
    public class RedisCachingMiddleware
    {
        public RequestDelegate Next { get; }
        public IDistributedCache DistributedCache { get; }
        public ILogger<RedisCachingMiddleware> Logger { get; }

        public RedisCachingMiddleware(RequestDelegate next, IDistributedCache distributedCache, ILogger<RedisCachingMiddleware> logger) 
        {
            Next = next;
            DistributedCache = distributedCache;
            Logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var requestPath = context.Request.Path;
            var queryStr = context.Request.QueryString;
            var cacheKey = requestPath+ queryStr;
            var cacheData = await DistributedCache.GetStringAsync(cacheKey.ToString());

            if (cacheData != null)
            {
                Logger.LogInformation("You get data from cache.");
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(cacheData);
            }
            else
            {
                context.Items["cacheKey"] = cacheKey;
                Logger.LogInformation("First Time Called");

                var responseStream = context.Response.Body;
                using var buffer = new MemoryStream();
                context.Response.Body = buffer;

                await Next(context);

                buffer.Seek(0, SeekOrigin.Begin);
                var responseContent = new StreamReader(buffer).ReadToEnd();
                Logger.LogInformation(responseContent);
                await buffer.CopyToAsync(responseStream);
                context.Response.Body = responseStream;

                var distributedCacheEntryOption = new DistributedCacheEntryOptions();
                distributedCacheEntryOption.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(180);
                distributedCacheEntryOption.SlidingExpiration = null; // if any appliction not using then remove

                await DistributedCache.SetStringAsync(cacheKey, responseContent,distributedCacheEntryOption);
            }
        }

    }
}
