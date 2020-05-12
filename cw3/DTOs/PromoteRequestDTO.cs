using System.ComponentModel.DataAnnotations;

namespace cw3.DTOs
{
    public class PromoteRequestDTO
    {
        [Required]
        public int StudiesId { get; set; }
        [Required]
        public int Semester { get; set; }
    }
}
