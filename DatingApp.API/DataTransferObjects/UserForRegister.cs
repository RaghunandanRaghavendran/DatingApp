using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.DataTransferObjects
{
    public class UserForRegister
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [StringLength(20,MinimumLength = 4,ErrorMessage="Password should be in range 4 to 20")]
        public string Password { get; set; }
    }
}