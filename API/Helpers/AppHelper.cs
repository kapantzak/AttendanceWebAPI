using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AttendanceWebApi.Models;

namespace AttendanceWebApi.Helpers
{
    public static class AppHelper
    {

        /// <summary>
        /// Get specific user's roles
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static List<int> GetUserRoles(int userId, attWebApiContext context) => (from u in context.Users
                                                                                       join ur in context.UserRoles on u.Id equals ur.UserId
                                                                                       where u.Id == userId
                                                                                       select ur.RoleId).ToList();

    }
}
