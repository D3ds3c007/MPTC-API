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



namespace MPTC_API.Controllers.Attendance
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ClockOutController : ControllerBase
    {
        
        private readonly RecognitionService _recognitionService;

        public MptcContext _context = new MptcContext();

        public ClockOutController(RecognitionService recognitionService)
        {
            _recognitionService = recognitionService;
        }

        [HttpGet("clock-out")]
        public async Task<IActionResult> Index()
        {
            Console.WriteLine("Start clock out");
            var capture = new VideoCapture(0);
            if (!capture.IsOpened)
            {
                Console.WriteLine("Failed to open camera.");
                // return BadRequest("Camera not available");
            }
            capture.Set(CapProp.Fps, 60);
            capture.Set(CapProp.Buffersize, 3);

            // Start processing frames asynchronously
            var cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;

            await Task.Run(() => _recognitionService.ProcessFrames(capture, token, false), token);

            
            return Ok("Welcome to ClockOut Controller");
        }

     
     

     
        
          




        

       

    }
}
