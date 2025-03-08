using System.IO.Pipes;
using System.Text;

namespace MoneyMind_API.Client
{
    // public class NamedPipeClient
    // {
    //     public async Task<string> SendMessageAsync(string message)
    //     {
    //         using (var pipeClient = new NamedPipeClientStream(".", "MyPipe", PipeDirection.InOut, PipeOptions.Asynchronous))
    //         {
    //             Console.WriteLine("Client: Đang kết nối đến server...");
    //             await pipeClient.ConnectAsync(5000); // Timeout 5 giây
    //             Console.WriteLine("Client: Kết nối thành công.");
    //
    //             byte[] messageBytes = Encoding.UTF8.GetBytes(message);
    //             await pipeClient.WriteAsync(messageBytes, 0, messageBytes.Length);
    //             pipeClient.WaitForPipeDrain();
    //
    //             byte[] buffer = new byte[256];
    //             int bytesRead = await pipeClient.ReadAsync(buffer, 0, buffer.Length);
    //             string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
    //             return response;
    //         }
    //     }
    // }
}
