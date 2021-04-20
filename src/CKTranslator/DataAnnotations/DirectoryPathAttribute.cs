using System.ComponentModel.DataAnnotations;
using System.IO;

namespace CKTranslator.DataAnnotations
{
    /// <summary>
    /// Validates directory paths
    /// </summary>
    public sealed class DirectoryPathAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
            => Directory.Exists((string?)value)
                ? ValidationResult.Success :
                new("Directory does not exists");
    }
}
