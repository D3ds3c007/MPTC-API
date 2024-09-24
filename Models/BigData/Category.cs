using System.ComponentModel.DataAnnotations;

namespace MPTC_API.Models.BigData
{
    public class Category
    {
        [Key]
        public int IdCategory { get; set; }

        [Required(ErrorMessage = "CategoryName is required and cannot be empty")]
        public String CategoryName { get; set; }

        //navigation property
        public virtual ICollection<Resource> Resources { get; set; }

    }

}
