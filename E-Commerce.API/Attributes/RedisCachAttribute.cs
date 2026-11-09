using E_Commerce.Application.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;

namespace E_Commerce.API.Attributes
{
    public class RedisCachAttribute : ActionFilterAttribute
    {
        private readonly int _durationInSeconds;

        public RedisCachAttribute(int durationInSeconds = 60)
        {
            _durationInSeconds = durationInSeconds;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            // get cache service from container 
            var cachService = context.HttpContext.RequestServices.GetRequiredService<ICacheService>();

            var cacheKey = CreateCachKey(context.HttpContext.Request);

            var data = await cachService.GetDataAsync(cacheKey);
            // if data exist in the cache => get data from the cache + skip endpoint
            if(!string.IsNullOrEmpty(data))
            {
                context.Result = new ContentResult()
                {
                    Content = data,
                    ContentType = "application/json",
                    StatusCode = StatusCodes.Status200OK
                };
                return;

            }

            // if data not exist          => execute the endpoint + store or chache the result if it is 200 ok with data 
            var executedContext = await next.Invoke();  
            if(executedContext.Result is OkObjectResult { Value : not null} ok)
            {
                await cachService.setDataAsync(cacheKey, ok.Value, TimeSpan.FromSeconds(_durationInSeconds));
            }
        }
        private static string CreateCachKey(HttpRequest request)
        {
            var key = new StringBuilder();
            key.Append(request.Path);
            if(request.Query.Any())
            {
                key.Append('?');
                foreach (var (k, v) in request.Query.OrderBy(x => x.Key))
                {
                    key.Append(k).Append("=").Append(v).Append('&');
                }
            }
            return key.ToString();
        }
    }
}
