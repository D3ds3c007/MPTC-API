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
using System.Security.Claims;


namespace MPTC_API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ExamController : ControllerBase
    {
        private readonly UserManager<Member> _userManager;
        private readonly IEmailService _emailService;
        private readonly RecognitionService _recognitionService;





        public MptcContext _context;

        public ExamController(UserManager<Member> userManager, IEmailService emailService, RecognitionService recognitionService, MptcContext context)
        {
            _userManager = userManager;
            _emailService = emailService;
            _recognitionService = recognitionService;
            _context = context;
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
        public async Task<IActionResult> UploadExam([FromForm] ExamFormDTO examformDTO)
        {
            Console.WriteLine("Welcome to the ExamController");

            // Get request header authorization
            string token = Request.Headers["Authorization"];
            Console.WriteLine($"token: {token}");

            // Check if token starts with "Bearer "
            if (token.StartsWith("Bearer "))
            {
                token = token.Substring("Bearer ".Length).Trim();
            }

            ClaimsPrincipal principal = AccountService.GetClaimsPrincipalFromToken(token);

            var idStaffClaim = principal.FindFirst("idStaff")?.Value;
            Console.WriteLine($"idStaffClaim : {idStaffClaim}");

            // read the file from the request
            // var file = Request.Form.Files[0];

            // Get request header authorization
            // string Cookie = Request.Headers["Cookie"];
            // Console.WriteLine($"Cookie: {Cookie}");

            if (examformDTO.Subject != null && examformDTO.Assetnote != null)
            {
                Console.WriteLine($"Received Asset Note File: {examformDTO.Assetnote.FileName}, Size: {examformDTO.Assetnote.Length}");
                Console.WriteLine($"Received Subject File: {examformDTO.Subject.FileName}, Size: {examformDTO.Subject.Length}");

                string assetnotePath = await ExamService.UploadPDFAsync(examformDTO.Assetnote);
                string subjectPath = await ExamService.UploadPDFAsync(examformDTO.Subject);

                DateTime dateExam = DateTime.Parse(examformDTO.DateExam).ToUniversalTime();

                Exam e = new Exam
                {
                    PeriodId = (int)examformDTO.PeriodId,
                    Session = (int)examformDTO.Session,
                    SubjectId = (int)examformDTO.SubjectId,
                    LevelId = (int)examformDTO.LevelId,
                    Uripath = subjectPath,
                    UripathAssetNote = assetnotePath,
                    DateCreated = dateExam,
                    StaffId = int.Parse(idStaffClaim)
                };

                //print exam attributes
                Console.WriteLine("Exam Details:");
                Console.WriteLine($"PeriodId: {e.PeriodId}");
                Console.WriteLine($"Session: {e.Session}");
                Console.WriteLine($"SubjectId: {e.SubjectId}");
                Console.WriteLine($"LevelId: {e.LevelId}");
                Console.WriteLine($"Uripath: {e.Uripath}");
                Console.WriteLine($"UripathAssetNote: {e.UripathAssetNote}");
                Console.WriteLine($"DateCreated: {e.DateCreated}");
                Console.WriteLine($"StaffId: {e.StaffId}");

                ExamService.createExam(e, _context);
            }
            else
            {
                Console.WriteLine("No file upload");
                return BadRequest("No file uploaded.");
            }

            return Ok();
        }

    }
}
