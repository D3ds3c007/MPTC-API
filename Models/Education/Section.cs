using System.ComponentModel.DataAnnotations;

namespace MPTC_API.Models.Education
{
    public class Section
    {
        [Key]
        public int IdSection { get; set; }

        [Required(ErrorMessage = "SectionName is required and cannot be empty")]
        public String SectionName { get; set; }

        //navigation property
        public virtual ICollection<SubjectSection> SubjectSections { get; set; }

    }

}
