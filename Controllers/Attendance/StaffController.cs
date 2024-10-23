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
using MPTC_API.Models.Attendance.StaffDTO;
using MongoDB.Bson;



namespace MPTC_API.Controllers.Attendance
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class StaffController : ControllerBase
    {
        private readonly UserManager<Member> _userManager;
        private readonly IEmailService _emailService;
        private readonly RecognitionService _recognitionService;

        public MptcContext _context = new MptcContext();

        public StaffController(UserManager<Member> userManager, IEmailService emailService, RecognitionService recognitionService)
        {
            _userManager = userManager;
            _emailService = emailService;
            _recognitionService = recognitionService;
        }

     
        [HttpPost("upload-employee")]
        public async Task<IActionResult> UploadEmployee([FromBody] JsonElement formData)
        {
            try{
                
                ArrayEnumerator pictureArray = formData.GetProperty("picture").EnumerateArray();
                List<EmployeeImage> employeeImages = new List<EmployeeImage>();
                
                List<Dictionary<string, object>>  descriptors = _recognitionService.ExtractFaceDescriptorPerImage(pictureArray);
                if(descriptors == null)
                {
                    return BadRequest("No face detected in the uploaded image");
                }
                 
                
                Staff staff = StaffService.AddStaff(formData, _context, _userManager, _emailService);
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
                        schedules.Add(new Schedule
                        {
                            Staff = staff,
                            DayOfWeek = day,
                            Begin = begin,
                            End = end
                        });
                        // Console.WriteLine($"{day.ToString()} is open from {begin} to {end}");
                    }
                   
                }

                _context.AddRange(schedules);
                _context.SaveChanges();

                //loop descriptors
                foreach (Dictionary<string, object> descriptor in descriptors)
                {
                    employeeImages.Add(new EmployeeImage{
                        //generate a objectId for id property
                        Id = ObjectId.GenerateNewId(),
                        IdStaff = staff.IdStaff,
                        StaffName = staff.StaffName + " " + staff.FirstName,
                        Base64Image = (string) descriptor["picture"],
                        Descriptor = descriptor["descriptor"] as float[]
                    });
                }


                var result = _recognitionService.InsertEmployeeImages(employeeImages);

                return Ok("Staff registered successfully") ;               
            }
                
                
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
            return Ok();
        }

        [HttpGet("employees")]
        public IActionResult GetEmployees()
        {
            try{
                List<StaffDTO> staffDTOs = StaffService.GetStaffDTOs(_context, _recognitionService);
                return Ok(staffDTOs);

            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("matricule-suggestions")]
        public IActionResult GetMatriculeSuggestions([FromQuery] string query)
        {
            return Ok(StaffService.GetMatriculeSuggestions(query, _context));          
        }
        
          




        

       

    }
}
