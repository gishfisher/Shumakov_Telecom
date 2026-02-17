using Shumakov_DigitalStore.ApplicationData;
using Shumakov_Telecom.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telecom.Services
{
    public class RequestService
    {
        private readonly TelecomServiceDeskEntities _db;

        public RequestService()
        {
            _db = TelecomServiceDeskEntities.GetContext();
        }

        public void SaveRequest(Request request, List<Service> services)
        {
            if (request.RequestId == 0)
            {
                request.CreatedAt = DateTime.Now;

                if (request.Employee != null)
                    request.StatusId = (int)RequestStatusType.Assigned;
                else
                    request.StatusId = (int)RequestStatusType.New;

                _db.Requests.Add(request);
            }

            request.Services.Clear();

            foreach (var service in services)
            {
                request.Services.Add(service);
            }

            request.TotalAmount = services.Sum(x => x.Price);

            _db.SaveChanges();
        }

        public void EditRequest(Request request, List<Service> services)
        {
            request.ClientId = request.ClientId;
            request.PriorityId = request.PriorityId;
            request.RequestTypeId = request.RequestTypeId;
            request.Description = request.Description;

            request.Services.Clear();

            foreach (var service in services)
            {
                if (service != null)
                    request.Services.Add(service);
            }

            request.TotalAmount = services.Sum(x => x.Price);

            _db.SaveChanges();
        }

        public void DeleteRequest(Request request)
        {
            _db.Requests.Remove(request);
            _db.SaveChanges();
        }

        public bool AssignRequest(Request request, Employee employee)
        {
            if (!CanChangeStatus(request.StatusId, (int)RequestStatusType.Assigned))
                return false;

            request.StatusId = (int)RequestStatusType.Assigned;
            request.Employee = employee;

            _db.SaveChanges();

            return true;
        }

        public bool CancelRequest(Request request)
        {
            if (!CanChangeStatus(request.StatusId, (int)RequestStatusType.Cancelled))
                return false;

            request.StatusId = (int)RequestStatusType.Cancelled;
            request.Employee = null;

            _db.SaveChanges();

            return true;
        }

        public bool TryChangeStatus(Request request, int newStatusId)
        {
            if (!CanChangeStatus(request.StatusId, newStatusId))
                return false;

            request.StatusId = newStatusId;
            _db.SaveChanges();
            return true;
        }

        private bool CanChangeStatus(int currentStatusId, int newStatusId)
        {
            switch (currentStatusId)
            {
                case (int)RequestStatusType.New:
                    return newStatusId == (int)RequestStatusType.Assigned
                        || newStatusId == (int)RequestStatusType.Cancelled;
                case (int)RequestStatusType.Assigned:
                    return newStatusId == (int)RequestStatusType.InProgress
                        || newStatusId == (int)RequestStatusType.New
                        || newStatusId == (int)RequestStatusType.Cancelled;
                case (int)RequestStatusType.InProgress:
                    return newStatusId == (int)RequestStatusType.Completed
                        || newStatusId == (int)RequestStatusType.Cancelled;
                case (int)RequestStatusType.Completed:
                    return newStatusId == (int)RequestStatusType.Closed;
                case (int)RequestStatusType.Cancelled:
                    return newStatusId == (int)RequestStatusType.Closed
                        || newStatusId == (int)RequestStatusType.Assigned;
                case (int)RequestStatusType.Closed:
                    return false;
                default:
                    return false;
            }
        }
    }
}
