using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Modules.Channel.B2B.Common
{
    public enum B2BEnvironment
    {
        Production,
        Preview
    }

    public enum CatalogType
    {
        Original,
        Delta
    }

    public enum Computation
    {
        EqualTo,
        GreaterThan,
        LessThan,
        GreaterThanOrEqualTo,
        LessThanOrEqualTo
    };

    public struct ScheduleIdentity
    {
        public Guid IdentityGuid;
        public long scheduleId;
    }

    public class CatalogProp
    {
        public string CatalogFormatType = "Hierarchical";
        public string CountryCode;
        public string SubLocationCode = "";
        public string RequesterEmailId = "Venkatesh_Vantri";
        public string SupplierId = "";
        public string Comments = "";
        public Region Region;
        public bool GPEnabled = false;
        public string GPShipToCurrency = "";
        public string GPShipToCountry = "";
        public string GPShipToLanguage;
        public string GPPurchaseOption = "";
        public bool CPFEnabled = false;
        public int GracePeriod = 1;
        public int ProfileId = 0;
        public string CustomerID = "";
        public string AccessGroup = "00000000-0000-0000-0000-000000000000";
        public string MessageType = "832";
        public string Sender = "DELL B2B";

        public CatalogProp(Region region)
        {
            this.Region = region;
            switch (region)
            {
                case Region.US:
                    this.CountryCode = "US";
                    this.GPShipToLanguage = "en";
                    break;
                case Region.EMEA:
                    this.CountryCode = "UK";
                    this.GPShipToLanguage = "en";
                    break;
                default:
                    break;
            }
        }
    }

    public class CatalogPropEMEA
    {
        public const string CatalogFormatType = "Hierarchical";
        public const string CountryCode = "US";
        public const string SubLocationCode = "";
        public const string RequesterEmailId = "Venkatesh_Vantri";
        public const string SupplierId = "";
        public const string Comments = "";
        public const string Region = "US";
        public const bool GPEnabled = false;
        public const string GPShipToCurrency = "";
        public const string GPShipToCountry = "";
        public const string GPShipToLanguage = "EN";
        public const string GPPurchaseOption = "";
        public const bool CPFEnabled = false;
        public const int GracePeriod = 1;
        public const int ProfileId = 0;
        public const string CustomerID = "";
        public const string AccessGroup = "00000000-0000-0000-0000-000000000000";
        public const string MessageType = "832";
        public const string Sender = "DELL B2B";
    }

    public enum CatalogItemType
    {
        ConfigWithDefaultOptions = 0,
        SNP = 2,
        Systems = 3,
        ConfigWithUpsellDownsell = 1
    }

    public enum CatalogOperation
    {
        Create,
        CreateAndPublish
    }

    public static class CatalogTimeOuts
    {
        public static TimeSpan EmailTimeOut = TimeSpan.FromMinutes(5);
        public static TimeSpan CatalogSearchTimeOut = TimeSpan.FromMinutes(5);
        public static TimeSpan AlertTimeOut = TimeSpan.FromMinutes(1);
        public static TimeSpan DefaultTimeOut = TimeSpan.FromMinutes(2);
    }

    public enum Region
    {
        US,
        EMEA
    }
}
