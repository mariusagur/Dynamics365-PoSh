using Dynamics365_PoSh.Helpers;
using Dynamics365_PoSh.Models;
using Microsoft.Xrm.Sdk;
using System.Management.Automation;

namespace Dynamics365_PoSh.EmailActions
{
    [Cmdlet(VerbsCommon.Get, "DynamicsMailbox", SupportsShouldProcess = true, DefaultParameterSetName = "oauth")]
    public class GetDynamicsMailbox : PSCmdlet
    {
        [Parameter(HelpMessage = "Only show unapproved mailboxes")]
        public SwitchParameter UnapprovedOnly;
        [Parameter(HelpMessage = "Only show mailboxes with failed tests")]
        public SwitchParameter FailedOnly;
        [Parameter(Mandatory = true)]
        public string EmailAddress = "*";
        [Parameter]
        public IOrganizationService Connection;

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            if (Connection == null)
            {
                var serviceObject = SessionState.PSVariable.Get(SessionVariableFactory.DataConnection);
                if (serviceObject == null)
                {
                    Host.UI.WriteLine(@"Please use Connect-DynamicsService first or pass in existing connection");
                    return;
                }
                else
                {
                    Connection = serviceObject.Value as IOrganizationService;
                }
            }
            var syncStatus = DynamicsMailBox.SynchronizationStatus.Any;
            if (UnapprovedOnly)
            {
                syncStatus = DynamicsMailBox.SynchronizationStatus.UnapprovedOnly;
            }
            if (FailedOnly)
            {
                syncStatus = DynamicsMailBox.SynchronizationStatus.FailedOnly;
            }

            var mailboxes = DynamicsMailBox.GetXrmMailBoxes(Connection, EmailAddress, syncStatus);
            WriteObject(mailboxes);
        }
    }
}
