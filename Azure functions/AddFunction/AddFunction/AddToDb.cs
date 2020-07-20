using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using QC = Microsoft.Data.SqlClient;
using Microsoft.Data.SqlClient;

namespace FunctionAppAzure
{
    public static class AddToDb
    {
        public static string ADO = "";
        private static void ConnectToDB(string name, ILogger log)
        {
            using (var connection = new SqlConnection(ADO))
            {
                connection.Open();
                log.LogInformation("Connection opened");
                string query = $@"insert into text(student_name) values ('{name}')";
                using (var cmd = new SqlCommand(query, connection))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    reader.Close();
                }
            }
        }

        [FunctionName("AddtoDB")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            string name = data?.name;
            log.LogInformation($"{name}");

            ConnectToDB(name, log);

            return name != null
                ? (ActionResult)new OkObjectResult($"Hello, {name}")
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }
}