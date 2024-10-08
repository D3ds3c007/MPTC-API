using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.IdentityModel.Tokens;
using MPTC_API.Data;
using MPTC_API.Models.Attendance;
using MPTC_API.Models.Attendance.MemberDTO;

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
    }

   
}
