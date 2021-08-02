using System.ComponentModel.DataAnnotations;

namespace MoviesApi.Dtos
{
    public class LoginDto
    {
        [Required,StringLength(256),EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public bool RemmberMe { get; set; }
    }
}