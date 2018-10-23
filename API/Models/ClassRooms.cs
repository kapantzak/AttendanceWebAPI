using System;
using System.Collections.Generic;

namespace AttendanceWebApi.Models
{
    public partial class ClassRooms
    {
        public ClassRooms()
        {
            CoursesAssignments = new HashSet<CoursesAssignments>();
        }

        public int Id { get; set; }
        public string Descr { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public double? RangeInMeters { get; set; }

        public ICollection<CoursesAssignments> CoursesAssignments { get; set; }
    }
}
