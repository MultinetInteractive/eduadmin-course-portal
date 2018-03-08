using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EduadminCoursePortal.Models
{
    public class EventCity
    {
        public string City { get; set; }
        public List<EventModel> Events { get; set; }
    }
}
