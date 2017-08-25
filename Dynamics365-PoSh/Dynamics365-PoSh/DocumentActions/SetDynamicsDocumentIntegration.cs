using Dynamics365_PoSh.Helpers;
using System.Management.Automation;
using System.Net;

namespace Dynamics365_PoSh.DocumentActions
{
    [Cmdlet(VerbsCommon.Set, "DynamicsDocumentIntegration", SupportsShouldProcess = true)]
    public class SetDynamicsDocumentIntegration : PSCmdlet
    {
        [Parameter(Mandatory = true, HelpMessage = "Server URL (with organization name) to environment")]
        public string ServerUrl;

        private CookieBrowser _browser = new CookieBrowser();
        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            var cookieVar = SessionState.PSVariable.Get(SessionVariableFactory.WebCookies);
            if (cookieVar == null)
            {
                Host.UI.WriteLine(@"Please use Connect-DynamicsService with the -AuthType flag set");
                return;
            }

            var cookies = cookieVar.Value as CookieCollection;

            // Magic Guid, not supported
            var wizardUri = ServerUrl.TrimEnd('/') + @"/WebWizard/WizardContainer.aspx?WizardId=2164dd44-6f89-430c-9c7b-abfa44320df0";
            _browser.Browse(wizardUri, cookies);
        }
    }
}
