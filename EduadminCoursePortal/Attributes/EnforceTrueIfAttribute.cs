using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using EduadminCoursePortal.Resources;

namespace EduadminCoursePortal.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class EnforceTrueIfAttribute : ValidationAttribute
    {
        public readonly string PropertyName;
        public readonly object DesiredValue;

        public EnforceTrueIfAttribute(string propertyName, object desiredValue)
        {
            PropertyName = propertyName;
            DesiredValue = desiredValue;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var instance = validationContext.ObjectInstance;
            var type = instance.GetType();
            var propValue = type.GetProperty(PropertyName)?.GetValue(instance, null);

            return propValue?.ToString() != DesiredValue.ToString() ? ValidationResult.Success : new ValidationResult(ErrorMessage);
        }
    }
}
