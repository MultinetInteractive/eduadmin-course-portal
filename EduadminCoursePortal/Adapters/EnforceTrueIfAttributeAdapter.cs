using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;
using EduadminCoursePortal.Attributes;

namespace EduadminCoursePortal.Adapters
{
    public class EnforceTrueIfAttributeAdapter : AttributeAdapterBase<EnforceTrueIfAttribute>
    {
        public readonly string PropertyName;
        public readonly object DesiredValue;

        public EnforceTrueIfAttributeAdapter(EnforceTrueIfAttribute attribute, IStringLocalizer stringLocalizer) : base(attribute, stringLocalizer)
        {
            DesiredValue = attribute.DesiredValue;
            PropertyName = attribute.PropertyName;
        }

        public override void AddValidation(ClientModelValidationContext context)
        {
            var smt = (Microsoft.AspNetCore.Mvc.Rendering.ViewContext)context.ActionContext;
            var property = context.ModelMetadata.ContainerType.GetProperty(Attribute.PropertyName);

            var model = smt.ViewData.ModelExplorer.Container.Model;
            
            var typeProperty = context.ModelMetadata.ContainerType.GetProperty(property.Name);
            var propertyValue = typeProperty?.GetValue(smt.ViewData.ModelExplorer.Container.Model, null);

            if (propertyValue?.GetType() == typeof(bool) && (bool)propertyValue != true)
                return;

            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-enforcetrue", GetErrorMessage(context));
        }

        public override string GetErrorMessage(ModelValidationContextBase validationContext)
        {
            return GetErrorMessage(validationContext.ModelMetadata, validationContext.ModelMetadata.GetDisplayName());
        }
    }
}
