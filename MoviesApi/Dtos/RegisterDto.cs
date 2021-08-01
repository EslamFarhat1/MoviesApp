using System.ComponentModel.DataAnnotations;

namespace MoviesApi.Dtos
{
    public class RegisterDto
    {
        [Required,StringLength(256),EmailAddress]
        public string Email { get; set; }
        [Required,StringLength(256)]
        public string userName { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        
        
        
    }
}