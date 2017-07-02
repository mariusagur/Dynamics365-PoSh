using Dynamics365_PoSh.Helpers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;
using System.Management.Automation;

namespace Dynamics365_PoSh.MailboxActions
{
    [Cmdlet(VerbsCommon.Set, "XrmMailboxApproval", SupportsShouldProcess = true)]
    public class SetXrmMailboxApproval : PSCmdlet
    {
        [Parameter(HelpMessage = "Existing connection of type IOrganizationService, see Get-XrmConnection")]
        public IOrganizationService Service;

        [Parameter(HelpMessage = "Server url to use for a one-off connection")]
        public string ServerUrl;

        [Parameter(Mandatory = true, ParameterSetName = "SetByEntity", ValueFromPipeline = true)]
        [Parameter(HelpMessage = "Entity repesentation of mailbox")]
        public Entity Mailbox;

        [Parameter(Mandatory = true, ParameterSetName = "SetByEmailAddress")]
        [Parameter(HelpMessage = "Unique email address for mailbox")]
        public string EmailAddress;

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

            if (Mailbox == null)
            {
                while (string.IsNullOrWhiteSpace(EmailAddress))
                {
                    Host.UI.WriteLine("Please enter valid email address");
                    EmailAddress = Host.UI.ReadLine();
                }
                var qe = new QueryExpression("mailbox")
                {
                    ColumnSet = new ColumnSet(true)
                };

                qe.Criteria.AddCondition(new ConditionExpression("emailaddress", ConditionOperator.Equal, EmailAddress));
                var result = Service.RetrieveMultiple(qe);

                if (result.Entities.Count() == 1)
                {
                    Mailbox = result.Entities.First();
                }
                else if (result.Entities.Count == 0)
                {
                    Host.UI.WriteLine($"No mailbox found with emailaddress: {EmailAddress}");
                }
                else
                {
                    Host.UI.WriteLine($"More than 1 mailbox found for emailaddress: {EmailAddress}");
                }
            }
            Mailbox.Attributes["testemailconfigurationscheduled"] = true;
            Service.Update(Mailbox);
        }

    }
}
