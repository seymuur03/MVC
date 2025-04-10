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
            List<IFormFile> _files = new List<IFormFile>();
            if (value is List<IFormFile> files)
                _files = files;
            if (value is IFormFile file)
                _files.Add(file);
            foreach (var _file in _files)
            {
                if (!_types.Contains(_file.ContentType))
                {
                    return new ValidationResult("File type is not suitable");
                }
            }
           
            return ValidationResult.Success ;
        }
    }
}
