using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EduadminCoursePortal.Models
{
    public class CourseTemplateModel
    {
        public string CourseName { get; set; }
        public string InternalCourseName { get; set; }
        public string ImageUrl { get; set; }
        public string CourseDescriptionShort { get; set; }
        public string CourseDescription { get; set; }
        public string TargetGroup { get; set; }
        public string CourseGoal { get; set; }
        public int CourseTemplateId { get; set; }
        public List<EventCity> EventCities { get; set; }
    }
}
