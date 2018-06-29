using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EduadminCoursePortal.Resources;

namespace EduadminCoursePortal.Models.Booking
{
    public class CustomerContact
    {
        public int CustomerContactID { get; set; }
        [Required(ErrorMessage = "MustSubmitFirstName")]
        [Display(Name = "FirstName")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "MustSubmitLastName")]
        [Display(Name = "LastName")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "MustSubmitEmail")]
        [EmailAddress(ErrorMessage = "MustSubmitValidEmail")]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
