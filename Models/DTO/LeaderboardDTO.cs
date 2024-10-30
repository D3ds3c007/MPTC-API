using System.ComponentModel.DataAnnotations;

namespace MPTC_API.Models.DTO
{
    public class LeaderboardDTO
    {
      
        public int Rank {get; set;}
        public int StaffId {get; set;}
        public string StaffName {get; set;}
        public string Matricule {get; set;}
        public int Year {get; set;}
        public int Month {get; set;}
        public int LatenessCount {get; set;}
        public int AbsenceCount {get; set;}
        public string PunctualityRating {get; set;}



        
       
       
    }

}
