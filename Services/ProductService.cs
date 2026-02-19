using Shumakov_Telecom.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelecomCompany.ApplicationData.Crypt;

namespace Shumakov_Telecom.Services
{
    internal class ProductService
    {
        private TelecomServiceDeskEntities _db => TelecomServiceDeskEntities.GetContext();

        public bool AddService(Service service)
        {
            if (service == null) return false;

            _db.Services.Add(service);
            _db.SaveChanges();

            return true;
        }

        public bool EditService(Service service)
        {
            if (service == null) return false;

            _db.SaveChanges();

            return true;
        }

        public bool RemoveService(Service service)
        {
            if (service == null) return false;

            _db.Services.Remove(service);
            _db.SaveChanges();

            return true;
        }
    }
}
