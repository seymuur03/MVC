using System.ComponentModel.DataAnnotations;

namespace PartialView.pustok.ViewModels.ForgotResetPassword
{
	public class ForgotPasswordVm
	{
		[Required]
		[EmailAddress]
		[DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string UserName { get; set; }
    }
}
