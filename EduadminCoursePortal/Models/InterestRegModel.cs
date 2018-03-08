using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EduadminCoursePortal.Resources;

namespace EduadminCoursePortal.Models
{
    public class InterestRegModel
    {
        [Display(Name = "CompanyName")]
        [Required(ErrorMessage = "CompanyNameMissing")]
        public string CompanyName { get; set; }

        [Display(Name = "FirstName")]
        [Required(ErrorMessage = "MustSubmitFirstName")]
        public string FirstName { get; set; }

        [Display(Name = "LastName")]
        [Required(ErrorMessage = "MustSubmitLastName")]
        public string LastName { get; set; }

        [Display(Name = "Mobile")]
        [Required(ErrorMessage = "MobileMissing")]
        [Phone(ErrorMessage = "PhoneNumberWrongFormat")]
        public string Mobile { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "EmailMissing")]
        [EmailAddress(ErrorMessage = "EmailWrong")]
        public string Email { get; set; }

        [Display(Name = "NumberParticipants")]
        [Required(ErrorMessage = "AtLeastOneParticipant")]
        [Range(0, int.MaxValue, ErrorMessage = "ParticipantsMustBePositive")]
        public int PartNr { get; set; }

        [Display(Name = "Notes")]
        public string Notes { get; set; }

        public int CourseTemplateId { get; set; }
    }
}
