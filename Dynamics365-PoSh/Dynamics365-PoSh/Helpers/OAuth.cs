using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Dynamics365_PoSh.Helpers
{
    /// <summary>
    /// Manages authentication to the Online Management API service.
    /// Uses Microsoft Azure Active Directory Authentication Library (ADAL) 
    /// to handle the OAuth 2.0 protocol. 
    /// </summary>
    public class OAuth
    {
        private HttpMessageHandler _clientHandler = null;
        private AuthenticationContext _authContext = null;
        private AuthenticationResult _authResult = null;
        private string _authority = null;
        private string _serviceUrl = null;

        // These values are obtained on registering your application with the 
        // Azure Active Directory.
        private static string _resource = null;
        private static string _clientId = "5498c4a2-9049-4a17-a714-c2356475c454";
        private static string _redirectUrl = "urn:ietf:wg:oauth:2.0:oob";

        #region Constructors
        /// <summary>
        /// Base constructor.
        /// </summary>
        public OAuth(string serviceUrl, string clientId = null, string redirectUrl = null)
        {
            _serviceUrl = serviceUrl;
            _clientId = clientId ?? _clientId;
            _redirectUrl = redirectUrl ?? _redirectUrl;
            SetClientHandler();
        }
        #endregion Constructors

        #region Properties
        /// <summary>
        /// The authentication context.
        /// </summary>
        public AuthenticationContext Context
        {
            get
            { return _authContext; }

            set
            { _authContext = value; }
        }

        /// <summary>
        /// The authentication result
        /// </summary>
        public AuthenticationResult AuthResult
        {
            get
            {
                Authorize();
                return _authResult;
            }
        }

        public string OrganizationUrl
        {
            get
            { return _serviceUrl; }
        }

        /// <summary>
        /// The HTTP client message handler.
        /// </summary>
        public HttpMessageHandler ClientHandler
        {
            get
            { return _clientHandler; }

            set
            { _clientHandler = value; }
        }

        private void DiscoverAuthority(Uri discoveryUrl)
        {
            try
            {
                Task.Run(async () =>
                {
                    AuthenticationParameters ap = await AuthenticationParameters.CreateFromResourceUrlAsync(discoveryUrl);
                    _resource = ap.Resource;
                    _authority = ap.Authority;
                }).Wait();
            }
            catch (HttpRequestException e)
            {
                throw new Exception("An HTTP request exception occurred during authority discovery.", e);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Returns the authentication result for the configured authentication context.
        /// </summary>
        /// <returns>The refreshed access token.</returns>
        /// <remarks>Refresh the access token before every service call to avoid having to manage token expiration.</remarks>
        private void Authorize()
        {
            if (_authResult == null || _authResult.ExpiresOn < DateTime.Now)
            {
                Task.Run(async () =>
                {
                    var authResult = await _authContext.AcquireTokenAsync(_resource, _clientId, new Uri(_redirectUrl),
                    new PlatformParameters(PromptBehavior.Always));
                    _authResult = authResult;
                }).Wait();
            }
        }

        /// <summary>
        /// Sets the client message handler.
        /// </summary>
        private void SetClientHandler()
        {
            _clientHandler = new OAuthMessageHandler(this, new HttpClientHandler());
            var discoveryUrl = _serviceUrl.StartsWith("https://admin.services.", StringComparison.InvariantCultureIgnoreCase) ?
                new Uri(_serviceUrl + "/api/aad/challenge") : new Uri(_serviceUrl + "/api/data");
            DiscoverAuthority(discoveryUrl);
            _authContext = new AuthenticationContext(_authority, false);
        }

        #endregion Methods

        /// <summary>
        /// Custom HTTP client handler that adds the Authorization header to message requests.
        /// </summary>
        class OAuthMessageHandler : DelegatingHandler
        {
            OAuth _auth = null;

            public OAuthMessageHandler(OAuth auth, HttpMessageHandler innerHandler)
                : base(innerHandler)
            {
                _auth = auth;
            }

            protected override Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
            {
                // It is a best practice to refresh the access token before every message request is sent. Doing so
                // avoids having to check the expiration date/time of the token. This operation is quick.
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _auth.AuthResult.AccessToken);

                return base.SendAsync(request, cancellationToken);
            }
        }
    }
}