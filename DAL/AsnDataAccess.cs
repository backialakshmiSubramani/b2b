using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Reflection;


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


                foreach (var asnQueue in results)
                {
                    foreach (var prop in asnQueue.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                    {
                        var value = prop.GetValue(asnQueue, new object[] { });
                        Console.Write("{0} = {1} \t", prop.Name, value);
                    }

                    Console.WriteLine();
                }

                return results.ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to get DPID from DB. Exception Message: {0}", e.Message);
                return null;
            }
        }

        public static List<string> FetchItemId(string poNumber)
        {
            try
            {
                asnDatamodelDataContext = new AsnDatamodelDataContext(asnConnectionString);

                var results = from im in asnDatamodelDataContext.ASNItemMappings
                              where
                                  (from aq in asnDatamodelDataContext.ASNQueues
                                   where aq.PONumber == poNumber
                                   select aq.DocId).Contains((int)im.DocId)
                              select im;

                foreach (var asnItemMapping in results)
                {
                    foreach (var prop in asnItemMapping.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                    {
                        var value = prop.GetValue(asnItemMapping, new object[] { });
                        Console.Write("{0} = {1} \t", prop.Name, value);
                    }

                    Console.WriteLine();
                }

                return results.Select(im => im.FulfillmentItemId.ToString()).ToList();
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
                              select om;

                foreach (var asnOrderMapping in results)
                {
                    foreach (var prop in asnOrderMapping.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                    {
                        var value = prop.GetValue(asnOrderMapping, new object[] { });
                        Console.Write("{0} = {1} \t", prop.Name, value);
                    }

                    Console.WriteLine();
                }

                return results.Select(om => om.OrderNumber).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to fetch Backend Order Number from DB. Exception Message: {0}", e.Message);
                return null;
            }
        }

        public static List<POLine> FetchPurchaseOrderDetails(string poNumber, out List<PurchaseOrder> listOfPurchaseOrder)
        {
            try
            {
                asnDatamodelDataContext = new AsnDatamodelDataContext(asnConnectionString);

                var purchaseOrderDetails = (from pos in asnDatamodelDataContext.PurchaseOrders
                    where pos.PONumber == poNumber
                    select pos);

                listOfPurchaseOrder = purchaseOrderDetails.ToList();

                var poLineDetails = (from polines in asnDatamodelDataContext.POLines
                    where polines.PurchaseOrder_PurchaseOrderId == purchaseOrderDetails.FirstOrDefault().PurchaseOrderId
                    select polines);


                Console.WriteLine("PurchaseOrders Table Content Start");
                foreach (var purchaseOrder in purchaseOrderDetails)
                {
                    foreach (var prop in purchaseOrder.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                    {
                        var value = prop.GetValue(purchaseOrder, new object[] { });
                        Console.Write("{0} = {1} \t", prop.Name, value);
                    }

                    Console.WriteLine();
                }
                Console.WriteLine("PurchaseOrders Table Content End");

                Console.WriteLine("POLines Table Content Start");


                foreach (var poLine in poLineDetails)
                {
                    foreach (var prop in poLine.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                    {
                        var value = prop.GetValue(poLine, new object[] { });
                        Console.Write("{0} = {1} \t", prop.Name, value);
                    }

                    Console.WriteLine();
                }
                Console.WriteLine("POLines Table Content End");


                return poLineDetails.ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to fetch Backend Order Number from DB. Exception Message: {0}", e.Message);
                listOfPurchaseOrder = null;
                return null;
            }
        }

        /// <summary>
        /// to get Purchase Order Details based on PONumber
        /// </summary>
        /// <param name="poNumber"></param>
        /// <returns></returns>
        public static List<PurchaseOrder> GetPurchaseOrdersDetailsBasedOnPoNumber(string poNumber)
        {
            try
            {
                asnDatamodelDataContext = new AsnDatamodelDataContext(asnConnectionString);

                var purchaseOrderDetails = (from pos in asnDatamodelDataContext.PurchaseOrders
                                            where pos.PONumber == poNumber
                                            select pos);

                return purchaseOrderDetails.ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to get DPID from DB. Exception Message: {0}", e.Message);
                return null;
            }
        }

        /// <summary>
        /// to get PO Line Details based on PONumber
        /// </summary>
        /// <param name="poNumber"></param>
        /// <returns></returns>
        public static List<POLine> GetLineDetailsBasedOnPoNumber(string poNumber)
        {
            try
            {
                asnDatamodelDataContext = new AsnDatamodelDataContext(asnConnectionString);

                var purchaseOrderDetails = (from pos in asnDatamodelDataContext.PurchaseOrders
                                            where pos.PONumber == poNumber
                                            select pos);

                var poLineDetails = (from polines in asnDatamodelDataContext.POLines
                                     where polines.PurchaseOrder_PurchaseOrderId == purchaseOrderDetails.FirstOrDefault().PurchaseOrderId
                                     select polines);

                return poLineDetails.ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to get DPID from DB. Exception Message: {0}", e.Message);
                return null;
            }
        }

        /// <summary>
        /// to get Order Details based on PONumber
        /// </summary>
        /// <param name="poNumber"></param>
        /// <returns></returns>
        public static List<Order> GetOrderDetailsBasedOnPoNumber(string poNumber)
        {
            try
            {
                asnDatamodelDataContext = new AsnDatamodelDataContext(asnConnectionString);

                var purchaseOrderDetails = (from pos in asnDatamodelDataContext.PurchaseOrders
                                            where pos.PONumber == poNumber
                                            select pos);

                var poLineDetails = (from polines in asnDatamodelDataContext.POLines
                                     where polines.PurchaseOrder_PurchaseOrderId == purchaseOrderDetails.FirstOrDefault().PurchaseOrderId
                                     select polines);

                var orderDetails = (from orders in asnDatamodelDataContext.Orders
                                    where orders.POLine_POLineId == poLineDetails.FirstOrDefault().POLineId
                                    orderby orders.OrderNumber  
                                    select orders);

                return orderDetails.ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to get DPID from DB. Exception Message: {0}", e.Message);
                return null;
            }
        }

        /// <summary>
        /// to get Ship Details based on PONumber
        /// </summary>
        /// <param name="poNumber"></param>
        /// <returns></returns>
        public static List<ShippingInfo> GetShipDetailsBasedOnOrderNumber(string orderId)
        {
            try
            {
                asnDatamodelDataContext = new AsnDatamodelDataContext(asnConnectionString);

                var shipDetails = (from shipinfo in asnDatamodelDataContext.ShippingInfos
                                   where shipinfo.Order_OrderId.ToString() == orderId
                                   select shipinfo);

                return shipDetails.ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to get DPID from DB. Exception Message: {0}", e.Message);
                return null;
            }
        }

        /// <summary>
        /// to get Ship Details based on OrderId
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public static int? GetNumberOfUnitsShippedBasedOnOrderNumber(string orderId)
        {
            try
            {
                asnDatamodelDataContext = new AsnDatamodelDataContext(asnConnectionString);

                var numberOfUnitsShipped = (from shipinfo in asnDatamodelDataContext.ShippingInfos
                                            where shipinfo.Order_OrderId.ToString() == orderId
                                            select shipinfo);

                int? totalNumberOfUnitsShipped = numberOfUnitsShipped.Sum(s => s.NumberofUnitsShipped);

                if (totalNumberOfUnitsShipped != null)
                    return totalNumberOfUnitsShipped;
                else
                    return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to get DPID from DB. Exception Message: {0}", e.Message);
                return 0;
            }
        }
    }
}
