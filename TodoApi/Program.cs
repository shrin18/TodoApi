namespace TodoApi
{
    using System;
    using System.Globalization;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using System.Text;
    using Newtonsoft.Json;
    using System.Data.SqlClient;
    using System.Collections.Generic;

    internal static class Program
    {
        static string StorageAccountName = "sigmaiot";
        static string StorageAccountKey = "3Vupif/boWGadEZ9eJOt7JysQDgxuKp35grvjPw0Vd4=";
        static string deviceId = "testdevice";
        static string sensorType = "Temperature";
        

        private static void Main()
        {
            // List the containers in a storage account.
            ListContainersAsyncREST(StorageAccountName, StorageAccountKey, CancellationToken.None).GetAwaiter().GetResult();

            Console.WriteLine("Press any key to continue.");
            Console.ReadLine();
        }

        /// <summary>
        /// This is the method to call the REST API to retrieve a list of
        /// containers in the specific storage account.
        /// This will call CreateRESTRequest to create the request, 
        /// then check the returned status code. If it's OK (200), it will 
        /// parse the response and show the list of containers found.
        /// </summary>
        private static async Task ListContainersAsyncREST(string storageAccountName, string storageAccountKey, CancellationToken cancellationToken)
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            using (var client = new HttpClient(handler))
            {

                var response = await client.GetAsync($"https://sigmaiotexercisetest.blob.core.windows.net/api/v1/devices/testdevice/data/2018-09-18/temperature");

                var result = await response.Content.ReadAsStringAsync();

                
            }
            // Construct the URI. This will look like this:
            //   https://myaccount.blob.core.windows.net/resource
            String uri = string.Format("http://{0}.blob.core.windows.net/api/v1/devices/{1}/2018-09-18/{2}", storageAccountName, deviceId, sensorType);
            // Set this to whatever payload you desire. Ours is null because 
            //   we're not passing anything in.
            Byte[] requestPayload = null;

            //Instantiate the request message with a null payload.
            using (var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri)
            { Content = (requestPayload == null) ? null : new ByteArrayContent(requestPayload) })
            {
                // Add the request headers for x-ms-date and x-ms-version.
                DateTime now = DateTime.UtcNow;
                httpRequestMessage.Headers.Add("x-ms-date", now.ToString("R", CultureInfo.InvariantCulture));
                httpRequestMessage.Headers.Add("x-ms-version", "2018-09-18");
             
                // If you need any additional headers, add them here before creating
                //   the authorization header. 

                // Add the authorization header.
                httpRequestMessage.Headers.Authorization = AzureStorageAuthenticationHelper.GetAuthorizationHeader(
                   storageAccountName, storageAccountKey, now, httpRequestMessage);

                // Send the request.
                using (HttpResponseMessage httpResponseMessage = await new HttpClient().SendAsync(httpRequestMessage, cancellationToken))
                {
                    // If successful (status code = 200), 
                    //   parse the XML response for the container names.
                    if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                    {
                        String xmlString = await httpResponseMessage.Content.ReadAsStringAsync();
                        XElement x = XElement.Parse(xmlString);
                        foreach (XElement container in x.Element("Containers").Elements("Container"))
                        {
                            Console.WriteLine("Container name = {0}", container.Element("Name").Value);
                        }
                    }
                }
            }

        }
    }
}
