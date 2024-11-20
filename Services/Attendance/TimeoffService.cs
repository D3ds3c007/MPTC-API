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

        

        
        
    }

}
