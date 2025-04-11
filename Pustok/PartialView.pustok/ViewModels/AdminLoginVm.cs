using System.ComponentModel.DataAnnotations;

namespace PartialView.pustok.ViewModels
{
    public class AdminLoginVm
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [MinLength(10)]
        public string Password { get; set; }

    }
}
