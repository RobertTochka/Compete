using Microsoft.AspNetCore.Authentication.Cookies;

namespace compete_platform.Infrastructure
{
    public class SameSiteCookieManager : ICookieManager
    {

        public SameSiteCookieManager()
        {
        }
        public static bool DisallowsSameSiteNone(HttpContext context)
        {
            var userAgent = context.Request.Headers["User-Agent"];
            if (string.IsNullOrEmpty(userAgent))
            {
                return false;
            }
            return userAgent.Contains("BrokenUserAgent") ||
                   userAgent.Contains("BrokenUserAgent2");
        }
        private void CheckSameSite(HttpContext context, CookieOptions options)
        {
            if (options.SameSite == SameSiteMode.None && DisallowsSameSiteNone(context))
            {
                options.SameSite = SameSiteMode.Strict;
            }
        }


        public string? GetRequestCookie(HttpContext context, string key)
        {
            context.Request.Headers.TryGetValue("Cookie", out var cookies);
            return cookies.ToString();
        }
        private string CreateCookieHeader(string key, string value, CookieOptions options, bool delete = false)
        {
            var header = $"{key}={value};";
            if (options.Expires.HasValue) header += $" Expires={options.Expires.Value:R};";
            if (options.Secure) header += " Secure;";
            if (options.HttpOnly) header += " HttpOnly;";
            if (options.SameSite != SameSiteMode.Unspecified) header += $" SameSite={options.SameSite};";
            if (delete) header += " Max-Age=0;";
            return header;
        }
        public void AppendResponseCookie(HttpContext context, string key, string? value, CookieOptions options)
        {
            CheckSameSite(context, options);
            context.Response.Headers.Append("Set-Cookie", CreateCookieHeader(key, value ?? string.Empty, options));
        }

        public void DeleteCookie(HttpContext context, string key, CookieOptions options)
        {
            CheckSameSite(context, options);
            context.Response.Headers.Append("Set-Cookie", CreateCookieHeader(key, string.Empty, options, true));
        }
    }

}
