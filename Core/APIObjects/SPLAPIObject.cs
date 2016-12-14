using Modules.Channel.B2B.Common;
using System.Collections.Generic;
using System.Net.Http;

namespace Modules.Channel.B2B.Core.APIObjects
{
    public class SPLAPIObject
    {
        public List<SPLAPIData> data { get; set; }
        public string version { get; set; }
        public int numberOfRecords { get; set; }
        public bool isSuccess { get; set; }
        public List<object> exceptionList { get; set; }
    }

    public class SPLAPIData
    {
        public string OrderCode { get; set; }
        public string VendorPartNumber { get; set; }
        public string UPCEAN { get; set; }
        public string AssociatedSNPSkus { get; set; }
        public string AssociatedServiceSkus { get; set; }
        public string BoxDimensions { get; set; }
        public string BoxWeight { get; set; }
        public string MinimumOrderQty { get; set; }
        public string PalletLayerQty { get; set; }
        public string PalletTotalQty { get; set; }
        public string PalletLength { get; set; }
        public string PalletWidth { get; set; }
        public string PalletHeight { get; set; }
        public string PalletLayerPerPallet { get; set; }
        public string PackSeparate { get; set; }
        public string CasePackQuantity { get; set; }
        public string CustomsCode { get; set; }
        public string ShippingInstructions { get; set; }
        public string DropShip { get; set; }
        public string CountryofOrigin { get; set; }
        public string EPEAT { get; set; }
        public string ECCN { get; set; }
        public string UHG { get; set; }
        public string Levy { get; set; }
        public string SerialScan { get; set; }
        public string Comments { get; set; }
        public string EstimatedEOLDate { get; set; }
        public string ReplacementVPN { get; set; }
        public string Promo { get; set; }
        public string OfflineQuoteNumber { get; set; }
        public string CountryCode { get; set; }
        public string Model { get; set; }
        public string UPC { get; set; }
        public string EAN { get; set; }
        public string JAN { get; set; }
        public string ConfigType { get; set; }
        public string LineofBusiness { get; set; }
        public string Type { get; set; }
        public string ProductCategory { get; set; }
        public string ProductSubCategory { get; set; }
        public string ShortDescription { get; set; }
        public string Aio { get; set; }
        public string EStar { get; set; }
        public string ResellerPrice { get; set; }
        public string ResellerDeltaPrice { get; set; }
        public string MCQ { get; set; }
        public Dictionary<string, string> TechSpec { get; set; }
        public Dictionary<string, string> Images { get; set; }

        public SPLAPIObject GetSPLAPIData()
        {
            SPLAPIObject splData = null;
            string uri = ConfigurationReader.GetValue("SPLWebApiURL");
            if (!string.IsNullOrEmpty(uri))
            {
                using (var client = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true }))
                using (var response = client.GetAsync(uri).Result)
                {
                    if (response.IsSuccessStatusCode)
                    {
                        splData = response.Content.ReadAsAsync<SPLAPIObject>().Result;
                    }
                }
            }
            return splData;
        }
    }
}
