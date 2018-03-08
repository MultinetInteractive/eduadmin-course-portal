using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.CodeAnalysis;
using Microsoft.Data.Edm.Validation;
using Microsoft.Extensions.Localization;
using EduadminCoursePortal.Attributes;

namespace EduadminCoursePortal.Adapters
{
    public class RequiredIfAttributeAdapter : AttributeAdapterBase<RequiredIfAttribute>
    {
        public RequiredIfAttributeAdapter(RequiredIfAttribute attribute, IStringLocalizer stringLocalizer) : base(
            attribute, stringLocalizer)
        {
        }

        public override void AddValidation(ClientModelValidationContext context)
        {
            var smt = (Microsoft.AspNetCore.Mvc.Rendering.ViewContext) context.ActionContext;
            var property = context.ModelMetadata.ContainerType.GetProperty(Attribute.PropertyName);

            var model = smt.ViewData.ModelExplorer.Container.Model;
            
            var typeProperty = context.ModelMetadata.ContainerType.GetProperty(property.Name);
            var propertyValue = typeProperty?.GetValue(smt.ViewData.ModelExplorer.Container.Model, null);

            if (propertyValue?.GetType() == typeof(bool) && (bool)propertyValue != true)
                return;

            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-required", GetErrorMessage(context));
        }

        public override string GetErrorMessage(ModelValidationContextBase validationContext)
        {
            return GetErrorMessage(validationContext.ModelMetadata, validationContext.ModelMetadata.GetDisplayName());
        }
    }
}
