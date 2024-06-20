namespace UniversityApp.UI.Models
{
    public class StudentCreateRequest
    {
        public int GroupId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }
        public IFormFile File { get; set; }
    }
}
