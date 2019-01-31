using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduadminCoursePortal.Models.Booking
{
    public class BookModel
    {
        [Required]
        public int EventId { get; set; }
        public int CourseTemplateId { get; set; }
        public string EventName { get; set; }
        public List<Participant> Participants { get; set; }
        public Customer Customer { get; set; }
        public CustomerContact CustomerContact { get; set; }
        public List<Question> BookingQuestions { get; set; }
        public bool FullyBooked { get; set; }
        public string Description { get; set; }
        public string BackUrl { get; set; }
        public bool RequireCivicRegistrationNumber { get; set; }
        public bool HasError { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? LastApplicationDate { get; set; }
        public BookModel()
        {
            Participants = new List<Participant>();
            Customer = new Customer();
            CustomerContact = new CustomerContact();
        }
    }
}
