using System.ComponentModel.DataAnnotations;

namespace MPTC_API.Models.Education
{
    public class SubjectSection
    {
        [Key]
        public int IdSubjectSection { get; set; }

        [Required(ErrorMessage = "SubjectId is required and cannot be empty")]
        public int SubjectId { get; set; }

        [Required(ErrorMessage = "SectionId is required and cannot be empty")]
        public int SectionId { get; set; }

        [Required(ErrorMessage = "Scale is required and cannot be empty")]
        public double Scale { get; set; }

        [Required(ErrorMessage = "LevelId is required and cannot be empty")]
        public int LevelId { get; set; }

        //navigation property
        public virtual Subject Subject { get; set; }
        public virtual Section Section { get; set; }
        public virtual Level Level { get; set; }
        public virtual ICollection<TempResultSection> TempResultSections { get; set; }
        public virtual ICollection<ResultNoteSection> ResultNoteSections { get; set; }


    }

}
