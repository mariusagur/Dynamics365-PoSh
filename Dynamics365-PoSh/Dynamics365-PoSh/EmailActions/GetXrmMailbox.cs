using Dynamics365_PoSh.Helpers;
using Dynamics365_PoSh.Models;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Management.Automation;

namespace Dynamics365_PoSh.EmailActions
{
    [Cmdlet(VerbsCommon.Get, "XrmMailbox", SupportsShouldProcess = true)]
    public class GetXrmMailbox : PSCmdlet
    {
        [Parameter(HelpMessage = "Existing connection of type IOrganizationService, see Get-XrmConnection")]
        public IOrganizationService Service;

        [Parameter(HelpMessage = "Server url to use for a one-off connection")]
        public string ServerUrl;

        [Parameter(HelpMessage = "Only show unapproved mailboxes")]
        public bool UnapprovedOnly = false;

        [Parameter(HelpMessage = "Only show mailboxes with failed tests")]
        public bool FailedOnly = false;

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            if (Service == null)
            {
                while (string.IsNullOrWhiteSpace(ServerUrl) && !Uri.TryCreate(ServerUrl, UriKind.Absolute, out Uri validationUri))
                {
                    Host.UI.WriteLine(@"Please specify a valid url for your Dynamics365 environment");
                    ServerUrl = Host.UI.ReadLine();
                }
                var webCookies = WebAuthentication.GetAuthenticatedCookies(ServerUrl, Models.AuthenticationType.O365);
                Service = CrmConnection.GetConnection(ServerUrl, webCookies);
            }

            var qe = new QueryExpression("mailbox")
            {
                ColumnSet = new ColumnSet(true),
                TopCount = 1
            };
            if (UnapprovedOnly)
            {
                qe.Criteria.AddCondition(new ConditionExpression("isemailaddressapprovedbyo365admin", ConditionOperator.Equal, true));
            }
            if (FailedOnly)
            {
                var statusFilter = new FilterExpression(LogicalOperator.Or);
                statusFilter.AddCondition(new ConditionExpression("incomingemailstatus", ConditionOperator.Equal, EmailStatus.Failure));
                statusFilter.AddCondition(new ConditionExpression("outgoingemailstatus", ConditionOperator.Equal, EmailStatus.Failure));
                statusFilter.AddCondition(new ConditionExpression("actstatus", ConditionOperator.Equal, EmailStatus.Failure));
                qe.Criteria.AddFilter(statusFilter);
            }

            var result = Service.RetrieveMultiple(qe);
            WriteObject(result.Entities);
        }
    }
}
