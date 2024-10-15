using Microsoft.AspNetCore.Mvc;
using MPTC_API.Data;
using MPTC_API.Models.Attendance;
using MPTC_API.Services.Authentication;
using Microsoft.AspNetCore.Identity;
using MPTC_API.Models.Attendance.MemberDTO;
using System.Text.Json;
using EllipticCurve.Utils;
using MPTC_API.Services.Attendance;
using MPTC_API.Services;
using static System.Text.Json.JsonElement;
using MPTC_API.Models.Attendance.StaffDTO;
using System.Text.RegularExpressions;
using Emgu.CV;
using Emgu.CV.CvEnum;
using System.Net.WebSockets;
using System.Text;



namespace MPTC_API.Controllers.Attendance
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class WebSocketController : ControllerBase
    {
        private readonly GlobalService _globalService;

        public WebSocketController(GlobalService globalService)
        {
            _globalService = globalService;
        }

        [HttpGet("/ws")]
        public async Task WebSocketHandler()
        {
            var context = ControllerContext.HttpContext;
            if (context.WebSockets.IsWebSocketRequest)
            {
                Console.WriteLine("Websocket request");

                var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                GlobalService.ws = webSocket;
                await Echo(webSocket);
            }
            else
            {
                context.Response.StatusCode = 400;
            }
        }

        private async Task Echo(WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);


            while (!result.CloseStatus.HasValue)
            {

                var receivedData = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine(receivedData);

                await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            // Handle the closure of the connection gracefully
                if (result.CloseStatus.HasValue)
                {
                    Console.WriteLine("Closing WebSocket connection...");
                    await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
                    Console.WriteLine("WebSocket connection closed.");
                }
        }

      
    }
}
