using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EduadminCoursePortal.Models.Booking
{
    public class QuestionAnswer
    {
        public int AnswerId { get; set; }
        public int QuestionTypeID { get; set; }
        public string AnswerTime { get; set; }
        public string AnswerNumber { get; set; }
        public string Answer { get; set; }
        [Required]
        public int QuestionID { get; set; }
        public bool Checked { get; set; }
        public List<Alternatives> Alternatives { get; set; }
    }

    public class Alternatives
    {
        public bool Checked { get; set; }
        public int AnswerId { get; set; }
    }
}
