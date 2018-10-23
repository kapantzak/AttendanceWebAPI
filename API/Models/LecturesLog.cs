using System;
using System.Collections.Generic;

namespace AttendanceWebApi.Models
{
    public partial class LecturesLog
    {
        public LecturesLog()
        {
            AttendanceLog = new HashSet<AttendanceLog>();
        }

        public int Id { get; set; }
        public int CourseAssignmentId { get; set; }
        public DateTime Date { get; set; }

        public CoursesAssignments CourseAssignment { get; set; }
        public ICollection<AttendanceLog> AttendanceLog { get; set; }
    }
}
