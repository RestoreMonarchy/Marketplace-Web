using Marketplace.Server.Filters;
using Marketplace.Server.Services;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Marketplace.Server.WebSockets
{
    public class WebSocketsMiddleware
    {
        private readonly RequestDelegate _next;

        public WebSocketsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IServersService serversService)
        {
            if (context.Request.Path == "/ws")
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    if (!await ApiKeyAuthAttribute.ValidateApiKeyAsync(context))
                    {
                        context.Response.StatusCode = 401;
                        return;
                    }

                    var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    await serversService.ListenServerWebSocketAsync(context, webSocket);
                }
                else
                {
                    context.Response.StatusCode = 400;
                }         
            }
            else
            {
                await _next(context);
            }
        }
    }
}
