using System.ComponentModel.DataAnnotations.Schema;

namespace UniversityApi.Data.Entites
{
    public class Group:AuditEntity
    {
        public string? No { get; set; }
        public byte Limit { get; set; }
        public List<Student> Students { get; set; } 
    }
}
