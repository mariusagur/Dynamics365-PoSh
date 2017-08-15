namespace Dynamics365_PoSh.Helpers
{
    public class DataCenterLocations
    {
        public static string BaseUrl = "https://{0}.crm{1}.dynamics.com{2}";

        public static string GetUrl(string subdomain, Locations location, string resource = "")
        {
            if (location == Locations.NorthAmerica)
            {
                return string.Format(BaseUrl, subdomain, "", resource);
            }
            else
            {
                return string.Format(BaseUrl, subdomain, (int)location, resource);
            }
        }

        public static string GetServiceUrl(Locations location, string operation = "")
        {
            return GetUrl("admin.services", location, $"/api/v1/{operation}");
        }

        public enum Locations
        {
            NorthAmerica = 1,
            SouthAmerica = 2,
            Canada = 3,
            EMEA = 4,
            APAC = 5,
            Oceania = 6,
            Japan = 7,
            India = 8,
            NorthAmerica2 = 9,
            UnitedKingdom = 11
        }
    }
}
