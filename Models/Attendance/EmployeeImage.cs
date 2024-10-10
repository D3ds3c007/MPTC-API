using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MPTC_API.Models.Attendance
{
   
    [BsonNoId]
    public class EmployeeImage
    {
        [BsonElement("_id")]
        public ObjectId Id { get;    set; }
        public int IdStaff { get; set; }
        public string? Base64Image { get; set; }
        public float[]? Descriptor { get; set; }

     

    }

}
