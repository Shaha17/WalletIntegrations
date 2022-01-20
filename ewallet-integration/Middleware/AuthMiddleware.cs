using System.Linq;
using System.Threading.Tasks;
using ewallet_integration.Context;
using ewallet_integration.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ewallet_integration.Middleware
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        public async Task InvokeAsync(HttpContext context, WalletDbContext dbContext)
        {
            var msisdn = context.Request.Headers["X-UserId"].ToString();
            if (dbContext.Wallets.Any(w => w.Msisdn.Equals(msisdn) || w.Msisdn.Equals($"992{msisdn}")))
            {
                await context.Response.WriteAsJsonAsync(new Error(){Message = "Unknown user"});
                return;
            }

            await _next.Invoke(context);
        }
    }
}