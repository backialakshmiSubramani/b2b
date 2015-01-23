using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Modules.Channel.B2B.DAL
{
    public class AsnDataAccess
    {
        private static AsnDatamodelDataContext asnDatamodelDataContext;

        private static string asnConnectionString = ConfigurationManager.AppSettings["AsnConnectionString"];

        public AsnDataAccess()
        {
            asnDatamodelDataContext = new AsnDatamodelDataContext(asnConnectionString);
        }

        public static List<ASNQueue> FetchRecordsFromAsnQueue(string poNumber)
        {
            try
            {
                asnDatamodelDataContext = new AsnDatamodelDataContext(asnConnectionString);

                var results = from aq in asnDatamodelDataContext.ASNQueues
                              where aq.PONumber == poNumber
                              select aq;

                return results.ToList();

                //return row != null ? row.DPID : string.Empty;
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to get DPID from DB. Exception Message: {0}", e.Message);
                return null;
            }
        }

        public static List<Guid?> FetchItemId(string poNumber)
        {
            try
            {
                asnDatamodelDataContext = new AsnDatamodelDataContext(asnConnectionString);

                var results = from im in asnDatamodelDataContext.ASNItemMappings
                              where
                                  (from aq in asnDatamodelDataContext.ASNQueues
                                   where aq.PONumber == poNumber
                                   select aq.DocId).Contains((int)im.DocId)
                              select im.FulfillmentItemId;

                return results.ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(
                    "Failed to fetch FulfillmentItemId from DB. Exception Message: {0} \n InnerException: {1} \n StackTrace {2}",
                    e.Message, e.InnerException, e.StackTrace);
                return null;
            }
        }

        public static List<string> FetchBackendOrderNumber(string poNumber)
        {
            try
            {
                asnDatamodelDataContext = new AsnDatamodelDataContext(asnConnectionString);

                var results = from om in asnDatamodelDataContext.ASNOrderMappings
                              where (from im in asnDatamodelDataContext.ASNItemMappings
                                     where
                                         (from aq in asnDatamodelDataContext.ASNQueues
                                          where aq.PONumber == poNumber
                                          select aq.DocId).Contains((int)im.DocId)
                                     select im.MapItemId).Contains((int)om.MapItemId)
                              select om.OrderNumber;

                return results.ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to fetch Backend Order Number from DB. Exception Message: {0}", e.Message);
                return null;
            }
        }
    }
}
