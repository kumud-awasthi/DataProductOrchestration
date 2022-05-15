using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using System;
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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;


namespace DataProductOrchestration
{
    public static class DataProductOrchestrator
    {
        [FunctionName("DataProductOrchestrator")]
        public static async Task<bool> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log)
        { 
              var videoLocation = context.GetInput<string>();
        var dependencies = await context.CallActivityAsync<string[]>("GetDependencies", null);
        var dependencyTasks= new List<Task<Boolean>>();

            foreach (var dependency in dependencies)
            {
               var task = context.CallActivityAsync<Boolean>("DependencyValidation", dependency);
                 //await context.WaitForExternalEvent<string>(dependency);
                log.LogInformation($"WaitForExternalEvent for " + dependency);

                dependencyTasks.Add(task);
            }

            //await context.CallActivityAsync<bool>("SubmitDependencyStatus", null);
            string externalResponse = await context.WaitForExternalEvent<string>("ApprovalResult");

            //log.LogInformation($"WaitForExternalEvent" + externalResponse);

            bool dependencyResults = await context.CallActivityAsync<bool>("DependencyCheck", null);
            //return dependencyResults;
            string input ="test";
            if (dependencyResults == true)
            {
                log.LogInformation($"running True ADF Trigger.");
                await context.CallActivityAsync("ExecuteADFPipeline", input);
                return dependencyResults;
            }
            else
            {
                log.LogInformation($"running True ADF Trigger.");
                await context.CallActivityAsync("DataProductActivity",input);
                return dependencyResults;
            }
        }

        /*{
            var outputs = new List<string>();

            // Replace "hello" with the name of your Durable Activity Function.
            outputs.Add(await context.CallActivityAsync<string>("DataProductActivity", "Tokyo"));
            outputs.Add(await context.CallActivityAsync<string>("DataProductActivity", "Seattle"));
            outputs.Add(await context.CallActivityAsync<string>("DataProductActivity", "London"));

            // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
            return outputs;
        }*/

        [FunctionName(nameof(DataProductActivity))]
        public static async Task DataProductActivity([ActivityTrigger] string input,ILogger log)
        {
            log.LogInformation($"Publishing ADF Trigger.");
            // simulate publishing


            await Task.Delay(1000);
        }

           }
}