using System.ComponentModel.DataAnnotations;

namespace PartialView.pustok.ViewModels.UserAccountDetailsVM
{
    public class UserUpdateProfileVm
    {
        public string FullName { get; set; }
        public string UserName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [DataType(DataType.Password)]
        [Compare("NewPassword")]
        public string ConfirmNewPassword { get; set; }

    }
}
