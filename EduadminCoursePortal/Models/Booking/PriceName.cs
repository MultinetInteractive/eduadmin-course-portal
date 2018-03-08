using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EduadminCoursePortal.Models.Booking
{
    public class PriceName
    {
        public int PriceNameId { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
    }
}
