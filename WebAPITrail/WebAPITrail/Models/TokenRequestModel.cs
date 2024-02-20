using System.ComponentModel.DataAnnotations;

namespace WebAPITrail.Models
{
    public class TokenRequestModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
