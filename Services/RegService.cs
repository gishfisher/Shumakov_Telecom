using Shumakov_Telecom.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using TelecomCompany.ApplicationData.Crypt;

namespace Shumakov_Telecom.Services
{
    internal class RegService
    {
        private TelecomServiceDeskEntities _db => TelecomServiceDeskEntities.GetContext();

        public bool RegisterEmployee(Employee employee, string password)
        {
            if (employee == null) 
                return false;

            employee.User.Password_Hash = 
                MD5Hasher.HashPassword(password);

            _db.Employees.Add(employee);
            _db.SaveChanges();

            return true;
        }

        public bool EditEmployee(Employee employee, string newPassword)
        {
            if (employee == null) 
                return false;

            if (!string.IsNullOrWhiteSpace(newPassword))
            {
                employee.User.Password_Hash =
                    MD5Hasher.HashPassword(newPassword);
            }

            _db.SaveChanges();
            return true;
        }

        public bool DeleteEmployee(Employee employee)
        {
            if (employee == null)
                return false;

            try
            {
                if (employee.User != null)
                {
                    _db.Users.Remove(employee.User);
                }

                _db.Employees.Remove(employee);
                _db.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
