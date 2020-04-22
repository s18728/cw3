using System.ComponentModel.DataAnnotations;

namespace cw3.DTOs
{
    public class LoginRequestDTO
    {
        [Required]
        public string Login { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
