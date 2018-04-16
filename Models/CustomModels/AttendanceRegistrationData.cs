using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceWebApi.Models.CustomModels
{
    public class AttendanceRegistrationData
    {
        /// <summary>
        /// AttendanceLog data to insert
        /// </summary>
        public AttendanceLog Attendance { get; set; }
        /// <summary>
        /// Course assignment id that the user wants to register attendance
        /// </summary>
        public int? CoursesAssignmentID { get; set; }
        /// <summary>
        /// User's Longitude
        /// </summary>
        public double GeoLon { get; set; }
        /// <summary>
        /// User's Latitude
        /// </summary>
        public double GeoLat { get; set; }
        /// <summary>
        /// The timestamp that the user scanned the QR Code 
        /// </summary>
        public long Timestamp { get; set; }
        /// <summary>
        /// The timestamp that the QR Code was generated
        /// </summary>
        public long SessionStartTimestamp { get; set; }
    }
}
