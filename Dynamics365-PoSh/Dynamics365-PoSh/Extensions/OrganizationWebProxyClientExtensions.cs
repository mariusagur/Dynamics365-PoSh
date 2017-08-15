using Dynamics365_PoSh.Helpers;
using System.Net;

namespace Microsoft.Xrm.Sdk.WebServiceClient
{
    public static partial class OrganizationWebProxyClientExtensions
    {
        public static void SetAuthenticationCookies(this OrganizationWebProxyClient client, CookieCollection cookies)
        {
            var cookieContainer = new CookieContainer();
            foreach (Cookie cookie in cookies)
            {
                cookieContainer.Add(cookie);
            }
            var cookieBehavior = new CookieBehavior(cookieContainer);
            client.Endpoint.EndpointBehaviors.Add(cookieBehavior);
        }
    }
}
