using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Services.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebApp.Authentication
{
    public class UserSessionMiddleware
    {
        private readonly RequestDelegate _next;

        public UserSessionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IAccountService accountService)
        {
            var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userId) && !accountService.UserIdExists(userId))
            {
                await context.SignOutAsync();
            }

            await _next(context);
        }
    }

}
