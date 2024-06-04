using System.ComponentModel.DataAnnotations;

namespace UniversityApi.Dtos.GroupDtos
{
    public class GroupCreateDto
    {
        [Required]
        [MaxLength(5)]
        [MinLength(4)]
        public string No { get; set; }
        [Required]
        [Range(5,18)]
        public byte Limit { get; set; }
    }
}
