using DCSG.ADEPT.Framework;
using DCSG.ADEPT.Framework.Core;
using DCSG.ADEPT.Framework.Core.Extensions.Locators;
using DCSG.ADEPT.Framework.Core.Extensions.WebDriver;
using DCSG.ADEPT.Framework.Core.Extensions.WebElement;
using DCSG.ADEPT.Framework.Core.Page;
using Microsoft.SharePoint.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modules.Channel.B2B.Common;
using Modules.Channel.B2B.Core.Pages;
using Modules.Channel.B2B.DAL;
using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using Excel = Microsoft.Office.Interop.Excel;

namespace Modules.Channel.B2B.Core.Workflows.Common
{
    public class PoOperations
    {
        private IWebDriver webDriver;
        private ArrayList crtDetails;

        public PoOperations(IWebDriver driver)
        {
            this.webDriver = driver;
            crtDetails = new ArrayList();
        }

        private B2BHomePage B2BHomePage
        {
            get
            {
                return new B2BHomePage(webDriver);
            }
        }

        private B2BQaToolsPage B2BQaToolsPage
        {
            get
            {
                return new B2BQaToolsPage(webDriver);
            }
        }

        private B2BLogReportPage B2BLogReportPage
        {
            get
            {
                return new B2BLogReportPage(webDriver);
            }
        }

        private B2BLogDetailPage B2BLogDetailPage
        {
            get
            {
                return new B2BLogDetailPage(webDriver);
            }
        }

        private B2BPoViewerPage B2BPoViewerPage
        {
            get
            {
                return new B2BPoViewerPage(webDriver);
            }
        }

        private B2BQuoteViewerPage B2BQuoteViewerPage
        {
            get
            {
                return new B2BQuoteViewerPage(webDriver);
            }
        }

        private GcmMainPage GcmMainPage
        {
            get
            {
                return new GcmMainPage(webDriver);
            }
        }

        private GcmFindEOrderPage GcmFindEOrderPage
        {
            get
            {
                return new GcmFindEOrderPage(webDriver);
            }
        }

        private GcmOrderGroupLogPage GcmOrderGroupLogPage
        {
            get
            {
                return new GcmOrderGroupLogPage(webDriver);
            }
        }

        private GcmEmailFaxViewPage GcmEmailFaxViewPage
        {
            get
            {
                return new GcmEmailFaxViewPage(webDriver);
            }
        }

        private GcmOrderGroupSummaryPage GcmOrderGroupSummaryPage
        {
            get
            {
                return new GcmOrderGroupSummaryPage(webDriver);
            }
        }

        private GcmXmlResultsPage GcmXmlResultsPage
        {
            get
            {
                return new GcmXmlResultsPage(webDriver);
            }
        }

        private GcmCustomerContactsPage GcmCustomerContactsPage
        {
            get
            {
                return new GcmCustomerContactsPage(webDriver);
            }
        }

        private GcmEndUserInfoPage GcmEndUserInfoPage
        {
            get
            {
                return new GcmEndUserInfoPage(webDriver);
            }
        }

        public bool VerifyEudcPoCreation(
            string poNumber,
            RunEnvironment environment,
            string crtFilePath,
            string crtEndUserId,
            string gcmUrl,
            string baseItemPrice,
            string expectedDpidMessage,
            string expectedPurchaseOderMessage)
        {
            this.GetCrtDetails(crtFilePath, crtEndUserId);

            if (!SearchPoInLogReportPage(poNumber))
            {
                return false;
            }

            // 2. Click on PONumber, PO Viewer page viewed. Verify the enduser details against the CRT File data.
            B2BLogReportPage.ClickPoNumberInTable();
            if (!CheckEndUserInfo(B2BPoViewerPage.GetEndUserDetails()))
            {
                Console.WriteLine("PO Viewer Page does not contain End User Info. Workflow {0}", Workflow.Eudc);
                return false;
            }

            webDriver.Navigate().Back();
            webDriver.WaitForPageLoad(new TimeSpan(0, 0, 10));

            // 3. "CBL PO: sendPurchaseOrder" Message, click on the link,verify the enduser details against the CRT file data.
            if (!B2BLogReportPage.FindMessageAndGoToLogDetailPage(expectedPurchaseOderMessage))
            {
                return false;
            }

            var logDetail = B2BLogDetailPage.GetEndUserDetailsFromLogDetail();

            if (!CheckEndUserInfo(logDetail))
            {
                Console.WriteLine("Log Detail does not contain End User Info. Workflow {0}", Workflow.Eudc);
                return false;
            }

            B2BLogDetailPage.ReturnToLogReport();

            // 4. "Continue Purchase Order: Purchase Order Success: <DPID>" - capture DPID & verify in GCM
            var dellPurchaseId = B2BLogReportPage.FindDellPurchaseId(expectedDpidMessage);

            if (string.IsNullOrEmpty(dellPurchaseId))
            {
                if (environment == RunEnvironment.Production)
                {
                    Console.WriteLine("DP Id is not generated in Production Environment. Stopping the test");
                    return false;
                }

                Console.WriteLine("DP Id is not generated. Stopping verfication. Not continuing with GCM verification");
                return true;
            }

            Console.WriteLine("Starting GCM verification");
            if (!VerifyOrderStatusInGcm(gcmUrl, dellPurchaseId))
            {
                return false;
            }

            // GCM verification for EUDC
            if (!GcmVerificationsForEudc(dellPurchaseId, baseItemPrice))
            {
                return false;
            }

            return true;
        }

        public bool SubmitXmlForPoCreation(string poXml, string environment, string targetUrl, out string poNumber)
        {
            B2BQaToolsPage.ClickLocationEnvironment(environment);
            B2BQaToolsPage.ClickLocationEnvironmentLink(environment);
            B2BQaToolsPage.PasteTargetUrl(targetUrl);
            B2BQaToolsPage.PasteInputXml(poXml);
            B2BQaToolsPage.ClickSubmitMessage();

            var submissionResult = B2BQaToolsPage.GetSubmissionResult();
            Console.WriteLine("Submission Result is: " + submissionResult);

            if (!submissionResult.Contains("200"))
            {
                Console.WriteLine("The status is not 200. The submission result: {0} ", submissionResult);
                poNumber = string.Empty;
                return false;
            }

            poNumber = submissionResult.Split(' ').Last();
            Console.WriteLine("PO Created: " + poNumber);
            return true;
        }

        /// <summary>
        /// Verification points for Sprint16_563614_P1 & Sprint16_563614_P2
        /// </summary>
        /// <param name="poNumber"></param>
        /// <param name="quoteRetrievedMessagePrefix"></param>
        /// <param name="quoteRetrievedMessageSuffix"></param>
        /// <param name="enteringMasterOrderGroupMessage"></param>
        /// <param name="itemDescription"></param>
        /// <param name="quantity"></param>
        /// <param name="unitPrice"></param>
        /// <returns></returns>
        public bool VerifyQmsQuoteCreation(
            string poNumber,
            string quoteRetrievedMessagePrefix,
            string quoteRetrievedMessageSuffix,
            string enteringMasterOrderGroupMessage,
            List<QuoteDetail> listOfQuoteDetail)
        {
            if (!SearchPoInLogReportPage(poNumber))
            {
                return false;
            }

            if (!B2BLogReportPage.FindQuoteRetrievalMessage(quoteRetrievedMessagePrefix, quoteRetrievedMessageSuffix))
            {
                return false;
            }

            var quoteDetail = listOfQuoteDetail.FirstOrDefault();

            B2BLogReportPage.FindMessageAndGoToQuoteViewerPage(enteringMasterOrderGroupMessage);
            return B2BQuoteViewerPage.CheckItemDetails(quoteDetail.ItemDescription, quoteDetail.Quantity, quoteDetail.Price);
        }

        /// <summary>
        /// Verification points for Sprint16_570456_P1 & Sprint16_570456_P2
        /// </summary>
        /// <param name="mapperRequestMessage"></param>
        public bool VerifyMapperRequestXmlDataInLogDetailPage(string poNumber, string mapperRequestMessage)
        {
            if (!SearchPoInLogReportPage(poNumber))
            {
                return false;
            }

            if (!B2BLogReportPage.FindMessageAndGoToLogDetailPage(mapperRequestMessage))
            {
                return false;
            }

            var poLineItems = B2BLogDetailPage.GetPoLineItemsFromMapperRequestXml();
            return VerifyMapperRequestDetailsAgainstPoTemplate(poLineItems);
        }

        /// <summary>
        /// Verification points for Sprint17_565299_P1 & Sprint17_565299_P2
        /// </summary>
        /// <param name="asnLogEventMessages"></param>
        /// <param name="asnLogDetailMessages"></param>
        /// <returns></returns>
        public bool VerifyMappingEntriesForChannelAsnEnabledProfile(string poNumber, List<string> asnLogEventMessages, List<string> asnLogDetailMessages, string mapperRequestMessage)
        {
            if (!SearchPoInLogReportPage(poNumber))
            {
                return false;
            }

            for (var i = 0; i < asnLogEventMessages.Count; i++)
            {
                if (!B2BLogReportPage.FindMessageAndGoToLogDetailPage(asnLogEventMessages[i]))
                {
                    return false;
                }

                if (!B2BLogDetailPage.GetLogDetail().Equals(asnLogDetailMessages[i]))
                {
                    return false;
                }

                B2BLogDetailPage.ReturnToLogReport();
            }

            if (!B2BLogReportPage.FindMessageAndGoToLogDetailPage(mapperRequestMessage))
            {
                return false;
            }

            try
            {
                var mapperXml = XDocument.Parse(B2BLogDetailPage.GetLogDetail());
                webDriver.Navigate().Back();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool VerifyExceptionLoggingForAsn(string asnErrorMessage)
        {
            return B2BLogReportPage.FindMessageOnLogReport(asnErrorMessage);
        }

        /// <summary>
        /// Verification points for Sprint18_P1 & Sprint18_P2 --> 532578
        /// </summary>
        /// <param name="poNumber"></param>
        /// <param name="expectedDpidMessage"></param>
        /// <param name="mapperRequestMessage"></param>
        /// <param name="ogXmlMessage"></param>
        /// <param name="gcmUrl"></param>
        /// <returns></returns>
        public bool VerifyFulfillmentUnits(string poNumber, string expectedDpidMessage, string mapperRequestMessage, string ogXmlMessage, string gcmUrl)
        {
            if (!SearchPoInLogReportPage(poNumber))
            {
                return false;
            }

            var dellPurchaseId = B2BLogReportPage.FindDellPurchaseId(expectedDpidMessage);
            if (string.IsNullOrEmpty(dellPurchaseId))
            {
                Console.WriteLine("Dpid not found. Stopping verification");
                return false;
            }

            ((IJavaScriptExecutor)this.webDriver).ExecuteScript("window.open();");
            var parentWindowHandle = this.webDriver.CurrentWindowHandle;
            var gcmWindow = webDriver.WindowHandles.LastOrDefault();
            webDriver.SwitchTo().Window(gcmWindow);
            webDriver.Manage().Window.Maximize();

            if (!VerifyOrderStatusInGcm(gcmUrl, dellPurchaseId))
            {
                return false;
            }

            webDriver.Close();
            webDriver.SwitchTo().Window(parentWindowHandle);

            if (!B2BLogReportPage.FindMessageAndGoToLogDetailPage(ogXmlMessage))
            {
                return false;
            }

            XDocument ogXml = XDocument.Parse(B2BLogDetailPage.GetLogDetail());

            var itemIds =
                ogXml.XPathSelectElements("//OrderGroup/OrderForms/OrderForm/Items/Item/Id").Select(e => e.Value);

            if (!itemIds.Any())
            {
                Console.WriteLine("Item Id not found in <Items> node");
                return false;
            }

            Console.WriteLine("Item Ids from <Items> node are \n");
            foreach (var itemId in itemIds)
            {
                Console.WriteLine(itemId + "\n");
            }

            B2BLogDetailPage.ReturnToLogReport();

            if (!B2BLogReportPage.FindMessageAndGoToLogDetailPage(mapperRequestMessage))
            {
                return false;
            }

            var itemIdsFromMapperRequestXml = B2BLogDetailPage.GetItemIdsFromMapperRequestXml().Select(e => e.Value);

            if (!itemIdsFromMapperRequestXml.Any() || !itemIds.All(i => itemIdsFromMapperRequestXml.Contains(i)))
            {
                Console.WriteLine("Item Ids fetched from <Items> node are not present in Mapper Request Xml");
                return false;
            }

            Console.WriteLine("All the Item Ids fetched from <Items> node are present in Mapper Request Xml");

            var itemIdsFromFulfillment =
                ogXml.XPathSelectElements(
                    "//OrderGroup/OrderForms/OrderForm/FulfillmentUnits/FulfillmentUnit/FulfillmentItemInformation/FulfillmentItemInformation/ItemId")
                    .Select(e => e.Value);

            if (!itemIdsFromFulfillment.Any() || !itemIdsFromFulfillment.All(i => itemIdsFromMapperRequestXml.Contains(i)))
            {
                Console.WriteLine("Item Ids fetched from <FulfillmentItemInformation> node are not present in Mapper Request Xml");
                return false;
            }

            Console.WriteLine("All the Item Ids fetched from <FulfillmentItemInformation> node are present in Mapper Request Xml");

            var itemIdsFromDb = AsnDataAccess.FetchItemId(poNumber).Select(i => i.ToString());

            if (!itemIdsFromDb.Any() || !itemIdsFromFulfillment.All(i => itemIdsFromDb.Contains(i)))
            {
                Console.WriteLine("Item Ids not found in ASNItemMapping Table for PO '{0}'.", poNumber);
                return false;
            }

            Console.WriteLine("All the Item Ids fetched from <FulfillmentItemInformation> node are found in ASNItemMapping Table");
            return true;
        }

        /// <summary>
        /// Verification points for Sprint18_P1 & Sprint18_P2 --> 532584
        /// </summary>
        /// <param name="expectedDpidMessage"></param>
        /// <param name="mapperRequestMessage"></param>
        /// <returns></returns>
        public bool VerifyDpidInMapperXmlAndDb(string poNumber, string expectedDpidMessage, string mapperRequestMessage)
        {
            if (!SearchPoInLogReportPage(poNumber))
            {
                return false;
            }

            var dellPurchaseId = B2BLogReportPage.FindDellPurchaseId(expectedDpidMessage);
            if (string.IsNullOrEmpty(dellPurchaseId))
            {
                Console.WriteLine("Dpid not found. Stopping verification");
                return false;
            }

            if (!B2BLogReportPage.FindMessageAndGoToLogDetailPage(mapperRequestMessage))
            {
                return false;
            }

            if (!B2BLogDetailPage.GetDpidFromMapperRequestXml().Equals(dellPurchaseId))
            {
                return false;
            }

            var firstRow = AsnDataAccess.FetchRecordsFromAsnQueue(poNumber).FirstOrDefault();

            return firstRow != null && firstRow.DPID.Equals(dellPurchaseId);
        }

        /// <summary>
        /// Verification points for Sprint18_P1 & Sprint18_P2 --> 562947
        /// </summary>
        /// <param name="poNumber"></param>
        /// <param name="expectedDpidMessage"></param>
        /// <param name="mapperRequestMessage"></param>
        /// <returns></returns>
        public bool MatchValuesInPoXmlAndMapperXml(
            string poNumber,
            string expectedDpidMessage,
            string mapperRequestMessage)
        {
            if (!SearchPoInLogReportPage(poNumber))
            {
                return false;
            }

            var threadId = B2BLogReportPage.GetThreadId();
            var dellPurchaseId = B2BLogReportPage.FindDellPurchaseId(expectedDpidMessage);
            if (string.IsNullOrEmpty(dellPurchaseId))
            {
                Console.WriteLine("Dpid not found. Stopping verification");
                return false;
            }

            if (!B2BLogReportPage.FindMessageAndGoToLogDetailPage(mapperRequestMessage))
            {
                return false;
            }

            var mapperXml = XDocument.Parse(B2BLogDetailPage.GetLogDetail());
            var savedPoCbl = XDocument.Load("CblTemplate.xml");
            var asnQueueEntry = AsnDataAccess.FetchRecordsFromAsnQueue(poNumber).FirstOrDefault();

            if (!asnQueueEntry.DPID.Equals(dellPurchaseId))
            {
                return false;
            }

            if (!mapperXml.XPathSelectElement("//ThreadId").Value.Equals(threadId))
            {
                return false;
            }

            if (!asnQueueEntry.ThreadId.Equals(threadId))
            {
                return false;
            }

            if (
                !mapperXml.XPathSelectElement("//PONumber")
                     .Value.Equals(savedPoCbl.XPathSelectElement("//BuyerRefNum/Reference/RefNum").Value))
            {
                return false;
            }

            if (
                !mapperXml.XPathSelectElement("//Partner")
                     .Value.ToUpper()
                     .Equals(
                         savedPoCbl.XPathSelectElement("//BuyerParty/Party/ListOfIdentifier/Identifier/Agency")
                     .Attribute("AgencyOther")
                     .Value.ToUpper()))
            {
                return false;
            }

            if (!asnQueueEntry.Partner.ToUpper().Equals(mapperXml.XPathSelectElement("//Partner").Value.ToUpper()))
            {
                return false;
            }

            if (
                !mapperXml.XPathSelectElements("//LineItems/MapperRequestPOLine/VendorPartNumber")
                     .FirstOrDefault()
                     .Value.Equals(
                         savedPoCbl.XPathSelectElements(
                             "//ListOfOrderDetail/OrderDetail/BaseItemDetail/ManufacturerPartNum/PartNum/PartID")
                     .FirstOrDefault()
                     .Value))
            {
                return false;
            }

            if (
                !asnQueueEntry.VendorPartNumber.Equals(
                    mapperXml.XPathSelectElements("//LineItems/MapperRequestPOLine/VendorPartNumber")
                     .FirstOrDefault()
                     .Value))
            {
                return false;
            }

            if (
                !mapperXml.XPathSelectElements("//LineItems/MapperRequestPOLine/BuyerSKU")
                     .FirstOrDefault()
                     .Value.Equals(
                         savedPoCbl.XPathSelectElements(
                             "//ListOfOrderDetail/OrderDetail/BaseItemDetail/BuyerPartNum/PartNum/PartID")
                     .FirstOrDefault()
                     .Value))
            {
                return false;
            }

            if (
                !asnQueueEntry.BuyerSKU.Equals(
                    mapperXml.XPathSelectElements("//LineItems/MapperRequestPOLine/BuyerSKU").FirstOrDefault().Value))
            {
                return false;
            }

            Console.WriteLine("Delivery Preference is: {0}", mapperXml.XPathSelectElement("//DeliveryPreference").Value);

            if (!asnQueueEntry.DeliveryPreference.ToString().Equals(mapperXml.XPathSelectElement("//DeliveryPreference").Value))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Verification points for Sprint19_P1 & Sprint19_P2 ---> 563059
        /// </summary>
        /// <param name="poNumber"></param>
        /// <param name="ogXmlMessage"></param>
        /// <param name="mapperRequestMessage"></param>
        /// <returns></returns>
        public bool MatchItemIdInOgXmlAndMapperRequestAndDb(
            string poNumber,
            string ogXmlMessage,
            string mapperRequestMessage)
        {
            if (!this.SearchPoInLogReportPage(poNumber))
            {
                return false;
            }

            if (!B2BLogReportPage.FindMessageAndGoToLogDetailPage(ogXmlMessage))
            {
                return false;
            }

            XDocument ogXml = XDocument.Parse(B2BLogDetailPage.GetLogDetail());

            B2BLogDetailPage.ReturnToLogReport();

            var itemId =
                ogXml.XPathSelectElements("//OrderGroup/OrderForms/OrderForm/Items/Item/Id").FirstOrDefault();

            if (itemId == null)
            {
                Console.WriteLine("Item Id not found in <Items> node");
                return false;
            }

            var itemIdsFromFulfillment =
                ogXml.XPathSelectElements(
                    "//OrderGroup/OrderForms/OrderForm/FulfillmentUnits/FulfillmentUnit/FulfillmentItemInformation/FulfillmentItemInformation/ItemId");

            if ((!itemIdsFromFulfillment.Any()) || (!itemIdsFromFulfillment.Any(i => i.Value.Equals(itemId.Value))))
            {
                Console.WriteLine("Item Id {0} not found in Fulfillment Information", itemId.Value);
                return false;
            }

            if (!B2BLogReportPage.FindMessageAndGoToLogDetailPage(mapperRequestMessage))
            {
                return false;
            }

            var itemIdsFromMapperRequestXml = B2BLogDetailPage.GetItemIdsFromMapperRequestXml();

            if ((!itemIdsFromMapperRequestXml.Any()) || (!itemIdsFromMapperRequestXml.Any(i => i.Value.Equals(itemId.Value))))
            {
                Console.WriteLine("Item Id {0} not found in Mapper Request Xml", itemId.Value);
                return false;
            }

            var itemIdsFromDb = AsnDataAccess.FetchItemId(poNumber);

            if ((!itemIdsFromDb.Any()) || (!itemIdsFromDb.Any(i => i.ToString().Equals(itemId.Value))))
            {
                Console.WriteLine("Item Id '{0}' not found in ASNItemMapping Table for PO '{1}'.", itemId.Value, poNumber);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Verification points for Sprint19_P1 & Sprint19_P2 ---> 563633
        /// </summary>
        /// <param name="poNumber"></param>
        /// <param name="ogXmlMessage"></param>
        /// <param name="gcmUrl"></param>
        /// <param name="expectedDpidMessage"></param>
        /// <returns></returns>
        public bool CaptureBackendOrderNumberFromOgXmlAndDb(
            string poNumber,
            string ogXmlMessage,
            string gcmUrl,
            string expectedDpidMessage)
        {
            if (!this.SearchPoInLogReportPage(poNumber))
            {
                return false;
            }

            if (!B2BLogReportPage.FindMessageAndGoToLogDetailPage(ogXmlMessage))
            {
                return false;
            }

            XDocument ogXml = XDocument.Parse(B2BLogDetailPage.GetLogDetail());

            B2BLogDetailPage.ReturnToLogReport();

            var itemId = ogXml.XPathSelectElements("//OrderGroup/OrderForms/OrderForm/Items/Item/Id").FirstOrDefault();

            if (itemId == null)
            {
                Console.WriteLine("Item Id not found in <Items> node");
                return false;
            }

            var itemIdsFromFulfillment =
                ogXml.XPathSelectElements(
                    "//OrderGroup/OrderForms/OrderForm/FulfillmentUnits/FulfillmentUnit/FulfillmentItemInformation/FulfillmentItemInformation/ItemId");

            if ((!itemIdsFromFulfillment.Any()) || (!itemIdsFromFulfillment.Any(i => i.Value.Equals(itemId.Value))))
            {
                Console.WriteLine("Item Id {0} not found in Fulfillment Information", itemId.Value);
                return false;
            }

            var dellPurchaseId = B2BLogReportPage.FindDellPurchaseId(expectedDpidMessage);
            if (string.IsNullOrEmpty(dellPurchaseId))
            {
                Console.WriteLine("Dpid not found. Stopping verification");
                return false;
            }

            Console.WriteLine("Starting GCM verification");
            if (!VerifyOrderStatusInGcm(gcmUrl, dellPurchaseId))
            {
                return false;
            }

            GcmFindEOrderPage.ClickViewButton(dellPurchaseId);
            GcmOrderGroupLogPage.GoToOrderGroupSummaryPage();
            GcmOrderGroupSummaryPage.GoToXmlResultsPage();
            var gcmOgXml = GcmXmlResultsPage.GetOgXml();

            var fulfillmentUnits = gcmOgXml.XPathSelectElements("//OrderGroup/OrderForms/OrderForm/FulfillmentUnits");

            var fulfillmentUnit = (from ffu in fulfillmentUnits.Elements("FulfillmentUnit")
                                   where
                                       ffu.XPathSelectElements(
                                           "//FulfillmentItemInformation/FulfillmentItemInformation/ItemId")
                                       .Any(ii => ii.Value == itemId.Value)
                                   select ffu).FirstOrDefault();
            var orderNumberElement =
                fulfillmentUnit.XPathSelectElements(
                    "//BackendResponses/BackendResponse/BackendOrders/BackendOrder/OrderNum").FirstOrDefault();

            if (orderNumberElement == null)
            {
                Console.WriteLine("Order number not found in OG Xml for Item Id: {0}", itemId.Value);
            }

            var orderNumber = orderNumberElement.Value;
            Console.WriteLine("Backend Order Number from OGXml for Item Id '{0}' is {1}", itemId.Value, orderNumber);

            var orderNumbersFromDb = AsnDataAccess.FetchBackendOrderNumber(poNumber);

            if ((!orderNumbersFromDb.Any()) || (!orderNumbersFromDb.Any(i => i.Equals(orderNumber))))
            {
                Console.WriteLine(
                    "Backend Order Number '{0}' not found in ASNOrderMapping Table for Item Id '{1}' for PO '{2}'.",
                    orderNumber,
                    itemId.Value,
                    poNumber);
                return false;
            }

            return true;
        }

        private void GetCrtDetails(string crtFilePath, string crtEndUserId)
        {
            var excelApplication = new Excel.Application();

            Excel.Workbook workbook =
                excelApplication.Workbooks.Open(
                    System.IO.Directory.GetCurrentDirectory() + "\\" + crtFilePath,
                    Type.Missing,
                    true,
                    Type.Missing,
                    Type.Missing,
                    Type.Missing,
                    Type.Missing,
                    Type.Missing,
                    Type.Missing,
                    Type.Missing,
                    Type.Missing,
                    Type.Missing,
                    Type.Missing,
                    Type.Missing,
                    Type.Missing);

            var worksheet = (Excel.Worksheet)workbook.Worksheets.Item[1];
            ////worksheet.Activate();
            ((Excel._Worksheet)worksheet).Activate();

            for (var i = 2; ; i++)
            {
                if (worksheet.Cells[i, 1].Text == crtEndUserId)
                {
                    crtDetails.Add(worksheet.Cells[i, 1].Text);
                    crtDetails.Add(worksheet.Cells[i, 2].Text);
                    crtDetails.Add(worksheet.Cells[i, 3].Text);
                    crtDetails.Add(worksheet.Cells[i, 4].Text);
                    crtDetails.Add(worksheet.Cells[i, 5].Text);
                    crtDetails.Add(worksheet.Cells[i, 6].Text);
                    crtDetails.Add(worksheet.Cells[i, 7].Text);
                    break;
                }

                if (string.IsNullOrWhiteSpace(worksheet.Cells[i, 1].Text))
                {
                    break;
                }
            }

            workbook.Close();
            excelApplication.Quit();
        }

        private bool GcmVerificationsForEudc(string dellPurchaseId, string baseItemPrice)
        {
            GcmFindEOrderPage.ClickViewButton(dellPurchaseId);
            if (!GcmOrderGroupLogPage.DoCustomerNumbersMatch())
            {
                return false;
            }

            GcmOrderGroupLogPage.GoToCustomerContactsPage();
            GcmCustomerContactsPage.GoToEndUserInfoPage();
            if (!CheckEndUserInfo(GcmEndUserInfoPage.GetEndUserDetails()))
            {
                return false;
            }

            webDriver.Navigate().Back(); // Go back to CustomerContacts page
            webDriver.WaitForPageLoad(new TimeSpan(0, 0, 10));
            webDriver.Navigate().Back(); // Go back to OrderGroupLog page
            webDriver.WaitForPageLoad(new TimeSpan(0, 0, 10));

            var parentWindow = webDriver.CurrentWindowHandle;
            GcmOrderGroupLogPage.GoToPrintFaxViewPage();
            if (!GcmEmailFaxViewPage.GetSubTotalValue().Substring(1).Equals(baseItemPrice))
            {
                return false;
            }

            if (!CheckEndUserInfo(GcmEmailFaxViewPage.GetEndUserDetails()))
            {
                return false;
            }

            GcmEmailFaxViewPage.GoToXmlResultsPage();
            if (!CheckEndUserInfo(GcmXmlResultsPage.GetEndUserDetails()))
            {
                return false;
            }

            return true;
        }

        private bool VerifyOrderStatusInGcm(string gcmUrl, string dellPurchaseId)
        {
            const int NumberOfRetries = 6;
            webDriver.Navigate().GoToUrl(gcmUrl);
            webDriver.WaitForPageLoad(new TimeSpan(0, 0, 10));
            // GCM verifcation start
            GcmMainPage.ClickDomsElement();

            // Checking the status, if COMPLETE or not
            var status = GcmFindEOrderPage.SearchByDpidAndGetOrderStatus(dellPurchaseId);
            if (status.ToUpper().Trim().Equals("COMPLETE"))
            {
                return true;
            }

            for (var i = 0; i < NumberOfRetries; i++)
            {
                System.Threading.Thread.Sleep(10000);
                Console.WriteLine("Retry No. {0}", i + 1);
                status = this.GcmFindEOrderPage.SearchByDpidAndGetOrderStatus(dellPurchaseId);
                if (status.ToUpper().Trim().Equals("COMPLETE"))
                {
                    break;
                }

                if (i != (NumberOfRetries - 1))
                {
                    continue;
                }

                Console.WriteLine("Aborting test. Status is ** {0} **", status);
                return false;
            }

            return true;
        }

        private bool SearchPoInLogReportPage(string poNumber)
        {
            // Log Report page operations. Provide po number and search
            B2BHomePage.ClickLogReport();

            return B2BLogReportPage.SearchByPoNumber(poNumber);
        }

        private bool CheckEndUserInfo(List<string> endUserDetails)
        {
            if (endUserDetails != null && endUserDetails.Count() != 0)
            {
                if (endUserDetails[0] == crtDetails[0].ToString() && endUserDetails[1] == crtDetails[1].ToString()
                    && endUserDetails[2] == crtDetails[2].ToString() && endUserDetails[3] == crtDetails[3].ToString()
                    && endUserDetails[4] == crtDetails[4].ToString() && endUserDetails[5] == crtDetails[5].ToString()
                    && endUserDetails[6] == crtDetails[6].ToString())
                {
                    return true;
                }
            }

            return false;
        }

        private bool VerifyMapperRequestDetailsAgainstPoTemplate(List<dynamic> poLineItems)
        {
            var orderDetails = XDocument.Load("CblTemplate.xml").XPathSelectElements("//OrderDetail");

            var listOfItemInfo = new List<dynamic>();

            var orderDetailCount = orderDetails.ToList().Count();

            for (var i = 0; i < orderDetailCount; i++)
            {
                var orderDetail = orderDetails.FirstOrDefault();

                listOfItemInfo.Add(new
                {
                    Quantity = orderDetail.XPathSelectElement("//BaseItemDetail/Quantity/Qty").Value,
                    Price = orderDetail.XPathSelectElement("//BuyerExpectedUnitPrice/Price/UnitPrice").Value
                });

                orderDetail.Remove();
            }

            if (poLineItems.Count() != listOfItemInfo.Count())
            {
                return false;
            }

            for (var i = 0; i < poLineItems.Count; i++)
            {
                if (
                    !(poLineItems[i].Quantity.Equals(listOfItemInfo[i].Quantity)
                      && poLineItems[i].Price.Equals(listOfItemInfo[i].Price)))
                {
                    return false;
                }
            }

            return true;
        }
    }

    public enum Workflow
    {
        Asn,
        Eudc
    }

    public enum RunEnvironment
    {
        Preview,
        Production
    }

    public enum PoXmlFormat
    {
        Cxml,
        Cbl
    }

    public enum QuoteType
    {
        Doms,
        EQuote,
        Bhc,
        OrQuote,
        Cif
    }
}
