using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace TodoApi.AzureBlobStorage
{
    class Program
    {
        private static string ACCOUNT_NAME = ConfigurationManager.AppSettings["accountName"];
        private static string STORAGE_ACCOUNT_KEY = ConfigurationManager.AppSettings["accountKey"];
        private static string STORAGE_ENDPOINT = "https://" + ConfigurationManager.AppSettings["accountName"] + ".blob.core.windows.net/";
        private static string CONTAINER = ConfigurationManager.AppSettings["container"];
        private static string BLOB_NAME = ConfigurationManager.AppSettings["blobName"];

        static void Main(string[] args)
        {
            PutBlob(CONTAINER, BLOB_NAME);

            ListBlobs(CONTAINER);
        }

        /// <summary>
        /// Insere um arquivo - https://msdn.microsoft.com/pt-br/library/azure/dd179451.aspx
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        private static void PutBlob(String containerName, string blobName)
        {
           
            const string requestMethod = "PUT";

            var urlPath = String.Format("{0}/{1}", containerName, blobName);
            const string storageServiceVersion = "2017-11-09";

            var dateInRfc1123Format = DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture);

            var blobContent = File.ReadAllBytes(ConfigurationManager.AppSettings["filePath"]);
            var blobLength = blobContent.Length;

            const String blobType = "BlockBlob";

            var canonicalizedHeaders = String.Format(
                    "x-ms-blob-type:{0}\nx-ms-date:{1}\nx-ms-version:{2}",
                    blobType,
                    dateInRfc1123Format,
                    storageServiceVersion);

            var canonicalizedResource = String.Format("/{0}/{1}", ACCOUNT_NAME, urlPath);

            var stringToSign = String.Format("{0}\n\n\n{1}\n\n\n\n\n\n\n\n\n{2}\n{3}",
                    requestMethod,
                    blobLength,
                    canonicalizedHeaders,
                    canonicalizedResource);

            var authorizationHeader = CreateAuthorizationHeader(stringToSign);

            var uri = new Uri(STORAGE_ENDPOINT + urlPath);

            var request = (HttpWebRequest)WebRequest.Create(uri);

            request.Method = requestMethod;

            request.Headers.Add("x-ms-blob-type", blobType);
            request.Headers.Add("x-ms-date", dateInRfc1123Format);
            request.Headers.Add("x-ms-version", storageServiceVersion);
            request.Headers.Add("Authorization", authorizationHeader);

            request.ContentLength = blobLength;

            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(blobContent, 0, blobLength);
            }

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                var eTag = response.Headers["ETag"];
            }
        }

        /// <summary>
        /// Listar arquivos de um container - https://msdn.microsoft.com/pt-br/library/azure/dd135734.aspx
        /// </summary>
        /// <param name="containerName"></param>
        public static void ListBlobs(String containerName)
        {
            const string requestMethod = "GET";

            var urlPath = String.Format("restype=containner&comp=list");

            const string storageServiceVersion = "2017-11-09";

            var dateInRfc1123Format = DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture);

            var canonicalizedHeaders = String.Format(
                    "x-ms-date:{0}\nx-ms-version:{1}",
                    dateInRfc1123Format,
                    storageServiceVersion);

            var canonicalizedResource = String.Format("/{0}/{1}{2}", ACCOUNT_NAME, containerName, "\ncomp:list\nrestype:container");

            var stringToSign = String.Format("{0}\n\n\n\n\n\n\n\n\n\n\n\n{1}\n{2}",
                    requestMethod,
                    canonicalizedHeaders,
                    canonicalizedResource);

            var authorizationHeader = CreateAuthorizationHeader(stringToSign);

            var uri = new Uri(STORAGE_ENDPOINT + containerName + "?" + urlPath);

            var request = (HttpWebRequest)WebRequest.Create(uri);

            request.Method = requestMethod;

            request.Headers.Add("x-ms-date", dateInRfc1123Format);
            request.Headers.Add("x-ms-version", storageServiceVersion);
            request.Headers.Add("Authorization", authorizationHeader);

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    var reader = new StreamReader(stream, Encoding.UTF8);

                    //XML RESPONSE
                    var responseXml = reader.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Método que cria o header authorization
        /// </summary>
        /// <param name="canonicalizedString"></param>
        /// <returns></returns>
        public static String CreateAuthorizationHeader(String canonicalizedString)
        {
            string signature;

            using (var hmacSha256 = new HMACSHA256(Convert.FromBase64String(STORAGE_ACCOUNT_KEY)))
            {
                var dataToHmac = Encoding.UTF8.GetBytes(canonicalizedString);
                signature = Convert.ToBase64String(hmacSha256.ComputeHash(dataToHmac));
            }

            var authorizationHeader = String.Format(
                CultureInfo.InvariantCulture,
                "{0} {1}:{2}",
                "SharedKey",
                ACCOUNT_NAME,
                signature
            );

            return authorizationHeader;
        }
    }
}
