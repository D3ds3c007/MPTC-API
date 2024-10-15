using System.Net.WebSockets;

namespace MPTC_API.Services
{
    public class GlobalService
    {
        public static WebSocket ws { get; set; }
        public static bool isStarted = false;


    }
}