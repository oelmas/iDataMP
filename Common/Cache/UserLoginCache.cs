using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Cache
{
    public static class UserLoginCache
    {
        public static int UserId { get; set; }
        public static string UserName { get; set; }
        public static string UserLastName { get; set; }
        public static string UserPosition { get; set; }
        public static int UserTypeId { get; set; }
        public static string UserEmail { get; set; }
    }
}
