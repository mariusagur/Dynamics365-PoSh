using Dynamics365_PoSh.Helpers;
using System;
using System.Management.Automation;
using System.Net.Http;

namespace Dynamics365_PoSh
{
    [Cmdlet(VerbsCommon.Get, "DynamicsInstanceTypeInfo")]
    public class GetDynamicsDynamicsInstanceTypeInfo : PSCmdlet
    {
        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            var authObject = SessionState.PSVariable.Get(SessionVariableFactory.OAuthAdmin);
            if (authObject == null)
            {
                Host.UI.WriteLine(@"Please use Connect-DynamicsService with the -AdminAPI switch first.");
                return;
            }
            var auth = authObject.Value as OAuthHelper;
            var serverUrl = auth.Endpoint.GetLeftPart(UriPartial.Authority);
            serverUrl += "/api/v1/InstanceTypeInfo";

            using (var httpClient = new HttpClient(auth.Handler))
            {
                var result = httpClient.GetStringAsync(serverUrl).Result;
                WriteObject(result);
            }
        }
    }
}
