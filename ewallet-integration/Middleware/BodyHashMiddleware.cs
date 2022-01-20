using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ewallet_integration.Context;
using ewallet_integration.Models;
using Microsoft.AspNetCore.Http;

namespace ewallet_integration.Middleware
{
    public class BodyHashMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _secretKey;

        public BodyHashMiddleware(RequestDelegate next, string secretKey)
        {
            this._next = next;
            _secretKey = secretKey;
        }

        public async Task InvokeAsync(HttpContext context, WalletDbContext dbContext)
        {
            var digest = context.Request.Headers["X-Digest"];
            if (string.IsNullOrWhiteSpace(digest.ToString()))
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsJsonAsync(new Error() {Message = "Error with headers"});
                return;
            }

            context.Request.EnableBuffering();
            var bodyAsText = await new System.IO.StreamReader(context.Request.Body).ReadToEndAsync();
            context.Request.Body.Position = 0;

            var hash = CalcHash(bodyAsText);
            if (hash.Equals(digest.ToString()))
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsJsonAsync(new Error() {Message = "Invalid hash"});
                return;
            }

            await _next.Invoke(context);
        }

        private string CalcHash(string bodyAsText)
        {
            var enc = Encoding.Default;
            HMACSHA1 myhmacsha1 = new HMACSHA1(enc.GetBytes(_secretKey));
            byte[] byteArray = enc.GetBytes(bodyAsText);
            MemoryStream stream = new MemoryStream(byteArray);
            var hash = myhmacsha1.ComputeHash(stream).Aggregate("", (s, e) => s + $"{e:x2}", s => s);
            return hash;
        }
    }
}