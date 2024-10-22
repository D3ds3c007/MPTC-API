using Microsoft.AspNetCore.Mvc;
using MPTC_API.Data;
using MPTC_API.Services;
using Emgu.CV;
using Emgu.CV.CvEnum;
using MPTC_API.Models.Attendance;
using MPTC_API.Services.Attendance;
using MPTC_API.Models.Attendance.MemberDTO;


namespace MPTC_API.Controllers.Attendance
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AttendanceController : ControllerBase
    {
        
        private readonly RecognitionService _recognitionService;

        public MptcContext _context = new MptcContext();

        public AttendanceController(RecognitionService recognitionService, MptcContext context)
        {
        
            _recognitionService = recognitionService;
            _context = context;
        }

       

        [HttpGet("records")]
        public async Task<IActionResult> GetAttendancesRecords()
        {

            try{
                List<MPTC_API.Models.Attendance.Attendance> attendances = _context.Attendances.OrderByDescending(a => a.Date).ToList();
                List<AttendanceDTO> attendanceDTOs =  AttendanceService.MapAttendanceToDTO(attendances);
                return Ok(attendanceDTOs);

            }catch(Exception e){
                return BadRequest(e.Message);
            }
           

            

        }

     
     

     
        
          




        

    }
}
