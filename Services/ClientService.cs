using Shumakov_Telecom.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelecomCompany.ApplicationData.Crypt;

namespace Shumakov_Telecom.Services
{
    internal class ClientService
    {
        private TelecomServiceDeskEntities _db => TelecomServiceDeskEntities.GetContext();

        public bool AddClient(Client client)
        {
            if (client == null) return false;

            _db.Clients.Add(client);
            _db.SaveChanges();

            return true;
        }

        public bool EditClient(Client client)
        {
            if (client == null) return false;

            _db.SaveChanges();

            return true;
        }

        public bool RemoveClient(Client client)
        {
            if (client == null) return false;

            _db.Clients.Remove(client);
            _db.SaveChanges();

            return true;
        }
    }
}
