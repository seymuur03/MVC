using System.ComponentModel.DataAnnotations;

namespace PartialView.pustok.ViewModels.LoginingToUserPanel
{
    public class LoginVm
    {
        [Required]
        public string UserName_Email {  get; set; }
        [Required]
        [DataType(DataType.Password)]
        [MinLength(10)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
