// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccessApi.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the AccessApi type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Net;
using System.Net.Http;
using System;
using System.IO;

namespace Modules.Channel.B2B.Common
{
    /// <summary>
    /// Class containing methods to access the service.
    /// </summary>
    public static class AccessApi
    {
        /// <summary>
        /// Method that gets the response for a request object provided.
        /// </summary>
        /// <param name="url">
        /// The URI of the service.
        /// </param>
        /// <param name="request">
        /// The request object.
        /// </param>
        /// <typeparam name="TRequest">
        /// </typeparam>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        public static HttpResponseMessage GetResponse<TRequest>(string url, TRequest request) where TRequest : class
        {
            var response = HttpRequestHelper.SendRequest(url, HttpRequestHelper.RequestMethod.Post, request);
            return response;
        }

        /// <summary>
        /// Method for posting a JSON request.
        /// </summary>
        /// <param name="serviceUrl">
        /// The service url.
        /// </param>
        /// <param name="jsonText">
        /// The json text.
        /// </param>
        /// <returns>
        /// The <see cref="HttpWebResponse"/>.
        /// </returns>
        public static HttpWebResponse PostJsonRequest(string serviceUrl, string jsonText)
        {
            // This method uses HTTP post method for JSON Request
            var request = WebRequest.CreateHttp(serviceUrl);

            request.Accept = "application/json";
            request.ContentType = "application/json";
            request.Method = "POST";

            var requestBody = "[" + jsonText + "]";

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(requestBody);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var response = (HttpWebResponse)request.GetResponse();

            // *********Enclose PostJsonRequest method call in a try block and add the following code********** //

            // This piece of code is to check the response for Malformed JSON request.
            // It is suppoed to throw exception

            ////catch (Exception ex)
            ////{
            ////    Console.WriteLine("Exception occured after firing ASN JSON request, response is null or error received");
            ////    Console.WriteLine(ex.StackTrace);
            ////}

            return response;
        }
    }
}
