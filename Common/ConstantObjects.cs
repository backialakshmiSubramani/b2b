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
    public enum ConfigRules
    {
        None=0,
        LeadTimeOff=1,
        LeadTimeON=2,
        DuplicateBPN=3, 
        NullBPN=4,
    }

    public enum ConfigAdditionalOptions
    {
        None=0,
        IncludeDefaultOptions=1
    }
    public enum DefaultOptions
    {
        On = 0,
        Off = 1
    }
    public enum SystemsAdditionalOptions
    {
        None = 0,
        IncludeDefaultOptions = 1
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
        EMEA,
        APJ
    }
    
    public enum CatalogStatus
    {
        Created,
        Published,
        Failed,
        Expired,
        Scheduled
    }

    public enum CRTStatus
    {
        ON,
        OFF
    };

}
