using System.ComponentModel.DataAnnotations;
using MPTC_API.Models.Attendance;

namespace MPTC_API.Models.DTO
{
    public class ExamUpdateDTO
    {
        public int IdExam { get; set; }
        public int PeriodId { get; set; }
        public int Session { get; set; }
        public int SubjectId { get; set; }
        public int LevelId { get; set; }
        public String UripathAssetNote { get; set; }
        public String Uripath { get; set; }
        public DateTime DateCreated { get; set; }
        public int StaffId { get; set; }

    }

}
