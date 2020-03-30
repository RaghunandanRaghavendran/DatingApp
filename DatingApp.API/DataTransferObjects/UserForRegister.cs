using System;
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
        [Required]
        public string knownAs { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Country { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastActive { get; set; }
        public UserForRegister()
        {
            CreationDate = DateTime.Now;
            LastActive = DateTime.Now;
        }
    }
}