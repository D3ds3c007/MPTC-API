using Microsoft.AspNetCore.Mvc;
using MPTC_API.Data;
using MPTC_API.Services;
using Emgu.CV;
using Emgu.CV.CvEnum;
using MPTC_API.Models.Attendance;
using MPTC_API.Services.Attendance;



namespace MPTC_API.Controllers.Attendance
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TimeoffController : ControllerBase
    {
        
        private readonly RecognitionService _recognitionService;

        public MptcContext _context = new MptcContext();

        public TimeoffController(RecognitionService recognitionService)
        {
        
            _recognitionService = recognitionService;
        }

        [HttpPost("timeoff")]
        public async Task<IActionResult> addTimeoff([FromBody] TimeOffDTO timeoffDTO)
        {
            //Print the dto details
            Console.WriteLine("TimeoffDTO: "  + " " + timeoffDTO.StaffMatricule + " " + timeoffDTO.BeginTimeOff + " " + timeoffDTO.EndTimeOff);
            //find the staff with the matricule
            try{

                Staff staff = _context.Staffs.FirstOrDefault(s => s.Matricule == timeoffDTO.StaffMatricule);
                TimeOff timeoff = new TimeOff(){
                    Staff = staff,
                    BeginTimeOff = timeoffDTO.BeginTimeOff.ToUniversalTime(),
                    EndTimeOff = timeoffDTO.EndTimeOff.ToUniversalTime()
                };
                TimeoffService.addTimeoff(timeoff, _context);
                //return ok with success message

                return Ok("Your time-off request has been successfully submitted! We’ll notify you once it’s reviewed. Thank you!");

            }catch(Exception ex)
            {
                Console.WriteLine(ex.InnerException);
                return BadRequest(ex.Message);
            }
        } 
        
        [HttpGet("timeoffs")]

        public async Task<IActionResult> getAllTimeOff()
        {
            try{
                List<TimeOffDTO> timeoffDTO = TimeoffService.getAllTimeOff(_context);
                return Ok(timeoffDTO);
            }catch(Exception e){
                return BadRequest(e.Message);
            }
        }

        

     
     

     
        
          




        

       

    }
}
