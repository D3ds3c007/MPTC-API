using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using MPTC_API.Models.Education;

namespace MPTC_API.Models.DTO
{
    public class ExamModifyDTO
    {
        public int IdExam { get; set; }
        public int PeriodId { get; set; }
        public int Session { get; set; }
        public int SubjectId { get; set; }
        public int LevelId { get; set; }
        public string DateExam { get; set; }

    }

}
