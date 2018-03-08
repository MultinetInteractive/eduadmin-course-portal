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
    public class DataMaskAttributeAdapter : AttributeAdapterBase<DataMaskAttribute>
    {
        private readonly DataMaskAttribute _attribute;

        public DataMaskAttributeAdapter(DataMaskAttribute attribute, IStringLocalizer stringLocalizer) : base(attribute, stringLocalizer)
        {
            _attribute = attribute;
        }

        public override void AddValidation(ClientModelValidationContext context)
        {
            MergeAttribute(context.Attributes, "data-mask", _attribute.Mask);
        }

        public override string GetErrorMessage(ModelValidationContextBase validationContext)
        {
            return GetErrorMessage(validationContext.ModelMetadata, validationContext.ModelMetadata.GetDisplayName());
        }
    }
}
