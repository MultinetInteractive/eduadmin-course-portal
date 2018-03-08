using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EduadminCoursePortal.Models.Booking
{
    public class CreateBookingResult
    {
        public bool Success { get; set; }
        public Dictionary<string, List<string>> Errors { get; set; }
        public string SuccessRedirectUrl { get; set; }
    }
}
