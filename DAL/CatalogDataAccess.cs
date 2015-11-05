using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modules.Channel.B2B.DAL.ChannelCatalog;

namespace Modules.Channel.B2B.DAL
{
    public class ChannelCatalogProdDataAccess
    {
        public ChannelCatalogProdDataAccess()
        {
        }

        public static Channel_Catalog_PackagingData GetPackagingDetails(string orderCode)
        {
            Channel_Catalog_PackagingData packagingData = null;

            using (ChannelCatalogProdEntities entities = new ChannelCatalogProdEntities())
            {
                packagingData = (from c in entities.Channel_Catalog_PackagingData
                                 where c.OrderCode == orderCode
                                 select c).First();
            }

            return packagingData;
        }
    }

    public class ChannelCatalogPrevDataAccess
    {
        public ChannelCatalogPrevDataAccess()
        {
        }

        public static Channel_Catalog_PackagingData GetPackagingDetails(string orderCode)
        {
            Channel_Catalog_PackagingData packagingData = null;

            using (ChannelCatalogPrevEntities entities = new ChannelCatalogPrevEntities())
            {
                packagingData = (from c in entities.Channel_Catalog_PackagingData
                                 where c.OrderCode == orderCode
                                 select c).First();
            }

            return packagingData;
        }
    }
}
