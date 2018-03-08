using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EduadminCoursePortal.Models
{
    public class CourseTemplatesModel
    {
        public List<CourseTemplateWithEvents> CourseTemplatesWithEvents { get; set; }
        public List<Subject> Subjects { get; set; }
        public List<Category> Categories { get; set; }
    }
}
