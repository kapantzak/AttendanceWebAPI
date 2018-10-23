using System;
using System.Collections.Generic;

namespace AttendanceWebApi.Models
{
    public partial class AttendanceLog
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public int AcademicTermId { get; set; }
        public DateTime Date { get; set; }
        public int AttendanceTypeId { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public long? Timestamp { get; set; }
        public int LecturesLogId { get; set; }

        public AcademicTerms AcademicTerm { get; set; }
        public AttendanceTypes AttendanceType { get; set; }
        public Courses Course { get; set; }
        public LecturesLog LecturesLog { get; set; }
        public Users Student { get; set; }
    }
}
