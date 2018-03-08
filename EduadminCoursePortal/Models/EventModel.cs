using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EduadminCoursePortal.Models
{
    public class EventModel
    {
        public string AddressName { get; set; }
        public string City { get; set; }
        public List<EventDates> EventDates
        { get; set; }
        public int EventId { get; set; }
        public bool IsFullyBooked { get; set; }
        public DateTime? LastApplicationDate { get; set; }
        public int? MaxParticipantNr { get; set; }
        public int CourseTemplateId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int StatusId { get; set; }
        public string StatusText { get; set; }
        public int TotalParticipantNr { get; set; }
        public int? ParticipantNrLeft { get; set; }
    }
}
