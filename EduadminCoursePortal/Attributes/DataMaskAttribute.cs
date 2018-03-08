using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EduadminCoursePortal.Attributes
{
    public class DataMaskAttribute : ValidationAttribute
    {
        public readonly string Mask;
        public string LengthErrorMessage { get; set; }
        public bool IgnoreLength { get; set; }
        public int MinimumLength { get; set; }
        public int MaximumLength { get; set; }

        public DataMaskAttribute(string mask)
        {
            Mask = mask;
            MinimumLength = mask.Length;
            MaximumLength = mask.Length;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return ValidationResult.Success;
        }
    }
}
