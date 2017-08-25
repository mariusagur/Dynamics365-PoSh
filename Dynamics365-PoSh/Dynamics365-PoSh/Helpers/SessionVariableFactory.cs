namespace Dynamics365_PoSh.Helpers
{
    public static class SessionVariableFactory
    {
        private static string _webCookies = "cookieauth";
        private static string _oauthData = "oauthdatatoken";
        private static string _oauthAdmin = "oauthadmintoken";
        private static string _dataConnection = "dataconnection";

        public static string WebCookies
        {
            get
            { return _webCookies; }
        }
        public static string OAuthData
        {
            get
            { return _oauthData; }
        }
        public static string OAuthAdmin
        {
            get
            { return _oauthAdmin; }
        }
        public static string DataConnection
        {
            get { return _dataConnection; }
        }
    }
}
