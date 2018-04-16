using System;
using System.Collections.Generic;

namespace AttendanceWebApi.Models
{
    public partial class CoursesAssignments
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int ProfessorId { get; set; }
        public int AcademicTermId { get; set; }
        public string Notes { get; set; }
        public int? ClassRoomId { get; set; }

        public AcademicTerms AcademicTerm { get; set; }
        public ClassRooms ClassRoom { get; set; }
        public Courses Course { get; set; }
        public Users Professor { get; set; }
    }
}
