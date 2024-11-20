using Microsoft.EntityFrameworkCore;
using MPTC_API.Data;
using MPTC_API.Models.Attendance;

namespace MPTC_API.Services.Attendance
{
    public class TimeoffService
    {

        public static void addTimeoff(TimeOff timeoff, MptcContext _context)
        {
            _context.TimeOffs.Add(timeoff);
           int result =  _context.SaveChanges();

           if(result <= 0)
           {
                throw new Exception("An error occured during the request time off. Please try again");
           }
            
        }

        public static List<TimeOffDTO> getAllTimeOff(MptcContext _context)
        {
            List<TimeOff> timeOffs = _context.TimeOffs.ToList();
            List<TimeOffDTO> timeoffDTO = new List<TimeOffDTO>();

            foreach(TimeOff timeOff in timeOffs)
            {

                timeoffDTO.Add(new TimeOffDTO(){
                    IdTimeOff = timeOff.IdTimeOff,
                    employeeName = timeOff.Staff.FirstName + " " + timeOff.Staff.StaffName,
                    StaffMatricule = timeOff.Staff.Matricule,
                    BeginTimeOff = timeOff.BeginTimeOff,
                    EndTimeOff = timeOff.EndTimeOff
                });

            }

            return timeoffDTO;
        }



        

        
        
    }

}
