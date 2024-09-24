using System.ComponentModel.DataAnnotations;

namespace MPTC_API.Models.BigData
{
    public class ResourceType
    {
        [Required(ErrorMessage = "IdResourceType is required and cannot be empty")]
        [Key]
        public int IdResourceType { get; set; }

        [Required(ErrorMessage = "TypeName is required and cannot be empty")]
        public String TypeName { get; set; }

        //navigation property
        public virtual ICollection<Resource> Resources { get; set; }

    }

}
