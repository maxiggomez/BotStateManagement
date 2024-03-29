﻿
namespace PostInAzureQueueByPost
{
    using System;
    using System.Globalization;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    internal static class Program
    {
        static string StorageAccountName = "mggchatbottest";
        static string StorageAccountKey = "WS4s3VF/W02FpO3AuiED6CoWaQc/kNWlzP61xF3y0d4vZ8U22bnvN6+mnYOMqZ6hkSF/VFJ7R5q6ZcoXiPFVVg==";

        private static void Main()
        {
            // List the containers in a storage account.
            //ListContainersAsyncREST(StorageAccountName, StorageAccountKey, CancellationToken.None).GetAwaiter().GetResult();

            //GetMessageQueue(StorageAccountName, StorageAccountKey, CancellationToken.None).GetAwaiter().GetResult();

            var mensaje = "<QueueMessage>" +
              "<MessageText> mensaje de texto 3</MessageText>" +
              "</QueueMessage>";

            Post(StorageAccountName, StorageAccountKey, "orders", mensaje, CancellationToken.None).GetAwaiter().GetResult();

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

            // Construct the URI. This will look like this:
            //   https://myaccount.blob.core.windows.net/resource
            String uri = string.Format("https://{0}.queue.core.windows.net?comp=list", storageAccountName);

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
                httpRequestMessage.Headers.Add("x-ms-version", "2017-04-17");
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

        private static async Task GetMessageQueue(string storageAccountName, string storageAccountKey, CancellationToken cancellationToken)
        {

            // Construct the URI. This will look like this:
            //   https://myaccount.blob.core.windows.net/resource
            String uri = string.Format("https://mggchatbottest.queue.core.windows.net/orders/messages", storageAccountName);

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
                httpRequestMessage.Headers.Add("x-ms-version", "2017-04-17");
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

                        Console.WriteLine(x);
                    }
                }
            }
        }




        private static async Task Post(string storageAccountName, string storageAccountKey, string queueName, string message, CancellationToken cancellationToken)
        {

            // Construct the URI. This will look like this:
            //   https://myaccount.blob.core.windows.net/resource
            String uri = string.Format("https://{0}.queue.core.windows.net/{1}/messages", storageAccountName, queueName);

            // Set this to whatever payload you desire. Ours is null because 
            //   we're not passing anything in.

            var buffer = System.Text.Encoding.ASCII.GetBytes(message);
            Byte[] requestPayload = buffer;

            //Instantiate the request message with a null payload.
            using (var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri)
            { Content = (requestPayload == null) ? null : new ByteArrayContent(requestPayload) })
            {

                // Add the request headers for x-ms-date and x-ms-version.
                DateTime now = DateTime.UtcNow;
                httpRequestMessage.Headers.Add("x-ms-date", now.ToString("R", CultureInfo.InvariantCulture));
                httpRequestMessage.Headers.Add("x-ms-version", "2017-04-17");
                httpRequestMessage.Headers.Add("x-ms-content-length", httpRequestMessage.Content.Headers.ContentLength.ToString());

                // Add the authorization header.
                httpRequestMessage.Headers.Authorization = AzureStorageAuthenticationHelper.GetAuthorizationHeader(
                   storageAccountName, storageAccountKey, now, httpRequestMessage);

                // Send the request.
                using (HttpResponseMessage httpResponseMessage = await new HttpClient().SendAsync(httpRequestMessage, cancellationToken))
                {
                    if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                    {
                        String xmlString = await httpResponseMessage.Content.ReadAsStringAsync();
                        XElement x = XElement.Parse(xmlString);

                        Console.WriteLine(x);
                    }
                }
            }
        }

    }
}
