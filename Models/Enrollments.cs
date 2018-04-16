using System;
using System.Collections.Generic;

namespace AttendanceWebApi.Models
{
    public partial class Enrollments
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public int AcademicTermId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Notes { get; set; }

        public AcademicTerms AcademicTerm { get; set; }
        public Courses Course { get; set; }
        public Users Student { get; set; }
    }
}
