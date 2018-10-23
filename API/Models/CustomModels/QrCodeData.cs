using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceWebApi.Models
{
    public class QrCodeData
    {
        public int? CourseId { get; set; }
        public int? AcademicTermId { get; set; }
        public int? CourseAssignmentId { get; set; }
        public int? LectureLogId { get; set; }
        public string Date { get; set; }
        public long? SessionStartTimestamp { get; set; }
    }
}
