using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Localization;
using EduadminCoursePortal.Adapters;
using EduadminCoursePortal.Attributes;

namespace EduadminCoursePortal
{
    public class CustomValidationAttributeAdapterProvider : IValidationAttributeAdapterProvider
    {
        private readonly IValidationAttributeAdapterProvider _baseProvider = new ValidationAttributeAdapterProvider();

        public IAttributeAdapter GetAttributeAdapter(ValidationAttribute attribute, IStringLocalizer stringLocalizer)
        {
            switch (attribute)
            {
                case CivRegNumberAttribute _:
                    return new CivRegNumberAttributeAdapter(attribute as CivRegNumberAttribute, stringLocalizer);
                case OrgNumberAttribute _:
                    return new OrgNumberAttributeAdapter(attribute as OrgNumberAttribute, stringLocalizer);
                case RequiredIfAttribute _:
                    return new RequiredIfAttributeAdapter(attribute as RequiredIfAttribute, stringLocalizer);
                case DataMaskAttribute _:
                    return new DataMaskAttributeAdapter(attribute as DataMaskAttribute, stringLocalizer);
                case EnforceTrueIfAttribute _:
                    return new EnforceTrueIfAttributeAdapter(attribute as EnforceTrueIfAttribute, stringLocalizer);
            }

            return _baseProvider.GetAttributeAdapter(attribute, stringLocalizer);
        }
    }
}
