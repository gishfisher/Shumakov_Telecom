using Shumakov_Telecom.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telecom.Services
{
    internal class UserSessionService
    {
        public static User CurrentUser { get; private set; }

        public static bool IsAuthenticated => CurrentUser != null;
        public static bool IsAdmin => CurrentUser.Role.RoleId == 1;

        public static void Logout()
        {
            CurrentUser = null;
        }

        public static void Login(User user)
        {
            CurrentUser = user;
        }
    }
}
