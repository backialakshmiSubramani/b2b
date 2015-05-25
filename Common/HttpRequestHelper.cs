// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpRequestHelper.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the HttpRequestHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Modules.Channel.B2B.Common
{
    public partial class HttpRequestHelper
    {
        private readonly string _baseUrl;

        public HttpRequestHelper(string baseUrl)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new ArgumentNullException("baseUrl");
            }

            _baseUrl = baseUrl;
        }
        public static HttpResponseMessage SendRequest(string url, RequestMethod method)
        {
            return SendRequest<object, HttpResponseMessage>(url, method, null);
        }


        public static TResponse SendRequest<TResponse>(string url, RequestMethod method)
            where TResponse : class
        {
            return SendRequest<object, TResponse>(url, method, null);
        }

        public static HttpResponseMessage SendEmptyRequest<TRequest>(string url, RequestMethod method)
            where TRequest : class
        {
            return SendRequest<TRequest, HttpResponseMessage>(url, method, null);
        }

        public static HttpResponseMessage SendRequest<TRequest>(string url, RequestMethod method, TRequest request)
            where TRequest : class
        {
            return SendRequest<TRequest, HttpResponseMessage>(url, method, request);
        }

        public static TResponse SendRequest<TRequest, TResponse>(string url, RequestMethod method, TRequest request)
            where TRequest : class
            where TResponse : class
        {
            return TestWebApi<TRequest, TResponse>(url, method, request).Result;
        }

        private static async Task<TResponse> TestWebApi<TRequest, TResponse>(string url, RequestMethod method, TRequest request)
            where TRequest : class
            where TResponse : class
        {
            using (var client = new HttpClient())
            {
                var baseUrl = ConfigurationReader.GetValue("ASN_BaseURL");
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = method == RequestMethod.Get
                    ? (await client.GetAsync(url))
                    : (await client.PostAsync<TRequest>(url, request, new JsonMediaTypeFormatter()));

                if (typeof(TResponse).Equals(typeof(HttpResponseMessage)))
                {
                    return response as TResponse;
                }
                else if (response.IsSuccessStatusCode)
                {
                    TResponse responseObject = await response.Content.ReadAsAsync<TResponse>();
                    return responseObject;
                }
                else
                {
                    throw new InvalidOperationException(string.Format("Response '{0}': {1}.", response.StatusCode, response.ReasonPhrase));
                }
            }
        }

        public enum RequestMethod
        {
            Get = 0,
            Post = 1
        }
    }
}
