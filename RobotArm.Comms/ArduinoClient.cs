using System.Net.WebSockets;
using System.Text;
using RobotArm.Domain;

namespace RobotArm.Comms;

public class ArduinoClient
{
    private ClientWebSocket _ws = new  ClientWebSocket();
    private string? _url;
    private readonly SemaphoreSlim _sendLock = new(1, 1);

    public async Task Connect(string url)
    {
        _url = url;
        await _ws.ConnectAsync(new Uri(url), CancellationToken.None);
    }

    public async Task SendServos(ServoAngles s)
    {
        var msg = $"6:{s.S6},5:{s.S5},4:{s.S4},3:{s.S3},2:{s.S2},1:{s.S1}";
        await Send(msg);
    }

    private async Task Send(string msg)
    {
        await _sendLock.WaitAsync();

        try
        {
            if (_ws.State != WebSocketState.Open)
            {
                Console.WriteLine($"[ArduinoClient] WebSocket state: {_ws.State}, reconnecting...");
                _ws.Dispose();
                _ws = new ClientWebSocket();
                await _ws.ConnectAsync(new Uri(_url!), CancellationToken.None);
                Console.WriteLine("[ArduinoClient] Reconnected.");
            }

            var bytes = Encoding.UTF8.GetBytes(msg);
            await _ws.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
        }
        finally
        {
            _sendLock.Release();
        }
    }
}