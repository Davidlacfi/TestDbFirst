using System.ComponentModel.DataAnnotations;

namespace TestDbFirst.Models
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "E-mail")]
        public string Email
        {
            get;
            set;
        }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Jelszó")]
        public string Password
        {
            get;
            set;
        }

        public string Username
        {
            get;
            set;
        }
    }
}