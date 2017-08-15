using Dynamics365_PoSh.Helpers;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.WebServiceClient;
using System;
using System.Management.Automation;
using System.Net;
using System.Net.Http;

namespace Dynamics365_PoSh.ConnectionActions
{
    [Cmdlet(VerbsCommon.Get, "XrmConnection", SupportsShouldProcess = true, DefaultParameterSetName = "oauth")]
    public class GetXrmConnection : PSCmdlet
    {
        [Parameter(ParameterSetName = "oauth")]
        public string ClientId;
        [Parameter(ParameterSetName = "oauth", Mandatory = true)]
        public string Organization;
        [Parameter(ParameterSetName = "oauth", Mandatory = true)]
        [ValidateSet("North America", "South America", "Canada", "EMEA", "APAC", "Oceania", "Japan", "India", "North America 2", "United Kingdom", IgnoreCase = true)]
        public string Location;

        [Parameter(Mandatory = true, ParameterSetName = "cookie_auth")]
        public string ServerUrl;

        protected override void ProcessRecord()
        {
            // OAuth authentication
            if (string.IsNullOrEmpty(Organization))
            {
                if (string.IsNullOrEmpty(ClientId))
                {
                    while (string.IsNullOrEmpty(ClientId))
                    {
                        Console.WriteLine("Please provide the client id (Application ID) for OAuth authentication");
                        ClientId = Console.ReadLine();
                    }
                }
            }

            base.ProcessRecord();

            ServerUrl.TrimEnd('/');
            var webAuthCookies = WebAuthentication.GetAuthenticatedCookies("https://admin.services.crm4.dynamics.com", Models.AuthenticationType.O365);

            var serviceUrl = new Uri(DataCenterLocations.GetUrl("agur", DataCenterLocations.Locations.EMEA, "/XRMServices/2011/Organization.svc/web"));
            var oauth = new OAuth(serviceUrl.GetLeftPart(UriPartial.Authority));
            var token = oauth.AuthResult.AccessToken;
            var newClient = new OrganizationWebProxyClient(serviceUrl, false)
            {
                HeaderToken = token,
                SdkClientVersion = "8.2"
            };

            var result = newClient.Execute(new WhoAmIRequest());

            GetInstances(webAuthCookies);
            var client = CrmConnection.GetConnection(ServerUrl, webAuthCookies);

            SessionState.PSVariable.Set("CrmServerUrl", ServerUrl);
            SessionState.PSVariable.Set("AuthCookies", webAuthCookies);
            SessionState.PSVariable.Set("XrmService", client);
            WriteObject(client);
        }


        private static string GetInstances(CookieCollection cookies)
        {
            var client = new HttpClient();
            var req = new HttpRequestMessage(HttpMethod.Get, new System.Uri("https://admin.services.crm4.dynamics.com/api/v1/instances"));
            foreach (Cookie cookie in cookies)
            {
                req.Headers.Add(cookie.Name, cookie.Value);
            }
            var response = client.SendAsync(req);
            response.Wait();
            return response.Result.ToString();
        }
    }
}