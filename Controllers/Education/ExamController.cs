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
using MPTC_API.Models.DTO;
using MPTC_API.Models.Education;



namespace MPTC_API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ExamController : ControllerBase
    {
        private readonly UserManager<Member> _userManager;
        private readonly IEmailService _emailService;
        private readonly RecognitionService _recognitionService;





        public MptcContext _context = new MptcContext();

        public ExamController(UserManager<Member> userManager, IEmailService emailService, RecognitionService recognitionService)
        {
            _userManager = userManager;
            _emailService = emailService;
            _recognitionService = recognitionService;
        }

        [HttpGet("data")]
        public async Task<IActionResult> SendDataToExamForm()
        {
            List<LevelDTO> levelDTOs = DataService.GetLevels(_context);
            List<SubjectDTO> subjectDTOs = DataService.GetSubjects(_context);
            List<PeriodDTO> periodDTOs = DataService.GetPeriods(_context);

            List<Object> data = new List<Object>{levelDTOs, subjectDTOs, periodDTOs};

            return Ok(data);
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetExams()
        {
            List<Exam> exams = _context.Exams.ToList();
            List<ExamDTO> examDTOs = ExamService.toExamDTO(exams);

            return Ok(examDTOs);
        }

        [HttpPost("create-exam")]
        public async Task<IActionResult> UploadExam([FromForm] ExamFormDTO examDTO)
        {
            Console.WriteLine("Welcome to the ExamController");

            if (examDTO.Subject != null && examDTO.Assetnote != null)
            {
                Console.WriteLine($"Received Asset Note File: {examDTO.Assetnote.FileName}, Size: {examDTO.Assetnote.Length}");
                Console.WriteLine($"Received Asset Note File: {examDTO.Assetnote.FileName}, Size: {examDTO.Assetnote.Length}");

                

            }else{
                return BadRequest("No file uploaded.");
            }



            Exam exam = new Exam();

            // Console.WriteLine("ExamDTO: " + JsonSerializer.Serialize(examDTO));
            // // Validate the uploaded file
            // if (file == null || file.Length == 0)
            // {
            //     return BadRequest("No file uploaded.");
            // }

            // if (!file.FileName.EndsWith(".pdf"))
            // {
            //     return BadRequest("Only PDF files are allowed.");
            // }

            // // Save the file as before
            // var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), TempDirectory);
            // if (!Directory.Exists(directoryPath))
            // {
            //     Directory.CreateDirectory(directoryPath);
            // }

            // var filePath = Path.Combine(directoryPath, Path.GetFileName(file.FileName));

            // using (var stream = new FileStream(filePath, FileMode.Create))
            // {
            //     await file.CopyToAsync(stream);
            // }

            // Process otherData as needed
            // Return the file path along with any other information
            // return Ok(new { filePath, additionalData = otherData });

            return Ok();
        }
    }
}
