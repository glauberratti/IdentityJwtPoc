using IdentityJwtPoc.Application.Services.Interfaces;
using IdentityJwtPoc.Domain;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace IdentityJwtPoc.Infra.Identity.Services
{
    public class CookieService : ICookieService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CookieService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public bool IsJwtFingerPrintValid(string claimFingerPrint)
        {
            var ok = _httpContextAccessor!.HttpContext!.Request.Cookies.TryGetValue(Claims.FB, out var fingerPrint);

            if (!ok) return false;

            return fingerPrint == claimFingerPrint;
        }

        public void CreateCookie(string key, string value, int expirationTime)
        {
            var cookieOp = new CookieOptions()
            {
                Expires = new DateTimeOffset(DateTime.Now.AddSeconds(expirationTime)),
                HttpOnly = true,
                IsEssential = true,
                SameSite = SameSiteMode.Strict,
                Secure = true,
            };

            _httpContextAccessor.HttpContext!.Response.Cookies.Append(key, value, cookieOp);
        }

        public string GetCookieValue(string key)
        {
            return _httpContextAccessor?.HttpContext?.Request.Cookies[key] ?? "";
        }

        public void DeleteCookie(string key)
        {
            _httpContextAccessor?.HttpContext?.Response.Cookies.Delete(key);
        }
    }
}
