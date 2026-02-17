using Shumakov_Telecom.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Shumakov_DigitalStore.ApplicationData
{
    public class TempStorageService
    {
        private List<TempServiceState> _items = new List<TempServiceState>();

        public List<TempServiceState> Items => _items;

        public bool Add(Service service)
        {
            if (_items.Any(i => i.Service.ServiceId == service.ServiceId))
                return false;

            _items.Add(new TempServiceState(service));
            return true;
        }

        public bool Remove(TempServiceState item)
        {
            return _items.Remove(item);
        }

        public void Clear()
        {
            _items.Clear();
        }
        public decimal GetTotalAmount()
        {
            return _items.Sum(x => x.Price);
        }
    }
}
