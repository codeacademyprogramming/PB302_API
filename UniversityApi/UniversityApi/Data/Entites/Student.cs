namespace UniversityApi.Data.Entites
{
    public class Student:AuditEntity
    {
        public int GroupId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }
        public Group Group { get; set; }
    }
}
