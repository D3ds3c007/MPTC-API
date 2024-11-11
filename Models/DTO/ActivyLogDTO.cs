using System.ComponentModel.DataAnnotations;

namespace MPTC_API.Models.DTO
{
    public class ActivityLogDTO
    {
      
       public string Matricule { get; set; }
       public string FirstName { get; set; }
       public string EventType { get; set; }
       public int year {get; set;}
       public DateTime EventTime { get; set; }

       
    }

}
