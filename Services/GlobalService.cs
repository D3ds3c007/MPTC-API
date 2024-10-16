using System.Net.WebSockets;

namespace MPTC_API.Services
{
    public class GlobalService
    {
        public static WebSocket wsIn { get; set; }
        public static WebSocket wsOut { get; set; }

        public static bool isStarted = false;


    }
}