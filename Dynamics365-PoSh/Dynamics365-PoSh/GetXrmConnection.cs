using Dynamics365_PoSh.Helpers;
using System.Management.Automation;

namespace Dynamics365_PoSh
{
    [Cmdlet(VerbsCommon.Get, "XrmConnection", SupportsShouldProcess = true)]
    public class GetXrmConnection : PSCmdlet
    {
        [Parameter(Mandatory = true)]
        public string ServerUrl;

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            ServerUrl.TrimEnd('/');
            var webAuthCookies = WebAuthentication.GetAuthenticatedCookies(ServerUrl, Models.AuthenticationType.O365);
            var connection = CrmConnection.GetConnection(ServerUrl, webAuthCookies);

            SessionState.PSVariable.Set("AuthCookies", webAuthCookies);
            SessionState.PSVariable.Set("XrmService", connection);

            WriteObject(connection);
        }
    }
}
