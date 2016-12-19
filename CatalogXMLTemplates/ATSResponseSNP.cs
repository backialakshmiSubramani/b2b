using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Channel.B2B.CatalogXMLTemplates
{
    public class ATSResponseSNP
    {
        public List<SkuList> SkuList { get; set; }
        public string CountryCode { get; set; }
    }

    public class SkuList
    {
        public string Inventory { get; set; }
        public string LeadTime { get; set; }
        public string  Sku { get; set; }
    }
}
