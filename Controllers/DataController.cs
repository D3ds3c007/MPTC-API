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




        public MptcContext _context = new MptcContext();

        public DataController(UserManager<Member> userManager)
        {
            _userManager = userManager;

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


                //Create Staff object

                Staff staff = new Staff();
                //Personal Information
                staff.FirstName = formData.GetProperty("firstName").GetString();
                staff.StaffName = formData.GetProperty("name").GetString();
                staff.Birth = DateTime.Parse(formData.GetProperty("birthDate").GetString()).ToUniversalTime();
                staff.Gender = formData.GetProperty("gender").GetString();
                staff.MaritalStatus = formData.GetProperty("maritalStatus").GetString();
                staff.NationalityId =  Int32.Parse(formData.GetProperty("Nationality").GetString());
                staff.HomeAddress = "N/A";

                //Job Information
                staff.VenueId = Int32.Parse(formData.GetProperty("venue").GetString());
                staff.PhoneNumber = formData.GetProperty("phoneNumber").GetString();
                staff.EmailAddress = formData.GetProperty("email").GetString();
                staff.IDCardNumber = formData.GetProperty("idCard").GetString();
                staff.PrivilegeId = Int32.Parse(formData.GetProperty("role").GetString());

                _context.Add(staff);
                _context.SaveChanges();
                
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
