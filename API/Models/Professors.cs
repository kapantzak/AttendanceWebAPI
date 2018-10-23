using System;
using System.Collections.Generic;

namespace AttendanceWebApi.Models
{
    public partial class Professors
    {
        public Professors()
        {
            CoursesAssignments = new HashSet<CoursesAssignments>();
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int UserId { get; set; }

        public Users User { get; set; }
        public ICollection<CoursesAssignments> CoursesAssignments { get; set; }
    }
}
