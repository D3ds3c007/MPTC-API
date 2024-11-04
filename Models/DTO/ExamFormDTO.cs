using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using MPTC_API.Models.Education;

namespace MPTC_API.Models.DTO
{
    public class ExamFormDTO
    {
        public int? PeriodId { get; set; }
        public int? Session { get; set; }
        public int? SubjectId { get; set; }
        public int? LevelId { get; set; }
        public IFormFile? Subject { get; set; }
        public IFormFile? Assetnote { get; set; }
        public string? DateExam { get; set; }
        public int? StaffId { get; set; }

    }

}
