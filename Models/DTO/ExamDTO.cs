using System.ComponentModel.DataAnnotations;
using MPTC_API.Models.Education;

namespace MPTC_API.Models.DTO
{
    public class ExamDTO
    {
        public int? IdExam { get; set; }
        public Period? Period { get; set; }
        public int? Session { get; set; }
        public Subject? Subject { get; set; }
        public Level? Level { get; set; }
        public String? Uripath { get; set; }
        public String? UripathAssetNote { get; set; }
        public DateTime? DateExam { get; set; }
        public int? StaffId { get; set; }

    }

}
