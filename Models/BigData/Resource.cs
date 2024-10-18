using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MPTC_API.Models.Attendance;

namespace MPTC_API.Models.BigData
{
    public class Resource
    {
        [Key]
        public int IdResource { get; set; }

        [Required(ErrorMessage = "ResourceName is required and cannot be empty")]
        public String ResourceName { get; set; }

        [Required(ErrorMessage = "Description is required and cannot be empty")]
        public String Description { get; set; }

        [Required(ErrorMessage = "LevelId is required and cannot be empty")]
        public int LevelId { get; set; }

        [Required(ErrorMessage = "SubjectId is required and cannot be empty")]
        public int SubjectId { get; set; }

        [Required(ErrorMessage = "ResourceTypeId is required and cannot be empty")]
        public int ResourceTypeId { get; set; }

        [Required(ErrorMessage = "CategoryId is required and cannot be empty")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "StaffId is required and cannot be empty")]
        public int StaffId { get; set; }

        [Required(ErrorMessage = "Uripath is required and cannot be empty")]
        public String Uripath { get; set; }

        [Required(ErrorMessage = "DateCreatedAt is required and cannot be empty")]
        public DateTime DateCreatedAt { get; set; }

        [Required(ErrorMessage = "Size is required and cannot be empty")]
        public double Size { get; set; }

        //navigation property
        public virtual ResourceType ResourceType { get; set; }
        public virtual Category Category { get; set; }
        [JsonIgnore]
        public virtual Staff Staff { get; set; }
        public virtual ICollection<ResourceInteraction> ResourceInteractions { get; set; }
    }

}
