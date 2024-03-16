using System.ComponentModel.DataAnnotations;

namespace KLTN_E.ViewModels
{
    public class ProfileVM
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string ProfileImage {  get; set; }
    }
}
