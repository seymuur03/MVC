using System.ComponentModel.DataAnnotations;

namespace PartialView.pustok.Areas.Manage.Attributes
{
    public class TypeAttribute:ValidationAttribute
    {
        private readonly string[] _types;

        public TypeAttribute(params string[] types)
        {
            _types = types;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                if (!_types.Contains(file.ContentType))
                {
                    return new ValidationResult("File type is not suitable");
                }
            }
            return ValidationResult.Success ;
        }
    }
}
