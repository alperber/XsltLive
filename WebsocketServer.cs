using System.Net;
using System.Net.WebSockets;

namespace XsltLive;

public class WebsocketServer
{
    private SynchronizedCollection<WebSocket> _clients = new();
    
    public async Task Start()
    {
        HttpListener listener = new HttpListener();
        listener.Prefixes.Add($"http://localhost:{AppConfig.WebsocketPort}/");
        listener.Start();

        while (true)
        {
            HttpListenerContext listenerContext = await listener.GetContextAsync();
            if (listenerContext.Request.IsWebSocketRequest)
            {
#pragma warning disable CS4014
                Task.Run(() =>
                {
                    ProcessWebsocketRequest(listenerContext);
                });
#pragma warning restore CS4014
            }
            else
            {
                listenerContext.Response.StatusCode = 400;
                listenerContext.Response.Close();
            }
        }
    }

    async Task ProcessWebsocketRequest(HttpListenerContext listenerContext)
    {
        WebSocketContext webSocketContext = null;
        try
        {
            webSocketContext = await listenerContext.AcceptWebSocketAsync(subProtocol: null);
        }
        catch (Exception e)
        {
            listenerContext.Response.StatusCode = 500;
            listenerContext.Response.Close();
            Logger.LogError("Error when accepting websocket request", e);
            return;
        }

        WebSocket webSocket = webSocketContext.WebSocket;
        _clients.Add(webSocket);
        try
        {
            byte[] receiveBuffer = new byte[1024];

            while (webSocket.State == WebSocketState.Open)
            {
                WebSocketReceiveResult receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);

                if (receiveResult.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                }
            }
            _clients.Remove(webSocket);
        }
        catch (Exception e)
        {
            Logger.LogError("Exception when listening websocket, maybe remote closed the websocket connection without making close handshake, removing that websocket from the client list", e);
            _clients.Remove(webSocket);
        }
        finally
        {
            webSocket.Dispose();
        }
    }
    
    public void NotifyClients()
    {
        // make copy with .ToList because collection can be modified while iterating(when ws close)
        foreach (var client in _clients.ToList())
        {
            try
            {
                client.SendAsync(new ArraySegment<byte>(AppConfig.ReloadCommandInBytes), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch (Exception ex)
            {
                Logger.LogError("Exception when notifying websocket ", ex);
            }
        }
    }
}