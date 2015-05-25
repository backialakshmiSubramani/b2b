using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Modules.Channel.B2B.Common
{
    public partial class HttpRequestHelper
    {
        /// <summary>
        /// Gets instance of <see cref="HttpRequestHelper"/> that takes base URL
        /// </summary>
        /// <returns></returns>
        public static HttpRequestHelper Default()
        {
            return HttpRequestHelper.FromUrl(ConfigurationReader.GetValue("ASN_BaseURL"));
        }

        public static HttpRequestHelper FromUrl(string url)
        {
            return new HttpRequestHelper(url);
        }

        //public enum RequestMethod
        //{
        //    GET = 0,
        //    POST
        //}
    }
}
