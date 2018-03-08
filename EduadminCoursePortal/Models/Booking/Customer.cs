using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EduadminCoursePortal.Attributes;
using EduadminCoursePortal.Resources;

namespace EduadminCoursePortal.Models.Booking
{
    public class Customer
    {
        public int CustomerID { get; set; }

        public int CustomerGroupID { get; set; }

        [Required(ErrorMessage = "MustSubmitCompanyName")]
        [Display(Name = "Company")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "MustSubmitAddress")]
        [Display(Name = "Address")]
        public string Address1 { get; set; }

        [Required(ErrorMessage = "MustSubmitCity")]
        [Display(Name = "City")]
        public string City { get; set; }

        [Display(Name = "OrganizationalNumber")]
        [Required(ErrorMessage = "MustSubmitOrganizationalNumber")]
        [OrgNumber(ErrorMessage = "InvalidOrganizationNumber")]
        public string InvoiceOrgnr { get; set; }

        [Required(ErrorMessage = "MustSubmitZipCode")]
        [Display(Name = "ZipCode")]
        public string Zip { get; set; }
    }
}
