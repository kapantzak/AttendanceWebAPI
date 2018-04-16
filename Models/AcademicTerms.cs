using System;
using System.Collections.Generic;

namespace AttendanceWebApi.Models
{
    public partial class AcademicTerms
    {
        public AcademicTerms()
        {
            AttendanceLog = new HashSet<AttendanceLog>();
            CoursesAssignments = new HashSet<CoursesAssignments>();
            Enrollments = new HashSet<Enrollments>();
        }

        public int Id { get; set; }
        public int TypeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Descr { get; set; }

        public AcademicTermTypes Type { get; set; }
        public ICollection<AttendanceLog> AttendanceLog { get; set; }
        public ICollection<CoursesAssignments> CoursesAssignments { get; set; }
        public ICollection<Enrollments> Enrollments { get; set; }
    }
}
