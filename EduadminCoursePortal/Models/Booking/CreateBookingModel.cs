using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EduadminCoursePortal.Models.Booking
{
    public class CreateBookingModel
    {
        [Required]
        public int EventId { get; set; }
        [Required]
        public DateTime? LastApplicationDate { get; set; }
        [Required]
        public DateTime? StartDate { get; set; }
        [Required]
        public List<CreateParticipant> Participants { get; set; }
        [Required]
        public Customer Customer { get; set; }
        [Required]
        public CustomerContact CustomerContact { get; set; }
        public List<QuestionAnswer> BookingQuestions { get; set; }
    }
}
