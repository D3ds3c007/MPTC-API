using System.ComponentModel.DataAnnotations;

namespace MPTC_API.Models.BigData
{
    public class ResourceInteraction
    {
        [Key]
        public int IdResourceInteraction { get; set; }

        [Required(ErrorMessage = "ResourceId is required and cannot be empty")]
        public int ResourceId { get; set; }

        [Required(ErrorMessage = "ViewNumber is required and cannot be empty")]
        public int ViewNumber { get; set; }
        
        //navigation property
        public virtual Resource Resource { get; set; }
    }

}
