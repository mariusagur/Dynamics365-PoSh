using Dynamics365_PoSh.Helpers;
using Dynamics365_PoSh.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Management.Automation;
using System.Net.Http;

namespace Dynamics365_PoSh
{
    [Cmdlet(VerbsCommon.Get, "DynamicsInstances")]
    public class GetDynamicsInstances : PSCmdlet
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

            using (var httpClient = new HttpClient(auth.Handler))
            {
                var result = httpClient.GetStringAsync(auth.Endpoint).Result;
                var instances = JsonConvert.DeserializeObject<List<InstanceDTO>>(result);
                WriteObject(instances);
            }
        }
    }
}
