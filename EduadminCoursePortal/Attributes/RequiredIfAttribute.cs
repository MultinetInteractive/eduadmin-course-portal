using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace EduadminCoursePortal.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class RequiredIfAttribute : RequiredAttribute
    {
        public readonly string PropertyName;
        public readonly object DesiredValue;

        public RequiredIfAttribute(string propertyName, object desiredValue)
        {
            PropertyName = propertyName;
            DesiredValue = desiredValue;

        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            var instance = context.ObjectInstance;
            var type = instance.GetType();
            var propValue = type.GetProperty(PropertyName.ToString())?.GetValue(instance, null);

            return propValue?.ToString() != DesiredValue.ToString() ? ValidationResult.Success : new ValidationResult(ErrorMessage);
        }
    }
}
