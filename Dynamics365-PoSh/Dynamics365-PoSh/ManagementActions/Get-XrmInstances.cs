using Dynamics365_PoSh.Helpers;
using System;
using System.Management.Automation;
using System.Net.Http;

namespace Dynamics365_PoSh.ManagementActions
{
    [Cmdlet(VerbsCommon.Get, "XrmInstances", SupportsShouldProcess = true, DefaultParameterSetName = "oauth")]
    public class Get_XrmInstances : Cmdlet
    {
        [Parameter(ParameterSetName = "oauth")]
        public string ClientId;
        [Parameter(ParameterSetName = "oauth", Mandatory = true)]
        public string InstanceName;
        [Parameter(ParameterSetName = "oauth", Mandatory = true)]
        [ValidateSet("NorthAmerica", "SouthAmerica", "Canada", "EMEA", "APAC", "Oceania", "Japan", "India", "NorthAmerica2", "UnitedKingdom", IgnoreCase = true)]
        public string Location;

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            Enum.TryParse(Location, out DataCenterLocations.Locations location);
            var serviceUrl = new Uri(DataCenterLocations.GetServiceUrl(location, "instances"));
            var oauth = new OAuth(serviceUrl.GetLeftPart(UriPartial.Authority), ClientId);
            var client = new HttpClient(oauth.ClientHandler);
            var req = new HttpRequestMessage(HttpMethod.Get, serviceUrl);

            var result = client.SendAsync(req);
            result.Wait();
            Console.WriteLine(result.Result);
            Console.ReadLine();
        }
    }
}
