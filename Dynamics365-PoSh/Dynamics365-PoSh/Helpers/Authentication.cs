using Microsoft.Xrm.Sdk;
using System;
using System.Net;

namespace Dynamics365_PoSh.Helpers
{
    public class Authentication
    {
        public OAuth OAuthDataConnection { get; set; }
        public OAuth OAuthAdminConnection { get; set; }
        private CookieCollection _cookies { get; set; }
        public string _serviceUrl { get; set; }
        private IOrganizationService _service = null;

        public Authentication(OAuth adminConnection, OAuth dataConnection)
        {
            OAuthAdminConnection = adminConnection;
            OAuthDataConnection = dataConnection;
        }

        public Authentication(CookieCollection cookies, string serviceUrl)
        {
            _cookies = cookies;
            _serviceUrl = serviceUrl;
        }

        public IOrganizationService OrganizationService
        {
            get
            {
                InstantiateOrganizationService();
                return _service;
            }
        }

        private void InstantiateOrganizationService()
        {
            if (_cookies != null)
            {
                _service = CrmConnection.GetConnection(_serviceUrl, _cookies, false);
            }
            else if (OAuthDataConnection != null)
            {
                var serverUrl = new Uri(OAuthDataConnection.OrganizationUrl).GetLeftPart(UriPartial.Authority);
                _service = CrmConnection.GetConnection(serverUrl, OAuthDataConnection.AuthResult.AccessToken);
            }
            else
            {
                Console.WriteLine("Please enter organization name");
                var org = Console.ReadLine();
                Console.WriteLine("Please choose OAuth [o] or Web authentication (cookie) [w]");
                var key = Console.ReadKey();
                if (key.KeyChar == 'o')
                {
                    var serverUrl = DataCenterLocations.GetUrl(org, DataCenterLocations.Locations.EMEA);
                    //OAuthDataConnection = new OAuth( )
                }
                else if (key.KeyChar == 'w')
                {

                }
            }
        }

        private static void GetServerInformation()
        {
            Console.WriteLine("");
        }
    }
}
