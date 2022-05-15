using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using Microsoft.Azure.Management.DataFactory;
using Microsoft.Azure.Management.DataFactory.Models;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace DataProductOrchestration
{
    public static class HttpFunctions
    {
        [FunctionName("DataProductDeploy")]
        public static async Task<IActionResult> HttpStart(
[HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req,
[DurableClient] IDurableOrchestrationClient starter,
ILogger log)
        {
            {
                /*string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);

                string tenantId = data?.tenantId;
                string applicationId = data?.applicationId;
                string authenticationKey = data?.authenticationKey;
                string subscriptionId = data?.subscriptionId;
                string resourceGroup = data?.resourceGroup;
                string factoryName = data?.factoryName;
                string pipelineName = data?.pipelineName;


                log.LogInformation("C# HTTP trigger function processed a request" + (String)(data));
                log.LogInformation("C# HTTP trigger function processed a request {applicationId}.");
                log.LogInformation("C# HTTP trigger function processed a request.");
                log.LogInformation("C# HTTP trigger function processed a request.");
                log.LogInformation("C# HTTP trigger function processed a request.");

                */
                // Function input comes from the request content.
                string instanceId = await starter.StartNewAsync("DataProductOrchestrator", null);

                log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

                return starter.CreateCheckStatusResponse(req, instanceId);
            }
        }

        [FunctionName(nameof(SubmitDependencyStatus))]
        public static async Task SubmitDependencyStatus(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "SubmitDependencyStatus/{id}")]
            HttpRequest req,
            [DurableClient] IDurableOrchestrationClient client,
            [Table("DependencyStatus", "Approval", "{id}", Connection = "AzureWebJobsStorage")] DependencyStatus depStatus,
            ILogger log)
        {
            log.LogInformation("checking Depdency Status");
            await Task.Delay(1000);

         /*   // nb if the approval code doesn't exist, framework just returns a 404 before we get here
            string result = req.GetQueryParameterDictionary()["result"];

            if (result == null)
                return new BadRequestObjectResult("Need an approval result");

            log.LogWarning($"Sending approval result to {depStatus.OrchestrationId} of {result}");
            // send the ApprovalResult external event to this orchestration
            await client.RaiseEventAsync(depStatus.OrchestrationId, "ApprovalResult", result);

            return new OkResult();*/
        }
     [FunctionName("ExecuteADFPipeline")]
        public static async Task ExecuteADFPipeline([ActivityTrigger] string input, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            
            string tenantId = "929362e3-d427-48bb-a404-eac389aba310";//data?.tenantId;
            string applicationId = "3ba0ded1-d1ce-47b0-8dc4-352b27d4d6cc"; //data?.applicationId;
            string authenticationKey = "-kl8Q~j1aecQeudHyPtdN0DLojPKYTNtC1ViJaiQ"; //data?.authenticationKey;
            string subscriptionId = "c83648f1-f206-4abd-b4c3-fbec51567cc6"; //data?.subscriptionId;
            string resourceGroup = "NextGen";//data?.resourceGroup;
            string factoryName = "NextGen-testADF-2";//data?.factoryName;
            string pipelineName = "pipeline1";//data?.pipelineName;
            //Check body for values
           /* if (
                tenantId == null ||
                applicationId == null ||
                authenticationKey == null ||
                subscriptionId == null ||
                factoryName == null ||
                pipelineName == null
                )
            {
                return new BadRequestObjectResult("Invalid request body, value missing.");
            }
           */
            //Create a data factory management client
            var context = new AuthenticationContext("https://login.windows.net/" + tenantId);
            ClientCredential cc = new ClientCredential(applicationId, authenticationKey);
            AuthenticationResult result = context.AcquireTokenAsync("https://management.azure.com/", cc).Result;
            ServiceClientCredentials cred = new TokenCredentials(result.AccessToken);
            var client = new DataFactoryManagementClient(cred)
            {
                SubscriptionId = subscriptionId
            };

            //Run pipeline
            CreateRunResponse runResponse;
            PipelineRun pipelineRun;
            dynamic data;
                log.LogInformation("Called pipeline without parameters.");

                runResponse = client.Pipelines.CreateRunWithHttpMessagesAsync(
                    resourceGroup, factoryName, pipelineName).Result.Body;
            
            log.LogInformation("Pipeline run ID: " + runResponse.RunId);

            //Wait and check for pipeline result
            log.LogInformation("Checking pipeline run status...");
            while (true)
            {
                pipelineRun = client.PipelineRuns.Get(
                    resourceGroup, factoryName, runResponse.RunId);

                log.LogInformation("Status: " + pipelineRun.Status);

                if (pipelineRun.Status == "InProgress" || pipelineRun.Status == "Queued")
                    System.Threading.Thread.Sleep(15000);
                else
                    break;
            }

            //Final return detail
            string outputString = "{ \"PipelineName\": \"" + pipelineName + "\", \"RunIdUsed\": \"" + pipelineRun.RunId + "\", \"Status\": \"" + pipelineRun.Status + "\" }";
            JObject outputJson = JObject.Parse(outputString);
         //   return new OkObjectResult(outputJson);
        }
    }


}
