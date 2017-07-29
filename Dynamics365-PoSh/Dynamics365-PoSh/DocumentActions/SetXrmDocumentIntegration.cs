using Dynamics365_PoSh.Helpers;
using System;
using System.Management.Automation;
using System.Net;
using System.Windows.Forms;

namespace Dynamics365_PoSh.DocumentActions
{
    [Cmdlet(VerbsCommon.Set, "XrmDocumentIntegration", SupportsShouldProcess = true)]
    public class SetXrmDocumentIntegration : PSCmdlet
    {
        [Parameter(Mandatory = true)]
        public string CrmServerUrl;

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            if (SessionState.PSVariable.Get("AuthCookies") == null)
            {
                string serverUrl = null;
                while (string.IsNullOrWhiteSpace(serverUrl) && !Uri.TryCreate(serverUrl, UriKind.Absolute, out Uri validationUri))
                {
                    Host.UI.WriteLine(@"Please specify a valid url for your Dynamics365 environment");
                    serverUrl = Host.UI.ReadLine();
                }
                var webCookies = WebAuthentication.GetAuthenticatedCookies(serverUrl, Models.AuthenticationType.O365);
                SessionState.PSVariable.Set("AuthCookies", webCookies);
            }
            var cookies = SessionState.PSVariable.Get("AuthCookies").Value as CookieCollection;
            var cookieBrowser = new CookieBrowser();

            CrmServerUrl = CrmServerUrl.EndsWith("/") ? CrmServerUrl : CrmServerUrl + "/";
            cookieBrowser.Browse(CrmServerUrl + @"WebWizard/WizardContainer.aspx?WizardId=2164dd44-6f89-430c-9c7b-abfa44320df0", cookies);

            //var DisplayLoginForm = new Form();
            //DisplayLoginForm.SuspendLayout();


            //DisplayLoginForm.Width = DEFAULT_WEBBROWSER_POP_UP_WIDTH;
            //DisplayLoginForm.Height = DEFAULT_WEBBROWSER_POP_UP_HEIGHT;
            //DisplayLoginForm.Text = this.fldTargetSiteUrl;

            //DisplayLoginForm.Controls.Add(cookieBrowser);
            //DisplayLoginForm.ResumeLayout(false);
        }
    }
}
