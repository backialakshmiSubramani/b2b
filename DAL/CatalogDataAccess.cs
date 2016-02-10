using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modules.Channel.B2B.DAL.ChannelCatalog;
using Modules.Channel.B2B.Common;

namespace Modules.Channel.B2B.DAL
{
    public class ChannelCatalogProdDataAccess
    {
        public ChannelCatalogProdDataAccess()
        { }

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

        public static CatalogMaster_Auto GetCatalog(string identityName, DateTime anyTimeAfter)
        {
            List<CatalogMaster_Auto> catalogEntities = null;

            using (ChannelCatalogProdEntities entities = new ChannelCatalogProdEntities())
            {
                catalogEntities = (from c in entities.CatalogMaster_Auto
                                   where c.User_Name == identityName
                                   && c.CreateDate > anyTimeAfter
                                   select c).ToList();
            }

            return catalogEntities.FirstOrDefault();
        }
    }

    public class ChannelCatalogPrevDataAccess
    {
        public ChannelCatalogPrevDataAccess()
        { }

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

        public static IEnumerable<CatalogMaster_Auto> GetCatalog(Guid identityGuid, DateTime anyTimeAfter)
        {
            IEnumerable<CatalogMaster_Auto> catalogEntities = null;

            using (ChannelCatalogPrevEntities entities = new ChannelCatalogPrevEntities())
            {
                catalogEntities = (from c in entities.CatalogMaster_Auto
                                   where c.IdentityId == identityGuid
                                   && c.CreateDate > anyTimeAfter
                                   select c);
            }

            return catalogEntities;
        }
    }
}
