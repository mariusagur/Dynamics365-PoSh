using Dynamics365_PoSh.Helpers;
using Dynamics365_PoSh.Models;
using Microsoft.Xrm.Sdk;
using System;
using System.Management.Automation;

namespace Dynamics365_PoSh.EmailActions
{
    [Cmdlet(VerbsCommon.Set, "DynamicsMailboxApproval", SupportsShouldProcess = true)]
    public class SetDynamicsMailboxApproval : PSCmdlet
    {
        [Parameter(HelpMessage = "Existing connection of type IOrganizationService, see Get-XrmConnection")]
        public IOrganizationService Connection;

        [Parameter(Mandatory = true, ParameterSetName = "SetByEntity", ValueFromPipeline = true)]
        [Parameter(HelpMessage = "Entity repesentation of mailbox")]
        public XrmMailBox Mailbox;

        public string EmailAddress;

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

            Mailbox.MailBox.Attributes["testemailconfigurationscheduled"] = true;
            Connection.Update(Mailbox.MailBox);
        }
    }
}
