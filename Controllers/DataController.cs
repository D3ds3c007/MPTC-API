using Microsoft.AspNetCore.Mvc;
using MPTC_API.Data;
using MPTC_API.Models.Attendance;
using MPTC_API.Services.Authentication;
using Microsoft.AspNetCore.Identity;
using MPTC_API.Models.Attendance.MemberDTO;
using System.Text.Json;
using EllipticCurve.Utils;
using MPTC_API.Services.Attendance;



namespace MPTC_API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DataController : ControllerBase
    {
        private readonly UserManager<Member> _userManager;
        private readonly IEmailService _emailService;





        public MptcContext _context = new MptcContext();

        public DataController(UserManager<Member> userManager, IEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
        }

        [HttpGet("form-data")]
        public async Task<IActionResult> GetMultiStepFormData()
        {
            EmployeeFormDataDTO employeeFormDataDTO = DataService.GetEmployeeFormDataDTO(_context);
            return Ok(employeeFormDataDTO);
        }

        [HttpPost("upload-employee")]
        public async Task<IActionResult> UploadEmployee([FromBody] JsonElement formData)
        {
            try{

                StaffService.AddStaff(formData, _context, _userManager, _emailService);
                //Create the staff schedule
                List<Schedule> schedules = new List<Schedule>();

                //Get all day of week string by id
                foreach(DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
                {
                    var scheduleOfDay = formData.GetProperty(day.ToString());
                    if(scheduleOfDay.GetProperty("open").GetBoolean())
                    {

                        TimeSpan begin = TimeSpan.Parse(scheduleOfDay.GetProperty("from").GetString());
                        TimeSpan end = TimeSpan.Parse(scheduleOfDay.GetProperty("to").GetString());

                        //create the schedule object and add it the schedules list
                        Schedule schedule = new Schedule();
                        schedule.DayOfWeek = day;
                        schedule.Begin = begin;
                        schedule.End = end;
                        // Console.WriteLine($"{day.ToString()} is open from {begin} to {end}");
                    }
                   
                }
                


                //access the picture array
                //parse the form data to json
                

                // var pictureArray = formData.GetProperty("picture").EnumerateArray();

                // foreach (var picture in pictureArray)
                // {
                //     string base64String = picture.GetProperty("preview").GetString();

                //     Console.WriteLine(base64String);
                // }
            }
                
                
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
            return Ok();
        }
        
          




        

       

    }
}
