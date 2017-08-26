using Dynamics365_PoSh.Helpers;
using Dynamics365_PoSh.Models;
using Newtonsoft.Json;
using System;
using System.Management.Automation;
using System.Net.Http;
using System.Text;

namespace MSDYN365AdminApiAndMore
{
    [Cmdlet(VerbsCommon.New, "DynamicsBackup")]
    public class NewDynamicsBackup : PSCmdlet
    {
        [Parameter(Mandatory = true, ParameterSetName = "pipedInput", ValueFromPipeline = true)]
        public InstanceDTO instance;

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
            var serverUrl = auth.Endpoint.GetLeftPart(UriPartial.Authority);
            serverUrl = serverUrl + "/api/v1/InstanceBackups";

            using (var httpClient = new HttpClient(auth.Handler))
            {
                var req = new BackupRequestDTO
                {
                    AzureStorageInformation = new BackupRequestDTO.AzureStorage
                    {
                        ContainerName = "DynamicsBackup",
                        StorageAccountKey = "I'm not sharing this key with you",
                        StorageAccountName = "AccountName"
                    },
                    InstanceId = instance.Id,
                    IsAzureBackup = true,
                    Label = "TestBackup001",
                    Notes = "Testing backup to azure"
                };
                var result = httpClient.PostAsync(serverUrl, new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json"));
                WriteObject(result.Result);
            }
        }
    }
}
