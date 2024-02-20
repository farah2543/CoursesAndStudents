using System.ComponentModel.DataAnnotations;

namespace WebAPITrail.Models
{
    public class RegisterModel
    {
        [Required]
        public string username { get; set; }
        [Required]
        public string password { get; set; }
        [Required]
        public string name { get; set; }
        [Required]
        public string email { get; set; }
        [Required]
        public string phone { get; set; }



    }
}
