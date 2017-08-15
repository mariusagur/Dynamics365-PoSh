using Dynamics365_PoSh.Helpers;
using Dynamics365_PoSh.Models;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;
using System.Management.Automation;

namespace Dynamics365_PoSh.EmailActions
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
        public XrmMailBox Mailbox;

        [Parameter(Mandatory = true, ParameterSetName = "SetByEmailAddress")]
        [Parameter(HelpMessage = "Unique email address for mailbox")]
        public string EmailAddress;

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            if (Service == null)
            {
                if (SessionState.PSVariable.Get("XrmService") != null)
                {
                    Service = SessionState.PSVariable.Get("XrmService") as IOrganizationService;
                }
                else
                {
                    while (string.IsNullOrWhiteSpace(ServerUrl) && !Uri.TryCreate(ServerUrl, UriKind.Absolute, out Uri validationUri))
                    {
                        Host.UI.WriteLine(@"Please specify a valid url for your Dynamics365 environment");
                        ServerUrl = Host.UI.ReadLine();
                    }
                    var webCookies = WebAuthentication.GetAuthenticatedCookies(ServerUrl, Models.AuthenticationType.O365);
                    Service = CrmConnection.GetConnection(ServerUrl, webCookies);
                }
            }

            if (Mailbox == null)
            {
                while (string.IsNullOrWhiteSpace(EmailAddress))
                {
                    Host.UI.WriteLine("Please enter valid email address");
                    EmailAddress = Host.UI.ReadLine();
                }

                Mailbox = new XrmMailBox
                {
                    EmailAddress = EmailAddress
                };
                var result = Mailbox.GetXrmMailBox(Service);
                if (result == ResultCodes.OK)
                {
                    Mailbox.MailBox.Attributes["testemailconfigurationscheduled"] = true;
                    Service.Update(Mailbox.MailBox);
                }
                else if (result == ResultCodes.MultipleResults)
                {
                    Host.UI.WriteLine($"More than 1 mailbox found for email address: {EmailAddress}");
                }
                else
                {
                    Host.UI.WriteLine($"No results found for email address: {EmailAddress}");
                }
            }
        }

    }
}
