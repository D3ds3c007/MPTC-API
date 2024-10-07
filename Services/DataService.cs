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
    }

   
}
