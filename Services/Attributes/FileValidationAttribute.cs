using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;
using System;

namespace Services.Attributes
{
    /// <summary>
    /// Attribute for validating file extensions and size in MegaBytes (MB).
    /// </summary>
    /// <seealso cref="ValidationAttribute" />
    [AttributeUsage(AttributeTargets.Property)]
    public class FileValidationAttribute : ValidationAttribute
    {
        private readonly string[] _allowedExtensions;
        private readonly long _maxFileSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileValidationAttribute"/> class.
        /// </summary>
        /// <param name="allowedExtensions">The allowed extensions.</param>
        /// <param name="maxFileSize">Maximum size of the file in MegaBytes (MB).</param>
        public FileValidationAttribute(string[] allowedExtensions, long maxFileSize)
        {
            _allowedExtensions = allowedExtensions;
            _maxFileSize = maxFileSize * 1024 * 1024;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!_allowedExtensions.Contains(extension))
                {
                    return new ValidationResult($"Invalid file type. Allowed types are: {string.Join(", ", _allowedExtensions)}.");
                }

                if (file.Length > _maxFileSize)
                {
                    return new ValidationResult($"File size must not exceed {_maxFileSize} MB.");
                }
            }
            return ValidationResult.Success;
        }
    }
}
