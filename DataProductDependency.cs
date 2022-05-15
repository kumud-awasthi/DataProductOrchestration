using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Collections;

namespace DataProductOrchestration
{
    public static class DataProductDependency
    {
        [FunctionName(nameof(GetDependencies))]
        public static string[] GetDependencies([ActivityTrigger] object input)
        {
            return Environment.GetEnvironmentVariable("Dependencies")
                    .Split(',')
                    //.Select(StringSplitOptions)//To be changed
                    .ToArray();
        }

        [FunctionName(nameof(DependencyValidation))]
        public static async Task<Boolean> DependencyValidation([ActivityTrigger] string inputDependencies, ILogger log)
        {
            //log.LogInformation($"Input Dependnencies {inputDependencies.DependencyID} to {inputDependencies.DependencyCondition}.");
            // simulate doing the activity
            await Task.Delay(5000);
            
            log.LogInformation($"Input Dependnencies " + inputDependencies);
            
            string pDependencyID = "";
            string pDataSubproductID = "";
            Boolean pstatus=false ;

            /*SqlConnection conn = new SqlConnection();
            SqlCommand cmd = new SqlCommand();

            
            string conn_Str = Environment.GetEnvironmentVariable("sqldb_connection");

            conn.ConnectionString = Environment.GetEnvironmentVariable("sqldb_connection");
            cmd.Connection = conn;

            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "GetDependencyStatus";
            cmd.Parameters.AddWithValue("@DependencyID", pDependencyID);
            cmd.Parameters.AddWithValue("@DataSubproductID", pDataSubproductID);
            cmd.Parameters.Add("@Status", System.Data.SqlDbType.Binary);
            cmd.Parameters["@Status"].Direction = System.Data.ParameterDirection.Output;
            
            try
            {
                conn.Open();
                int i = cmd.ExecuteNonQuery();
                //Storing the output parameters value in 3 different variables.  
                pstatus = (Boolean)cmd.Parameters["@Status"].Value;
                // Here we get all three values from database in above three variables.  
            }
            catch (Exception ex)
            {
                // throw the exception  
            }
            finally
            {
                conn.Close();
            }
            */
            return pstatus;
           
        }

        
        [FunctionName(nameof(DependencyValidation2))]
        public static  async Task<Boolean> DependencyValidation2([ActivityTrigger] string inputDependencies, ILogger log, DependencyStatus depStatus,
            [ActivityTrigger][Table("Approvals", "AzureWebJobsStorage")]  DependencyInfo dependencyInfo, Boolean pstatus
            ) 
        {
            //log.LogInformation($"Input Dependnencies {inputDependencies.DependencyID} to {inputDependencies.DependencyCondition}.");
            // simulate doing the activity
            //await Task.Delay(5000);

            log.LogInformation($"Input Dependnencies " + inputDependencies);

             pstatus = false;
            var approvalCode = Guid.NewGuid().ToString("N");
            depStatus = new DependencyStatus
            {
                PartitionKey = "Approval",
                RowKey = approvalCode,
                OrchestrationId = depStatus.OrchestrationId
            };

            log.LogInformation($"Sending approval request for " + depStatus.OrchestrationId);

            return pstatus;

        }

        [FunctionName(nameof(DependencyCheck))]
        public static async Task<Boolean> DependencyCheck([ActivityTrigger] bool[] taskresults ,ILogger log)
        {
            //log.LogInformation($"Input Dependnencies {inputDependencies.DependencyID} to {inputDependencies.DependencyCondition}.");
            // simulate doing the activity
            await Task.Delay(5000);
            bool pstatus = true ;
            log.LogInformation($"Input Dependnencies " + taskresults);
            string expression = "true and true and true";
            System.Data.DataTable table = new System.Data.DataTable();
            table.Columns.Add("", typeof(Boolean));
            table.Columns[0].Expression = expression;

            System.Data.DataRow r = table.NewRow();
            table.Rows.Add(r);
            bool blResult = (Boolean)r[0];
            //Console.WriteLine(blResult);

            return blResult;

        }




    }
}
