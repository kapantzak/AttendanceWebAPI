using System;
using System.Collections.Generic;

namespace AttendanceWebApi.Models
{
    public partial class UserRoles
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }

        public Roles Role { get; set; }
        public Users User { get; set; }
    }
}
