using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using MPTC_API.Data;
using MPTC_API.Models.Attendance;
using MPTC_API.Models.Attendance.StaffDTO;
using MPTC_API.Services.Authentication;

namespace MPTC_API.Services.Attendance
{
    public class StaffService
    {
        
        public static string GenerateMatricule(String fullName, int sequenceValue, DateTime registrationDate, int VenueId)
        {
            string[] nameParts = fullName.Split(' ');
            string initials = string.Concat(nameParts.Select(part => part[0]));

            // Convert prefix to uppercase

            string timestamp = registrationDate.ToString($"yyM{VenueId}");

            return $"{initials}{timestamp}{sequenceValue}";
        }

        public static Staff AddStaff(JsonElement formData, MptcContext _context, UserManager<Member> _userManager, IEmailService emailService)
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
                staff.HomeAddress = formData.GetProperty("homeAddress").GetString();

                //Job Information
                staff.VenueId = Int32.Parse(formData.GetProperty("venue").GetString());
                staff.PhoneNumber = formData.GetProperty("phoneNumber").GetString();
                staff.EmailAddress = formData.GetProperty("email").GetString();
                staff.IDCardNumber = formData.GetProperty("idCard").GetString();
                staff.PrivilegeId = Int32.Parse(formData.GetProperty("role").GetString());

                _context.Add(staff);
                int entries = _context.SaveChanges();
                if(entries == 0)
                {
                    throw new Exception("An error occurred durint employee registration. Please try again later.");
                }

                //Check if staf has login access then register it as member
                if(formData.GetProperty("loginAccess").GetBoolean())
                {
                    //generate random string for password including these condition Passwords must have at least one non alphanumeric character. Passwords must have at least one uppercase ('A'-'Z')

                    string password = GeneratePassword(12);
                     Task<IdentityResult> result =  AccountService.RegisterAsync(staff, _userManager, password);
                    Console.WriteLine(result);
                    if(result.Result.Succeeded)
                    {
                        //send email to staff with password
                        emailService.SendWelcomeEmail(staff.EmailAddress, password);
                    }
                }
                return staff;

            }catch(Exception e){
                throw new Exception("An error occurred during employee registration. Please try again later.\n Error: " + e.Message);
            }
                  
                
        }
        
        public static List<StaffDTO> GetStaffDTOs(MptcContext _context, RecognitionService _recognitionService)
        {
            List<Staff> staffs = _context.Staffs.ToList();
            List<StaffDTO> staffDTOs = new List<StaffDTO>();
            
            if(staffs == null)
            {
                throw new Exception("No record found in database");
            }
            foreach(Staff staff in staffs)
            {
                 //map staff to staffdto
                var pictures = _recognitionService.GetEmployeeImage(staff.IdStaff);
                List<EmployeeImage> employeeImage = pictures.Result.ToList();
                List<Schedule> schedules = staff.Schedules.ToList();
                schedules.ForEach(schedule => schedule.Staff = null);

                //make all descsriptor null in employeeImage using LINQ
                employeeImage.ForEach(image => image.Descriptor = null);
                StaffDTO staffDTO = new StaffDTO{
                       IdStaff = staff.IdStaff,
                       Matricule = staff.Matricule,
                       FullName = staff.StaffName + " " + staff.FirstName,
                       Birth = staff.Birth.Date,
                       Gender = staff.Gender,
                       NationalId = staff.IDCardNumber,
                       PhoneNumber = staff.PhoneNumber,
                       Email = staff.EmailAddress,
                       Nationality = staff.Nationality.NationalityName,
                       Schedule = schedules,
                       Privilege = staff.Privilege.PrivilegeName,
                       Picture = employeeImage
                };
                staffDTOs.Add(staffDTO);
             
                
            }
             
            return staffDTOs;
        }

        public static string GeneratePassword(int length)
        {
            if (length < 2) throw new ArgumentException("Password length must be at least 2 to meet requirements.");
            
            Random random = new Random();
            
            // Define character sets
            string uppercaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string nonAlphanumericChars = "!@#$%^&*()_+=-{}[]:;'<>,.?/";
            string alphaChars = "abcdefghijklmnopqrstuvwxyz";
            string numericChars = "0123456789";

            var password = new StringBuilder();
            
            // Ensure at least one uppercase letter
            password.Append(uppercaseLetters[random.Next(uppercaseLetters.Length)]);

            // Ensure at least one non-alphanumeric character
            password.Append(nonAlphanumericChars[random.Next(nonAlphanumericChars.Length)]);

            // Fill remaining with random alphanumeric characters
            for (int i = 2; i < length; i++)
            {
                string allCharacters = uppercaseLetters + alphaChars + nonAlphanumericChars + numericChars;
                password.Append(allCharacters[random.Next(allCharacters.Length)]);
            }

            // Shuffle the string to avoid predictable patterns
            return new string(password.ToString().OrderBy(c => random.Next()).ToArray());
        }

        


    }

}
