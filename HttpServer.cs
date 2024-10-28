using System.Net;
using System.Text;

namespace XsltLive;

public class HttpServer
{
    public async Task Start()
    {
        var httpListenUrl = $"http://localhost:{AppConfig.HttpPort}/";
        var listener = new HttpListener();
        listener.Prefixes.Add(httpListenUrl);
        listener.Start();
        Logger.LogInfo($"listening on {httpListenUrl}");
        while (true)
        {
            var ctx = await listener.GetContextAsync();
#pragma warning disable CS4014
            await HandleHttpRequest(ctx);
#pragma warning restore CS4014
        }
    }

    private async Task HandleHttpRequest(HttpListenerContext ctx)
    {
        try
        {
            ctx.Response.ContentType = "text/html";
            ctx.Response.ContentEncoding = Encoding.UTF8;

            if (ctx.Request.Url!.AbsolutePath != "/")
            {
                var notFound = AppConfig.NotFoundResponseInBytes;
                ctx.Response.StatusCode = 404;
                ctx.Response.ContentLength64 = notFound.LongLength;
                await ctx.Response.OutputStream.WriteAsync(notFound, 0, notFound.Length);
                ctx.Response.Close();
                return;
            }

            var responseBytes = Encoding.UTF8.GetBytes(GlobalState.RenderedHtml);
            ctx.Response.ContentLength64 = responseBytes.Length;
            ctx.Response.StatusCode = 200;

            await ctx.Response.OutputStream.WriteAsync(responseBytes, 0, responseBytes.Length);
            ctx.Response.Close();
        }
        catch (Exception e)
        {
            Logger.LogError("Error occured while processing http request", e);
        }
    }
}