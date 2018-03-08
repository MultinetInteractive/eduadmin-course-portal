using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using EduadminCoursePortal.Attributes;
using EduadminCoursePortal.Resources;
using EduadminCoursePortal.Models.Booking;

namespace EduadminCoursePortal.Models
{
    public class Participant
    {
        public int PersonID { get; set; }

        [Display(Name = "FirstName")]
        [Required(ErrorMessage = "MustSubmitFirstName")]
        public string FirstName { get; set; }

        [Display(Name = "LastName")]
        [Required(ErrorMessage = "MustSubmitLastName")]
        public string LastName { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "MustSubmitEmail")]
        [EmailAddress(ErrorMessage = "MustSubmitValidEmail")]
        public string Email { get; set; }

        [Required(ErrorMessage = "MustSelectPrice")]
        public int PriceNameId { get; set; }

        public List<PriceName> PriceNames { get; set; }

        public List<Session> Sessions { get; set; }

        [Display(Name = "CustomerContact")]
        public bool IsContactPerson { get; set; }

        [CivRegNumber(ErrorMessage = "CivicNumberError")]
        [RequiredIf("RequireCivicRegistrationNumber", true, ErrorMessage = "MustSubmitCivRegNumber")]
        [Display(Name = "CivRegNumber", Prompt = "yyyymmdd-xxxx")]
        [DataMask("99999999-9999")]
        public string CivicRegistrationNumber { get; set; }

        public bool RequireCivicRegistrationNumber { get; set; }

        public List<Question> ParticipantQuestions { get; set; }
    }
}
