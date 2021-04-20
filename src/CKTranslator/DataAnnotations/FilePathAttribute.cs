using System.ComponentModel.DataAnnotations;
using System.IO;

namespace CKTranslator.DataAnnotations
{
    public sealed class FilePathAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
            => File.Exists((string?)value)
                ? ValidationResult.Success :
                new("Directory does not exists");
    }
}
