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
using MPTC_API.Services.Education;



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
            // List<ExamDTO> examDTOs = ExamService.listExams(_context);
            List<ExamDTO> examDTOs = ExamService.listExamsPerProf(_context, 1);

            return Ok(examDTOs);
        }

        [HttpGet("list/{staffId}")]
        public async Task<IActionResult> GetExams(int staffId)
        {
            List<ExamDTO> examDTOs = ExamService.listExamsPerProf(_context, staffId);

            return Ok(examDTOs);
        }

        [HttpGet("get-exam/{examId}")]
        public async Task<IActionResult> GetExam(int examId)
        {
            ExamUpdateDTO exam = ExamService.getExamInfo(examId, _context);
            return Ok(exam);
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

                Exam newExam = new Exam
                {
                    PeriodId = (int)examformDTO.PeriodId,
                    Session = (int)examformDTO.Session,
                    SubjectId = (int)examformDTO.SubjectId,
                    LevelId = (int)examformDTO.LevelId,
                    Uripath = subjectPath,
                    UripathAssetNote = assetnotePath,
                    DateCreated = dateExam,
                    StaffId = int.Parse(idStaffClaim),
                    Name = examformDTO.Name,
                    DateLastModified = DateTime.UtcNow
                };

                int lastExamId = _context.Exams
                .OrderByDescending(e => e.IdExam)
                .Select(e => e.IdExam)
                .FirstOrDefault();

                if(lastExamId == 0)
                {
                    newExam.IdExam = 1;
                }
                else
                {
                    newExam.IdExam = lastExamId + 1;
                }

                //print exam attributes
                Console.WriteLine("Exam Details:");
                Console.WriteLine($"IdExam: {newExam.IdExam}");
                // Console.WriteLine($"PeriodId: {newExam.PeriodId}");
                // Console.WriteLine($"Session: {newExam.Session}");
                // Console.WriteLine($"SubjectId: {newExam.SubjectId}");
                // Console.WriteLine($"LevelId: {newExam.LevelId}");
                // Console.WriteLine($"Uripath: {newExam.Uripath}");
                // Console.WriteLine($"UripathAssetNote: {newExam.UripathAssetNote}");
                // Console.WriteLine($"DateCreated: {newExam.DateCreated}");
                // Console.WriteLine($"StaffId: {newExam.StaffId}");
                // Console.WriteLine($"Name: {newExam.Name}");
                // Console.WriteLine($"DateLastModified: {newExam.DateLastModified}");

                ExamService.validateExam(_context, newExam);
                Exam e = await ExamService.createExam(newExam, _context);

                ExtractorService.createCSV(e.Uripath, e.UripathAssetNote, newExam.Name);


            }
            else
            {
                Console.WriteLine("No file upload");
                return BadRequest("No file uploaded.");
            }

            return Ok();
        }

        [HttpPut("update-exam")]
        public async Task<IActionResult> UpdateExam([FromForm] ExamModifyDTO examModifyDTO){
            Console.WriteLine("Welcome to the ExamController");

            DateTime dateExam = DateTime.Parse(examModifyDTO.DateExam).ToUniversalTime();

            Exam e = new Exam
            {
                IdExam = (int)examModifyDTO.IdExam,
                PeriodId = (int)examModifyDTO.PeriodId,
                Session = (int)examModifyDTO.Session,
                SubjectId = (int)examModifyDTO.SubjectId,
                LevelId = (int)examModifyDTO.LevelId,
                DateCreated = dateExam,
            };

            Console.WriteLine("Exam Details:");
            Console.WriteLine($"PeriodId: {e.PeriodId}");
            Console.WriteLine($"Session: {e.Session}");
            Console.WriteLine($"SubjectId: {e.SubjectId}");
            Console.WriteLine($"LevelId: {e.LevelId}");
            Console.WriteLine($"DateCreated: {e.DateCreated}");

            ExamService.updateExam(e, _context);

            return Ok();
        }

        [HttpDelete("delete-exam/{examId}")]
        public async Task<IActionResult> DeleteExam(int examId){
            Console.WriteLine("Welcome to the ExamController");

            ExamService.deleteExam(examId, _context);

            return Ok();
        }

    }
}
