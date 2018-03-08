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
    public class CivRegNumberAttributeAdapter : AttributeAdapterBase<CivRegNumberAttribute>
    {
        public CivRegNumberAttributeAdapter(CivRegNumberAttribute attribute, IStringLocalizer stringLocalizer) : base(
            attribute, stringLocalizer)
        {
        }

        public override void AddValidation(ClientModelValidationContext context)
        {
            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-civregnr", GetErrorMessage(context));
        }

        public override string GetErrorMessage(ModelValidationContextBase validationContext)
        {
            return GetErrorMessage(validationContext.ModelMetadata, validationContext.ModelMetadata.GetDisplayName());
        }
    }
}
