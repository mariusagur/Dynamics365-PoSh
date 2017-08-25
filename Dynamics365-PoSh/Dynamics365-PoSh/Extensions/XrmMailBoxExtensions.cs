using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Linq;

namespace Dynamics365_PoSh.Models
{
    public static partial class XrmMailBoxExtensions
    {
        public static void ApproveMailBox(this XrmMailBox mailBox, IOrganizationService service)
        {
            mailBox.MailBox.Attributes["testemailconfigurationscheduled"] = true;
            service.Update(mailBox.MailBox);
        }

        public static ResultCodes GetXrmMailBox(this XrmMailBox mailBox, IOrganizationService service, bool unapprovedOnly = false, bool failedOnly = false)
        {
            var qe = new QueryExpression("mailbox")
            {
                ColumnSet = new ColumnSet(true)
            };

            qe.Criteria.AddCondition(new ConditionExpression("emailaddress", ConditionOperator.Equal, mailBox.EmailAddress));
            if (unapprovedOnly)
            {
                qe.Criteria.AddCondition(new ConditionExpression("isemailaddressapprovedbyo365admin", ConditionOperator.Equal, true));
            }
            if (failedOnly)
            {
                var statusFilter = new FilterExpression(LogicalOperator.Or);
                statusFilter.AddCondition(new ConditionExpression("incomingemailstatus", ConditionOperator.Equal, EmailStatus.Failure));
                statusFilter.AddCondition(new ConditionExpression("outgoingemailstatus", ConditionOperator.Equal, EmailStatus.Failure));
                statusFilter.AddCondition(new ConditionExpression("actstatus", ConditionOperator.Equal, EmailStatus.Failure));
                qe.Criteria.AddFilter(statusFilter);
            }

            var result = service.RetrieveMultiple(qe);
            if (result.Entities.Count == 1)
            {
                mailBox.EntityType = result.Entities.First().LogicalName;
                mailBox.MailBox = result.Entities.First();
                return ResultCodes.OK;
            }
            else if (result.Entities.Count > 1)
            {
                return ResultCodes.MultipleResults;
            }
            else
            {
                return ResultCodes.NoResults;
            }
        }
    }
}
