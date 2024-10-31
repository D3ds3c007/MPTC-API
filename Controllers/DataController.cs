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



namespace MPTC_API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DataController : ControllerBase
    {
        private readonly UserManager<Member> _userManager;
        private readonly IEmailService _emailService;
        private readonly RecognitionService _recognitionService;





        public MptcContext _context = new MptcContext();

        public DataController(UserManager<Member> userManager, IEmailService emailService, RecognitionService recognitionService)
        {
            _userManager = userManager;
            _emailService = emailService;
            _recognitionService = recognitionService;
        }

        [HttpGet("form-data")]
        public async Task<IActionResult> GetMultiStepFormData()
        {
            EmployeeFormDataDTO employeeFormDataDTO = DataService.GetEmployeeFormDataDTO(_context);
            return Ok(employeeFormDataDTO);
        }

        [HttpGet("exam-data")]
        public async Task<IActionResult> SendDataToExamForm()
        {
            List<LevelDTO> levelDTOs = DataService.GetLevels(_context);
            List<SubjectDTO> subjectDTOs = DataService.GetSubjects(_context);
            List<PeriodDTO> periodDTOs = DataService.GetPeriods(_context);

            List<Object> data = new List<Object>{levelDTOs, subjectDTOs, periodDTOs};

            return Ok(data);
        }
    }
}
