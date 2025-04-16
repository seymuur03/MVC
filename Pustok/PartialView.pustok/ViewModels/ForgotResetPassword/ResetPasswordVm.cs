using System.ComponentModel.DataAnnotations;

namespace PartialView.pustok.ViewModels.ForgotResetPassword
{
	public class ResetPasswordVm
	{
		[Required]
		[DataType(DataType.Password)]
		public string NewPassword { get; set; }
		[Required]
		[DataType(DataType.Password)]
		[Compare(nameof(NewPassword))]
		public string ConfirmNewPassword { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
