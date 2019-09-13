using Dynamics365_PoSh.Helpers;
using Microsoft.Xrm.Sdk;
using System;
using System.Management.Automation;

namespace Dynamics365_PoSh.ConnectionActions
{
    [Cmdlet(VerbsCommon.Get, "DynamicsDataConnection", SupportsShouldProcess = true, DefaultParameterSetName = "oauth")]
    public class GetDynamicsDataConnection : PSCmdlet
    {
        [Parameter(ParameterSetName = "oauth")]
        public string ClientId = null;
        [Parameter(ParameterSetName = "oauth")]
        public string RedirectUri = null;

        [Parameter(Mandatory = true, ParameterSetName = "webauth")]
        [ValidateSet("CurrentUser", "Windows", "Forms", "O365")]
        public string AuthenticationType;

        [Parameter(Mandatory = true)]
        public string Organization;
        [Parameter(Mandatory = true)]
        [ValidateSet("North America", "South America", "Canada", "EMEA", "APAC", "Oceania", "Japan", "India", "North America 2", "United Kingdom", IgnoreCase = true)]
        public string Location;

        private DataCenterLocations _location;
        private IOrganizationService _service;

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            Enum.TryParse(Location, out _location);
            var serviceUrl = new Uri(string.Format(UrlFactory.BaseUrl, Organization, (int)_location, UrlFactory.OrgServiceQueryPath));

            // OAuth authentication
            if (ParameterSetName == "oauth")
            {
                var endpoint = UrlFactory.GetDiscoveryUrl(serviceUrl, ApiType.CustomerEngagement);
                var auth = new OAuthHelper(endpoint, ClientId, RedirectUri);
                _service = CrmConnection.GetConnection(serviceUrl, auth.AuthResult.AccessToken);
            }
            // Cookie authentication
            else if (ParameterSetName == "webauth")
            {
                Enum.TryParse(AuthenticationType, out Models.AuthenticationType authType);
                var webAuthCookies = WebAuthHelper.GetAuthenticatedCookies(serviceUrl.ToString(), authType);
                _service = CrmConnection.GetConnection(new Uri(serviceUrl, UrlFactory.OrgServiceQueryPath), webAuthCookies);
            }

            WriteObject(_service);
        }
    }
}