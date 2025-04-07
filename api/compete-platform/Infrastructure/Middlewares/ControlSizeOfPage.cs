
using compete_platform.Dto;
using compete_platform.Dto.Common;

namespace compete_platform.Infrastructure;

public class ControlSizeOfPage : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            if (context.Request.Query.TryGetValue("pageSize", out var pageSizeValue))
            {
                if (int.TryParse(pageSizeValue, out int pageSize) && pageSize > 100)
                {
                    return;
                }
            }
            await next(context);

        }
        catch (Exception)
        {
            await next(context);
        }
    }
}
