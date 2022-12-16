using IdentityJwtPoc.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;

namespace IdentityJwtPoc.API.Middlewares
{
    public static class CustomJwtMiddleware
    {
        public static string FormAddedEmailClaimKey(this string userId) => $"AddEmailClaim-{userId}";

        public static void UseCustomJwtMiddleware(this IApplicationBuilder app)
        {
            app.Use(async (HttpContext context, Func<Task> next) =>
            {
                await next();
                await Check(context, context.RequestServices, context.User);
                //if (replacementUser != null)
                //    context.User = replacementUser;

            });
        }

        public static async Task Check(
            HttpContext context,
            IServiceProvider serviceProvider,
            ClaimsPrincipal user)
        {
            PathString pathLogin = new("/api/Auth/login");
            PathString pathRefreshToken = new("/api/Auth/refresh-token");
            
            if (context.Request.Path.Equals(pathLogin, StringComparison.OrdinalIgnoreCase) ||
                context.Request.Path.Equals(pathRefreshToken, StringComparison.OrdinalIgnoreCase)) return;

            if (!user.Identity.IsAuthenticated) return;
            
            var userId = user.Claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userId != null)
            {
                //There is a logged-in user, so we see if the FileStore cache contains a new Permissions claim
                //var fsCache = serviceProvider.GetRequiredService<IDistributedFileStoreCacheClass>();
                var cookieService = serviceProvider.GetRequiredService<ICookieService>();
                var fingerPrintClaim = user.Claims.SingleOrDefault(c => c.Type == "fp");
                var strFingerPrintClaim = fingerPrintClaim?.Value ?? "";

                if (!cookieService.IsJwtFingerPrintValid(strFingerPrintClaim))
                {
                    //context.Response.Clear();
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }

                var lastChangeClaim = user.Claims.SingleOrDefault(c => c.Type == "LastChange");
                if (lastChangeClaim == null)
                {
                    //context.Response.Clear();
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }

                var userService = serviceProvider.GetRequiredService<IUserService>();
                var userDb = await userService.GetById(new Guid(userId.Value));

                if (userDb == null) return;

                if (userDb.HasChange(lastChangeClaim.Value))
                {
                    //context.Response.Clear();
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }

                //var usersEmail = await fsCache.GetAsync(userId.FormAddedEmailClaimKey());
                //if (usersEmail == null)
                //{
                //    //Not set up yet, so we need to get the user's email and place it in the cache
                //    var context = serviceProvider.GetRequiredService<AuthPermissionsDbContext>();
                //    usersEmail = context.AuthUsers.Where(x => x.UserId == userId).Select(x => x.Email).FirstOrDefault();

                //    if (usersEmail == null)
                //        return null; //shouldn't happen, but could in certain updates

                //    await fsCache.SetAsync(userId.FormAddedEmailClaimKey(), usersEmail);
                //}

                ////We need to add the Email from the cache
                //var updateClaims = user.Claims.ToList();
                //updateClaims.Add(new Claim(ClaimTypes.Email, usersEmail));

                //var appIdentity = new ClaimsIdentity(updateClaims, user.Identity!.AuthenticationType);
                ////return new ClaimsPrincipal(appIdentity);
            }

            return; //no change to the current user
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
