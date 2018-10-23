using System;
using System.Collections.Generic;

namespace AttendanceWebApi.Models
{
    public partial class Roles
    {
        public Roles()
        {
            UserRoles = new HashSet<UserRoles>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public ICollection<UserRoles> UserRoles { get; set; }
    }
}
