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

            string videoFilePath = "D:\\Dev\\Record Video RF\\a.mp4";
            string networkStreamUrl = "rtsp://localhost:8554/stream"; // Replace with your RTSP stream URL

            var capture = new VideoCapture(videoFilePath);
            if (!capture.IsOpened)
            {
                Console.WriteLine("Failed to open camera.");
                // return BadRequest("Camera not available");
            }else{
                Console.WriteLine("Camera 1 opened");
            }
            capture.Set(CapProp.Fps, 60);
            capture.Set(CapProp.Buffersize, 10);

            // Start processing frames asynchronously
            var cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;

            try{
                Console.WriteLine("Before process");
                await Task.Run(() => _recognitionService.ProcessFrames(capture, token, true, _context), token);
                Console.WriteLine("After  process");
            }catch(Exception e){
                Console.WriteLine(e.Message);
            }
            // Console.WriteLine("Before process");
            // await Task.Run(() => _recognitionService.ProcessFrames(capture, token, true), token);
            // Console.WriteLine("After  process");
            return Ok("Welcome to ClockIn Controller");
        }

     
     

     
        
          




        

       

    }
}
