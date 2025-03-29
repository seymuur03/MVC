using System.ComponentModel.DataAnnotations;

namespace PartialView.pustok.Areas.Manage.Attributes
{
    public class LengthAttribute: ValidationAttribute
    {
        private readonly int _length;

        public LengthAttribute(int lenght)
        {
            _length = lenght;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(value is IFormFile file)
            {
                if (file.Length >_length)
                {
                    return new ValidationResult("File Lenght is so big");
                }
            }
            return ValidationResult.Success;
        }
    }
}
