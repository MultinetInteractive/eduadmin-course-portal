using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using EduadminCoursePortal.Resources;

namespace EduadminCoursePortal.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class OrgNumberAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace((string)value))
                return ValidationResult.Success;

            return Validation.IsCorrectOrgnr((string)value,out var message) ? ValidationResult.Success : new ValidationResult(ErrorMessage);
        }
    }
}
