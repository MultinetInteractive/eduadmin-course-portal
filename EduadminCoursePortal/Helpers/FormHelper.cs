using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EduadminCoursePortal.Helpers
{
    public sealed class FormHelper
    {
        #region DateStrings

        public static string GetShortDateString(DateTime? start, DateTime? end)
        {
            return start?.ToString("dddd dd'/'MM") + " och " + end?.ToString("dddd dd'/'MM");
        }

        public static string GetHtmlDateString(DateTime? start, DateTime? end)
        {
            if (start?.Date == end?.Date)
            {
                return start?.Date == end?.Date ? start?.ToString("dddd dd'/'MM HH:mm") + " till " + end?.ToString("HH:mm") : start?.ToString("ddd dd'/'MM HH:mm") + " till " + end?.ToString("ddd dd'/'MM HH:mm");
            }
            else
            {
                return start?.Date == end?.Date ? start?.ToString("dddd dd'/'MM HH:mm") + " till " + "<br/>" + end?.ToString("HH:mm") : start?.ToString("ddd dd'/'MM HH:mm") + " till " + "<br/>" + end?.ToString("ddd dd'/'MM HH:mm");
            }
        }

        public static string GetDateString(DateTime? start, DateTime? end)
        {
            return start?.Date == end?.Date ? start?.ToString("dddd dd'/'MM HH:mm") + " - " + end?.ToString("HH:mm") : start?.ToString("ddd dd'/'MM HH:mm") + " - " + end?.ToString("ddd dd'/'MM HH:mm");
        }

        #endregion
    }
}
