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

        [HttpPost("record")]
        public async Task<IActionResult> AddAttendance([FromBody]AttendanceRecordDTO attendanceRecordDTO)
        {
            //find staff by matricule
            var staff = _context.Staffs.FirstOrDefault(s => s.Matricule == attendanceRecordDTO.Matricule);

            if(staff == null){
                return BadRequest("Staff not found");
            }
            //Map to Attendance
            MPTC_API.Models.Attendance.Attendance attendance = new MPTC_API.Models.Attendance.Attendance
            {
                Staff = staff,
                StaffId = staff.IdStaff,
                ClockInTime = attendanceRecordDTO.ClockIn,
                ClockOutTime = attendanceRecordDTO.ClockOut,
                Date = attendanceRecordDTO.Date.ToUniversalTime(),
                Remark = attendanceRecordDTO.Remarks,
                LastDetectedTime = attendanceRecordDTO.Date.ToUniversalTime()
            };

            try{
                _context.Attendances.Add(attendance);
                _context.SaveChanges();


                AttendanceDTO attendanceDTO = new AttendanceDTO
                {
                    AttendanceId = attendance.IdAttendance,
                    Matricule = attendance.Staff.Matricule,
                    StaffName = attendance.Staff.FirstName + " " + attendance.Staff.StaffName,
                    recordDate = attendance.Date,
                    timeIn = attendance.ClockInTime,
                    timeOut = attendance.ClockOutTime,
                    isLate = AttendanceService.IsLate(attendance),
                    remark = attendance.Remark ?? "N/A"
                };
                return Ok(attendanceDTO);
            }catch(Exception e){
                return BadRequest(e.Message + e.StackTrace + e.InnerException);
            }
        }

        [HttpPut("record")]
        public async Task<IActionResult> UpdateAttendance([FromBody]AttendanceRecordDTO attendanceRecordDTO)
        {
            //find staff by matricule
            var staff = _context.Staffs.FirstOrDefault(s => s.Matricule == attendanceRecordDTO.Matricule);

            if(staff == null){
                return BadRequest("Staff not found");
            }
            //Map to Attendance
            MPTC_API.Models.Attendance.Attendance attendance = new MPTC_API.Models.Attendance.Attendance
            {
                Staff = staff,
                IdAttendance = (int)attendanceRecordDTO.AttendanceId,
                StaffId = staff.IdStaff,
                ClockInTime = attendanceRecordDTO.ClockIn,
                ClockOutTime = attendanceRecordDTO.ClockOut,
                Date = attendanceRecordDTO.Date.ToUniversalTime(),
                Remark = attendanceRecordDTO.Remarks,
                LastDetectedTime = attendanceRecordDTO.Date.ToUniversalTime()
            };

            try{
                _context.Attendances.Update(attendance);
                _context.SaveChanges();

                return Ok(attendance);
            }catch(Exception e){
                return BadRequest(e.Message + e.StackTrace + e.InnerException);
            }

     
     

     
        
          



    }
}
}
