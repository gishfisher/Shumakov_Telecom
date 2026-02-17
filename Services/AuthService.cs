using Shumakov_Telecom.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telecom.Services
{
    internal class AuthService
    {
        private TelecomServiceDeskEntities _db => TelecomServiceDeskEntities.GetContext();
        
        public User Authenticate(string login, string passwordHash)
        {
            return _db.Users.FirstOrDefault(u => u.Login == login && u.Password_Hash == passwordHash);
        }
    }
}
