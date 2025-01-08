using System.ComponentModel.DataAnnotations;
using MPTC_API.Models.Education;

namespace MPTC_API.Models.DTO
{
    public class ExamDTO
    {
        public int IdExam { get; set; }
        public string Period { get; set; }
        public string Session { get; set; }
        public string Subject { get; set; }
        public string Level { get; set; }
        public string Uripath { get; set; }
        public string UripathAssetNote { get; set; }
        public string DateExam { get; set; }
        public int StaffId { get; set; }

    }

}
