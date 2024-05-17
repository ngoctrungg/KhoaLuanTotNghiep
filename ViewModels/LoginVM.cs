using System.ComponentModel.DataAnnotations;

namespace KLTN_E.ViewModels
{
    public class LoginVM
    {
        [Display(Name = "User Name")]
        [Required(ErrorMessage = "The UserName is required")]
        [MaxLength(20, ErrorMessage = "Maximum 20 character")]
        public string UserName { get; set; }
        [Display(Name = "Password")]
        [Required(ErrorMessage = "The Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    } 
}
