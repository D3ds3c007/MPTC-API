using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.IdentityModel.Tokens;
using MPTC_API.Data;
using MPTC_API.Models.Attendance;
using MPTC_API.Models.Attendance.MemberDTO;
using MPTC_API.Models.DTO;
using MPTC_API.Models.Education;
using System.Globalization;

namespace MPTC_API.Services.Authentication
{
    public class DataService
    {
        public static List<VenueDTO> GetVenueDTOs(MptcContext context)
        {
            List<VenueDTO> venueDTOs = new List<VenueDTO>();
            List<Venue> venues = context.Venues.ToList();
            foreach (Venue venue in venues)
            {
                VenueDTO venueDTO = new VenueDTO();
                venueDTO.IdVenue = venue.IdVenue;
                venueDTO.Name = venue.VenueName;
                venueDTOs.Add(venueDTO);
            }
            return venueDTOs;
        }

        public static List<PrivilegeDTO> GetPrivilegeDTOs(MptcContext context)
        {
            List<PrivilegeDTO> privilegeDTOs = new List<PrivilegeDTO>();
            List<Privilege> privileges = context.Privileges.ToList();
            foreach (Privilege privilege in privileges)
            {
                PrivilegeDTO privilegeDTO = new PrivilegeDTO();
                privilegeDTO.IdPrivilege = privilege.IdPrivilege;
                privilegeDTO.Name = privilege.PrivilegeName;
                privilegeDTOs.Add(privilegeDTO);
            }
            return privilegeDTOs;
        }

        //list of nationalities using DTO
        public static List<NationalityDTO> GetNationalities(MptcContext context)
        {
            List<NationalityDTO> nationalityDTOs = new List<NationalityDTO>();
            List<Nationality> nationalities = context.Nationalitys.ToList();

            foreach (Nationality nationality in nationalities)
            {
                //map to nationalityDTOs
                NationalityDTO nationalityDTO = new NationalityDTO();
                nationalityDTO.IdNationality = nationality.IdNationality;
                nationalityDTO.Name = nationality.NationalityName; 
                nationalityDTOs.Add(nationalityDTO);                    
            }

            return nationalityDTOs;

        }

        //get EmployeeFormDataDTO
        public static EmployeeFormDataDTO GetEmployeeFormDataDTO(MptcContext _context)
        {
            EmployeeFormDataDTO employeeFormDataDTO = new EmployeeFormDataDTO();
            employeeFormDataDTO.Venues = GetVenueDTOs(_context);
            employeeFormDataDTO.Privileges = GetPrivilegeDTOs(_context);
            employeeFormDataDTO.Nationalities = GetNationalities(_context);
            return employeeFormDataDTO;
        }

        public static List<LevelDTO> GetLevels(MptcContext context)
        {
            List<LevelDTO> levelDTOs = new List<LevelDTO>();
            List<Level> levels = context.Levels.ToList();

            foreach (Level level in levels)
            {
                LevelDTO levelDTO = new LevelDTO();
                    levelDTO.IdLevel = level.IdLevel;
                    levelDTO.LevelName = level.LevelName;

                levelDTOs.Add(levelDTO);
            }

            return levelDTOs;
        }

        public static List<SubjectDTO> GetSubjects(MptcContext context)
        {
            List<SubjectDTO> subjectDTOs = new List<SubjectDTO>();
            List<Subject> subjects = context.Subjects.ToList();

            foreach (Subject subject in subjects)
            {
                SubjectDTO subjectDTO = new SubjectDTO();
                    subjectDTO.IdSubject = subject.IdSubject;
                    subjectDTO.SubjectName = subject.SubjectName;

                subjectDTOs.Add(subjectDTO);
            }

            return subjectDTOs;
        }

        public static List<PeriodDTO> GetPeriods(MptcContext context)
        {
            List<PeriodDTO> periodDTOs = new List<PeriodDTO>();
            List<Period> periods = context.Periods.ToList();

            foreach (Period period in periods)
            {
                PeriodDTO periodDTO = new PeriodDTO();
                    periodDTO.IdPeriod = period.IdPeriod;
                    periodDTO.Name = DataService.FormatDateRange(period.BeginDate, period.EndDate);

                periodDTOs.Add(periodDTO);
            }

            return periodDTOs;
        }

        public static string FormatDateRange(DateTime startDate, DateTime endDate)
        {
            // Get the short month names and year for both dates
            string startMonthYear = startDate.ToString("MMM yyyy", CultureInfo.InvariantCulture).ToUpper();
            string endMonthYear = endDate.ToString("MMM yyyy", CultureInfo.InvariantCulture).ToUpper();

            // Format as requested
            return $"({startMonthYear} - {endMonthYear})";
        }
    }

   
}
