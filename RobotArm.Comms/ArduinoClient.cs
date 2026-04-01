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
        var msg = $"6:{s.S6},5:{s.S5},4:{s.S4},3:{s.S3},2:{s.S2},1:{s.S1}";
        await Send(msg);
    }

    private async Task Send(string msg)
    {
        var bytes = Encoding.UTF8.GetBytes(msg);
        await ws.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
    }
}