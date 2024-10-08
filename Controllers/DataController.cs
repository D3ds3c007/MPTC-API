using Microsoft.AspNetCore.Mvc;
using MPTC_API.Data;
using MPTC_API.Models.Attendance;
using MPTC_API.Services.Authentication;
using Microsoft.AspNetCore.Identity;
using MPTC_API.Models.Attendance.MemberDTO;
using System.Text.Json;



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
                staff.Birth = DateTime.Parse(formData.GetProperty("birthDate").GetString());
                staff.Gender = formData.GetProperty("gender").GetString();
                staff.MaritalStatus = formData.GetProperty("maritalStatus").GetString();
                staff.NationalityId = formData.GetProperty("Nationality").GetInt32();
                staff.HomeAddress = "N/A";

                //Job Information
                staff.VenueId = formData.GetProperty("venue").GetInt32();
                staff.PhoneNumber = formData.GetProperty("phoneNumber").GetString();
                staff.EmailAddress = formData.GetProperty("email").GetString();
                staff.IDCardNumber = formData.GetProperty("idCard").GetString();
                staff.PrivilegeId = formData.GetProperty("role").GetInt32();

                //Create the staff schedule
                Schedule schedule = new Schedule();
                


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
