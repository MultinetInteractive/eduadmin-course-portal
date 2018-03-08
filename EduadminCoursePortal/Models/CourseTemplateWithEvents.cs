using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EduadminCoursePortal.Models
{
    public class CourseTemplateWithEvents
    {
        public int CourseTemplateId { get; set; }
        public string ObjectName { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string CourseDescriptionShort { get; set; }
        public List<EventModel> Events { get; set; }
    }
}
