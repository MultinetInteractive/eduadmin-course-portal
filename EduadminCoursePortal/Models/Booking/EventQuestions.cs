using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EduadminCoursePortal.Models.Booking
{
    public class EventQuestions
    {
        public List<Question> BookingQuestions { get; set; }
        public List<Question> ParticipantQuestions { get; set; }
    }
}
