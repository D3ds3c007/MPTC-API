using System.ComponentModel.DataAnnotations;

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

    }

}
