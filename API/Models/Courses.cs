using System;
using System.Collections.Generic;

namespace AttendanceWebApi.Models
{
    public partial class Courses
    {
        public Courses()
        {
            AttendanceLog = new HashSet<AttendanceLog>();
            CoursesAssignments = new HashSet<CoursesAssignments>();
            Enrollments = new HashSet<Enrollments>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Descr { get; set; }
        public bool? IsActive { get; set; }

        public ICollection<AttendanceLog> AttendanceLog { get; set; }
        public ICollection<CoursesAssignments> CoursesAssignments { get; set; }
        public ICollection<Enrollments> Enrollments { get; set; }
    }
}
