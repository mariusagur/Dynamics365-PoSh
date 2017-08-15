using Dynamics365_PoSh.Helpers;
using Dynamics365_PoSh.Models;
using Microsoft.Xrm.Sdk;
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
        public SwitchParameter UnapprovedOnly;
        [Parameter(HelpMessage = "Only show mailboxes with failed tests")]
        public SwitchParameter FailedOnly;

        [Parameter]
        public string EmailAddress = "*";

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            var auth = SessionState.PSVariable.Get("authenticated_connections").Value as Authentication;
            if (auth == null)
            {

            }
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
                    SessionState.PSVariable.Set("CrmServerUrl", ServerUrl);
                    var webCookies = WebAuthentication.GetAuthenticatedCookies(ServerUrl, Models.AuthenticationType.O365);
                    Service = CrmConnection.GetConnection(ServerUrl, webCookies);
                }
            }

            var syncStatus = XrmMailBox.SynchronizationStatus.Any;
            if (UnapprovedOnly)
            {
                syncStatus = XrmMailBox.SynchronizationStatus.UnapprovedOnly;
            }
            if (FailedOnly)
            {
                syncStatus = XrmMailBox.SynchronizationStatus.FailedOnly;
            }

            var mailboxes = XrmMailBox.GetXrmMailBoxes(Service, EmailAddress, syncStatus);
            WriteObject(mailboxes);
        }
    }
}
