using Microsoft.AspNetCore.Mvc;
using MPTC_API.Data;
using MPTC_API.Services;
using Emgu.CV;
using Emgu.CV.CvEnum;



namespace MPTC_API.Controllers.Attendance
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ClockInController : ControllerBase
    {
        
        private readonly RecognitionService _recognitionService;

        public MptcContext _context = new MptcContext();

        public ClockInController(RecognitionService recognitionService)
        {
        
            _recognitionService = recognitionService;
        }

        [HttpGet("clock-in")]
        public async Task<IActionResult> Index()
        {
            Console.WriteLine("Start clock in");

            var capture = new VideoCapture(1);
            if (!capture.IsOpened)
            {
                Console.WriteLine("Failed to open camera.");
                return BadRequest("Camera not available");
            }
            capture.Set(CapProp.Fps, 60);
            capture.Set(CapProp.Buffersize, 3);

            // Start processing frames asynchronously
            var cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;

            await Task.Run(() => _recognitionService.ProcessFrames(capture, token, true), token);

            
            return Ok("Welcome to ClockIn Controller");
        }

     
     

     
        
          




        

       

    }
}