using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Blob2Blob
{
    /*
     //Configuration//
     {
    "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "Blob2Blob_In": "blob2blob-in",
    "Blob2Blob_Out": "blob2blob-out"
  }
}
         */
    public static class Blob2Blob
    {
        [FunctionName("BlobToBlob")]
        [return: Blob("%Blob2Blob_Out%/archive-{rand-guid}-{name}")]
        public static string Run(
            [BlobTrigger("%Blob2Blob_In%/DK-{name}", Connection = "")]Stream inboundBlob,
            [Blob("%Blob2Blob_In%/RS-{name}")] string readSecondBlob,
            [Blob("%Blob2Blob_Out%/Jag-{rand-guid}.txt")] out string returnJag,
            [Blob("%Blob2Blob_Out%/Div-{rand-guid}.txt")] out string returnDiv,
            string name, ILogger log)
        {
            string vReturn = string.Empty;
            returnDiv = "";
            returnJag = "";
            log.LogInformation($"File recived in Inbound Blob\n Name:{name} \n Size: {inboundBlob.Length} Bytes");
            try
            {
                StreamReader reader = new StreamReader(inboundBlob);
                string strInboundBlob = reader.ReadToEnd();
                vReturn = strInboundBlob;

                if (readSecondBlob.Contains("DIV", comparisonType: StringComparison.OrdinalIgnoreCase))
                {
                    returnDiv = strInboundBlob + Environment.NewLine + readSecondBlob;
                }
                else if (readSecondBlob.Contains("JAG", comparisonType: StringComparison.OrdinalIgnoreCase))
                {
                    returnJag = "Jags:" +Environment.OSVersion +
                                Environment.NewLine + readSecondBlob;
                }
                else
                {
                    returnDiv = $"Machine: {Environment.MachineName} deosn't have value for Div";  
                    returnJag = $"Current Directory: {Environment.CurrentDirectory} deosn't have value for Jag";
                }
            }
            catch (Exception ex)
            {
                vReturn = $"Process has error [{ex.Message}].{Environment.NewLine}Completed unsuccessfully..";
                log.LogError($"Exception:{ex.Message}");
            }
            return vReturn;

        }
    }
}
