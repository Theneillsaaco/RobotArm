using System.Net.WebSockets;
using System.Text;
using RobotArm.Domain;

namespace RobotArm.Comms;

public class ArduinoClient
{
    private ClientWebSocket ws = new  ClientWebSocket();

    public async Task Connect(string url)
    {
        await ws.ConnectAsync(new Uri(url), CancellationToken.None);
    }

    public async Task SendServos(ServoAngles s)
    {
        await Send($"6:{s.S6}");
        await Send($"5:{s.S5}");
        await Send($"4:{s.S4}");
    }

    private async Task Send(string msg)
    {
        var bytes = Encoding.UTF8.GetBytes(msg);
        await ws.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
    }
}