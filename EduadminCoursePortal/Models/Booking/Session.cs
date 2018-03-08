using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EduadminCoursePortal.Models.Booking
{
    public class Session
    {
        public int SessionId { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Price { get; set; }
        public int PriceNameId { get; set; }
        public bool Participating { get; set; }
    }
}
