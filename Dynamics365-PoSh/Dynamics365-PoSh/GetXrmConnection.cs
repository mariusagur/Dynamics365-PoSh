using Dynamics365_PoSh.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

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
            WriteObject(CrmConnection.GetConnection(ServerUrl, webAuthCookies));
        }
    }
}
