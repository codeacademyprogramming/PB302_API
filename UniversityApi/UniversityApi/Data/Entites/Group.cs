namespace UniversityApi.Data.Entites
{
    public class Group
    {
        public int Id { get; set; }
        public string No { get; set; }
        public byte Limit { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
    }
}
