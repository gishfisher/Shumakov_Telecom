using Shumakov_Telecom.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shumakov_DigitalStore.ApplicationData
{
    public class TempServiceState
    {
        public Service Service { get; set; }

        public string Name => Service.Name;
        public decimal Price => Service.Price;

        public TempServiceState(Service service)
        {
            Service = service;
        }
    }
}