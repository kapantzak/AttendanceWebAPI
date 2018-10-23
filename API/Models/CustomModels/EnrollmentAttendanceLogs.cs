using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceWebApi.Models.CustomModels
{
    public class EnrollmentAttendanceLogs
    {
        public List<AttendanceLog> Logs { get; set; }
        public string CourseTitle { get; set; }
        public int? LecturesMinNum { get; set; }
        public int? LecturesTargetNum { get; set; }
        public int? LecturesActualNum { get; set; }
        public DateTime? Date { get; set; }
    }
}
