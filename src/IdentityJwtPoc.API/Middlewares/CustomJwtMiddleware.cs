using IdentityJwtPoc.Application.Services.Interfaces;
using IdentityJwtPoc.Domain;
using IdentityJwtPoc.Infra.Data.CrossCutting.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;

namespace IdentityJwtPoc.API.Middlewares
{
    public static class CustomJwtMiddleware
    {
        public static void UseCustomJwtMiddleware(this IApplicationBuilder app)
        {
            app.Use(async (HttpContext context, Func<Task> next) =>
            {
                await next();
                await Check(context);
            });
        }

        public static async Task Check(
            HttpContext context)
        {
            PathString pathLogin = new("/api/Auth/login");
            PathString pathRefreshToken = new("/api/Auth/refresh-token");

            if (context.Request.Path.Equals(pathLogin, StringComparison.OrdinalIgnoreCase) ||
                context.Request.Path.Equals(pathRefreshToken, StringComparison.OrdinalIgnoreCase)) return;

            var user = context.User;
            if (!user.Identity!.IsAuthenticated) return;

            var userId = user.Claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userId == null)
                return;

            var serviceProvider = context.RequestServices;
            var cookieService = serviceProvider.GetRequiredService<ICookieService>();
            var identityService = serviceProvider.GetRequiredService<IIdentityService>();
            var fingerPrintClaim = user.Claims.SingleOrDefault(c => c.Type == Claims.FB);
            var strFingerPrintClaim = fingerPrintClaim?.Value ?? "";
            var decryptedStrFingerPrintClaim = Cryptography.DecryptString(strFingerPrintClaim);

            if (!cookieService.IsJwtFingerPrintValid(decryptedStrFingerPrintClaim))
            {
                Invalidate(context, identityService);
                return;
            }

            var lastChangeClaim = user.Claims.SingleOrDefault(c => c.Type == Claims.LAST_CHANGE);
            if (lastChangeClaim == null)
            {
                Invalidate(context, identityService);
                return;
            }

            var userService = serviceProvider.GetRequiredService<IUserService>();
            var userDb = await userService.GetById(new Guid(userId.Value));

            if (userDb == null) return;

            if (userDb.HasChange(lastChangeClaim.Value))
            {
                Invalidate(context, identityService);
                return;
            }

            return;
        }

        public static async Task CustomOnForbidden(ForbiddenContext context)
        {
            await Check(context.HttpContext);
        }

        private static void Invalidate(HttpContext context, IIdentityService identityService)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            identityService.Logout();
        }

        //public static async Task CustomOnChallenge(JwtBearerChallengeContext context)
        //{
        //    if (!await Check2(context.HttpContext, context.HttpContext.RequestServices, context.HttpContext.User))
        //    {
        //        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        //    }
        //}

        //public static async Task CustomOnMessageReceived(MessageReceivedContext context)
        //{
        //    if (!await Check2(context.HttpContext, context.HttpContext.RequestServices, context.HttpContext.User))
        //    {
        //        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        //    }
        //}

        //public static async Task CustomOnTokenValidated(TokenValidatedContext context)
        //{
        //    if (!await Check2(context.HttpContext, context.HttpContext.RequestServices, context.HttpContext.User))
        //    {
        //        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        //    }
        //}

        //public static async Task<bool> Check2(
        //    HttpContext context,
        //    IServiceProvider serviceProvider,
        //    ClaimsPrincipal user)
        //{
        //    var userId = user.Claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        //    if (userId != null)
        //    {
        //        var cookieService = serviceProvider.GetRequiredService<ICookieService>();
        //        var fingerPrintClaim = user.Claims.SingleOrDefault(c => c.Type == "fp");
        //        var strFingerPrintClaim = fingerPrintClaim?.Value ?? "";

        //        if (!cookieService.IsJwtFingerPrintValid(strFingerPrintClaim))
        //        {
        //            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        //            return false;
        //        }

        //        var lastChangeClaim = user.Claims.SingleOrDefault(c => c.Type == "LastChange");
        //        if (lastChangeClaim == null)
        //        {
        //            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        //            return false;
        //        }

        //        var userService = serviceProvider.GetRequiredService<IUserService>();
        //        var userDb = await userService.GetById(new Guid(userId.Value));

        //        if (userDb == null) return false;

        //        if (userDb.HasChange(lastChangeClaim.Value))
        //        {
        //            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        //            return false;
        //        }
        //    }

        //    return true;
        //}
    }
}
