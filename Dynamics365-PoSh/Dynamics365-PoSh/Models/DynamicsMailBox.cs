using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using System.Linq;

namespace Dynamics365_PoSh.Models
{
    public class DynamicsMailBox
    {
        public string EmailAddress { get; set; }
        public string EntityType { get; set; }
        public Entity MailBox { get; set; }
        public enum SynchronizationStatus
        {
            Any,
            UnapprovedOnly,
            FailedOnly
        }
        public static List<DynamicsMailBox> GetXrmMailBoxes(IOrganizationService service, string emailAddress, SynchronizationStatus filter = SynchronizationStatus.Any)
        {
            var qe = new QueryExpression("mailbox")
            {
                ColumnSet = new ColumnSet(true),
                TopCount = 1
            };
            if (filter == SynchronizationStatus.UnapprovedOnly)
            {
                qe.Criteria.AddCondition(new ConditionExpression("isemailaddressapprovedbyo365admin", ConditionOperator.Equal, false));
            }
            if (filter == SynchronizationStatus.FailedOnly)
            {
                var statusFilter = new FilterExpression(LogicalOperator.Or);
                statusFilter.AddCondition(new ConditionExpression("incomingemailstatus", ConditionOperator.Equal, EmailStatus.Failure));
                statusFilter.AddCondition(new ConditionExpression("outgoingemailstatus", ConditionOperator.Equal, EmailStatus.Failure));
                statusFilter.AddCondition(new ConditionExpression("actstatus", ConditionOperator.Equal, EmailStatus.Failure));
                qe.Criteria.AddFilter(statusFilter);
            }

            var result = service.RetrieveMultiple(qe);

            return result.Entities.Select(e => new DynamicsMailBox
            {
                EmailAddress = e.Attributes["emailaddress"].ToString(),
                MailBox = e,
                EntityType = e.LogicalName
            }).ToList();
        }    
    }
}
