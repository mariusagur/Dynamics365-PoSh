using Dynamics365_PoSh.Helpers;
using System;
using System.Management.Automation;
using System.Net;

namespace Dynamics365_PoSh.DocumentActions
{
    [Cmdlet(VerbsCommon.Set, "XrmDocumentIntegration", SupportsShouldProcess = true)]
    public class SetXrmDocumentIntegration : PSCmdlet
    {
        [Parameter]
        public string ServerUrl;

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            if (SessionState.PSVariable.Get("AuthCookies") == null)
            {
                while (string.IsNullOrWhiteSpace(ServerUrl) && !Uri.TryCreate(ServerUrl, UriKind.Absolute, out Uri validationUri))
                {
                    Host.UI.WriteLine(@"Please specify a valid url for your Dynamics365 environment");
                    ServerUrl = Host.UI.ReadLine();
                }
                SessionState.PSVariable.Set("CrmServerUrl", ServerUrl);
                var webCookies = WebAuthentication.GetAuthenticatedCookies(ServerUrl, Models.AuthenticationType.O365);
                SessionState.PSVariable.Set("AuthCookies", webCookies);
            }
            var cookies = SessionState.PSVariable.Get("AuthCookies").Value as CookieCollection;
            var cookieBrowser = new CookieBrowser();

            ServerUrl = ServerUrl.EndsWith("/") ? ServerUrl : ServerUrl + "/";
            cookieBrowser.Browse(ServerUrl + @"WebWizard/WizardContainer.aspx?WizardId=2164dd44-6f89-430c-9c7b-abfa44320df0", cookies);
        }
    }
}
