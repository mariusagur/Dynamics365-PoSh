using Dynamics365_PoSh.Helpers;
using System;
using System.Management.Automation;

namespace Dynamics365_PoSh.ConnectionActions
{
    [Cmdlet("Connect", "DynamicsService", DefaultParameterSetName = "oauthdata")]
    public class ConnectDynamicsService : PSCmdlet
    {
        [Parameter(ParameterSetName = "oauthdata")]
        [Parameter(ParameterSetName = "oauthadmin")]
        public string ClientId = null;
        [Parameter(ParameterSetName = "oauthdata")]
        [Parameter(ParameterSetName = "oauthadmin")]
        public string RedirectUri = null;

        [Parameter(Mandatory = true, ParameterSetName = "oauthdata")]
        [Parameter(Mandatory = true, ParameterSetName = "webauth")]
        public string Organization;

        [Parameter(Mandatory = true, ParameterSetName = "oauthdata")]
        [Parameter(Mandatory = true, ParameterSetName = "oauthadmin")]
        [Parameter(Mandatory = true, ParameterSetName = "webauth")]
        [ValidateSet("North America", "South America", "Canada", "EMEA", "APAC", "Oceania", "Japan", "India", "North America 2", "United Kingdom", IgnoreCase = true)]
        public string Location;

        [Parameter(Mandatory = true, ParameterSetName = "webauth")]
        [ValidateSet("CurrentUser", "Windows", "Forms", "O365")]
        public string AuthType;

        [Parameter(Mandatory = true, ParameterSetName = "oauthadmin")]
        public SwitchParameter AdminApi;

        private DataCenterLocations _location;

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            Enum.TryParse(Location, out _location);
            var serviceUrl = new Uri(string.Format(UrlFactory.BaseUrl, Organization, (int)_location, UrlFactory.OrgServiceQueryPath));

            // OAuth authentication, data connection
            if (ParameterSetName == "oauthdata")
            {
                var endpoint = UrlFactory.GetDiscoveryUrl(serviceUrl, ApiType.CustomerEngagement);
                var auth = new OAuthHelper(endpoint, ClientId, RedirectUri);
                var service = CrmConnection.GetConnection(serviceUrl, auth.AuthResult.AccessToken);
                SessionState.PSVariable.Set(SessionVariableFactory.OAuthData, auth);
                SessionState.PSVariable.Set(SessionVariableFactory.DataConnection, service);
            }
            // Cookie authentication, data connection
            else if (ParameterSetName == "webauth")
            {
                Enum.TryParse(AuthType, out Models.AuthenticationType authType);
                var webAuthCookies = WebAuthHelper.GetAuthenticatedCookies(serviceUrl.ToString(), authType);
                var service = CrmConnection.GetConnection(serviceUrl, webAuthCookies);
                SessionState.PSVariable.Set(SessionVariableFactory.WebCookies, webAuthCookies);
                SessionState.PSVariable.Set(SessionVariableFactory.DataConnection, service);
            }
            // OAuth authentication, admin API
            else if (ParameterSetName == "oauthadmin")
            {
                serviceUrl = UrlFactory.GetServiceUrl(_location);
                var endpoint = UrlFactory.GetDiscoveryUrl(serviceUrl, ApiType.Admin);
                var auth = new OAuthHelper(endpoint, ClientId, RedirectUri);
                SessionState.PSVariable.Set(SessionVariableFactory.OAuthAdmin, auth);
            }
        }
    }
}