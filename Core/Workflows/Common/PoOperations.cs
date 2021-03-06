﻿
using System.Globalization;
using System.Text.RegularExpressions;
//using Microsoft.SharePoint.Client.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modules.Channel.B2B.Common;
using Modules.Channel.B2B.Core.Pages;
using Modules.Channel.B2B.DAL;
using OpenQA.Selenium;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Excel = Microsoft.Office.Interop.Excel;
using decode = System.Web;
using Modules.Channel.ASN.Core.Pages;

//Adept Framework 
using Dell.Adept.UI.Web.Support.Extensions.WebDriver;
using Extensions = System.Xml.XPath.Extensions;


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

        private B2BCustomerProfileListPage B2BCustomerProfileListPage
        {
            get
            {
                return new B2BCustomerProfileListPage(webDriver);
            }
        }

        private B2BProfileSettingsGeneralPage B2BProfileSettingsGeneralPage
        {
            get
            {
                return new B2BProfileSettingsGeneralPage(webDriver);
            }
        }

        private B2BProfileSettingsAsnPage B2BProfileSettingsAsnPage
        {
            get
            {
                return new B2BProfileSettingsAsnPage(webDriver);
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

            if (string.IsNullOrEmpty(dellPurchaseId) || Convert.ToInt64(dellPurchaseId).Equals(-1))
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

        public bool SubmitXmlForPoCreation(string poXml, string environment, string targetUrl, string testEnvironment, out string poNumber)
        {
            B2BQaToolsPage.SelectLocationAndEnvironment(environment, testEnvironment);
            B2BQaToolsPage.PasteTargetUrl(targetUrl);
            B2BQaToolsPage.PasteInputXml(poXml);
            B2BQaToolsPage.ClickSubmitMessage();

            var submissionResult = B2BQaToolsPage.GetSubmissionResult();
            Console.WriteLine("Submission Result is: {0}", submissionResult);

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

            var quoteDetail =
                listOfQuoteDetail.FirstOrDefault(
                    qd =>
                    (!string.IsNullOrEmpty(qd.ItemDescription) && !string.IsNullOrEmpty(qd.Quantity)
                     && !string.IsNullOrEmpty(qd.Price)));

            if (quoteDetail == null)
            {
                Console.WriteLine("No valid item details were supplied for verification.");
                return false;
            }

            if (!B2BLogReportPage.FindMessageAndGoToQuoteViewerPage(enteringMasterOrderGroupMessage))
            {
                return false;
            }

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

            for (var i = 0; i < poLineItems.Count(); i++)
            {
                Console.WriteLine(
                    "Mapper XML PO Line Item {0} --> Quantity: {1}\t Price: {2}",
                    i + 1,
                    poLineItems[i].Quantity,
                    poLineItems[i].Price);
            }

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

                if (!B2BLogDetailPage.GetLogDetail().Contains(asnLogDetailMessages[i]))
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
                B2BLogDetailPage.ReturnToLogReport();
                return true;
            }
            catch
            {
                return false;
            }
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
            if (string.IsNullOrEmpty(dellPurchaseId) || Convert.ToInt64(dellPurchaseId).Equals(-1))
            {
                Console.WriteLine("Dpid not found. Stopping verification");
                return false;
            }

            ((IJavaScriptExecutor)this.webDriver).ExecuteScript("window.open();");
            var parentWindowHandle = this.webDriver.CurrentWindowHandle;
            var gcmWindow = webDriver.WindowHandles.LastOrDefault();
            webDriver.SwitchTo().Window(gcmWindow);
            webDriver.Manage().Window.Maximize();

            VerifyOrderStatusInGcm(gcmUrl, dellPurchaseId);

            webDriver.Close();
            webDriver.SwitchTo().Window(parentWindowHandle);

            return this.VerifyItemIdsInOgXmlMapperXmlAndDb(ogXmlMessage, mapperRequestMessage, poNumber);
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

            Console.WriteLine("Begin retrieval from ASNQueue");
            var firstRow = AsnDataAccess.FetchRecordsFromAsnQueue(poNumber).FirstOrDefault();
            Console.WriteLine("End retrieval from ASNQueue");
            return firstRow != null && firstRow.DPID.Equals(dellPurchaseId);
        }

        /// <summary>
        /// Verification points for Sprint18_P1 & Sprint18_P2 --> 562947
        /// </summary>
        /// <param name="poNumber"></param>
        /// <param name="expectedDpidMessage"></param>
        /// <param name="mapperRequestMessage"></param>
        /// <param name="profileName"></param>
        /// <param name="quote"></param>
        /// <returns></returns>

        public bool MatchValuesInPoXmlAndMapperXml(

            string poNumber,
            string expectedDpidMessage,
            string mapperRequestMessage,
            string profileName, string quote)
        {
            B2BHomePage.ClickB2BProfileList();
            B2BCustomerProfileListPage.SearchProfile(null, profileName);
            B2BCustomerProfileListPage.ClickSearchedProfile(profileName.ToUpper());
            B2BProfileSettingsGeneralPage.GoToAsnTab();
            var deliveryPreference = B2BProfileSettingsAsnPage.GetDeliveryPreference();
            B2BProfileSettingsAsnPage.GoToHomePage();

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

            var strXml = B2BLogDetailPage.GetLogDetail();
            strXml = PrepareXml(strXml);

            var mapperXml = XDocument.Parse(strXml);
            var savedPoCbl = XDocument.Load("CblTemplate.xml");
            Console.WriteLine("Begin retrieval from ASNQueue");
            var asnQueueEntry = AsnDataAccess.FetchRecordsFromAsnQueue(poNumber).FirstOrDefault();
            Console.WriteLine("End retrieval from ASNQueue");

            if (asnQueueEntry != null && !asnQueueEntry.DPID.Equals(dellPurchaseId))
            {
                return false;
            }

            if (!mapperXml.XPathSelectElement("//ThreadId").Value.Equals(threadId))
            {
                return false;
            }

            if (asnQueueEntry != null && !asnQueueEntry.ThreadId.Equals(threadId))
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

            if (asnQueueEntry != null && !asnQueueEntry.Partner.ToUpper().Equals(mapperXml.XPathSelectElement("//Partner").Value.ToUpper()))
            {
                return false;
            }

            var mapperLineItemNumber = mapperXml.XPathSelectElements("//LineItems/MapperRequestPOLine/LineItemNumber").FirstOrDefault();
            var poLineItemNumber = savedPoCbl.XPathSelectElements("//ListOfOrderDetail/OrderDetail/BaseItemDetail/LineItemNum").FirstOrDefault();
            if (mapperLineItemNumber != null && poLineItemNumber != null)
            {
                if (!mapperLineItemNumber.Value.Equals(poLineItemNumber.Value))
                {
                    return false;
                }
            }

            var mapperVendorPartNumber = mapperXml.XPathSelectElements("//LineItems/MapperRequestPOLine/VendorPartNumber").FirstOrDefault();
            var poVendorPartNumber = savedPoCbl.XPathSelectElements("//ListOfOrderDetail/OrderDetail/BaseItemDetail/ManufacturerPartNum/PartNum/PartID").FirstOrDefault();
            if (mapperVendorPartNumber != null && poVendorPartNumber != null)
            {
                if (!mapperVendorPartNumber.Value.Equals(poVendorPartNumber.Value))
                {
                    return false;
                }
            }

            if (mapperVendorPartNumber != null && asnQueueEntry != null && !asnQueueEntry.VendorPartNumber.Equals(mapperVendorPartNumber.Value))
            {
                return false;
            }

            var mapperBuyerSku = mapperXml.XPathSelectElements("//LineItems/MapperRequestPOLine/BuyerSKU").FirstOrDefault();
            var poBuyerSku = savedPoCbl.XPathSelectElements("//ListOfOrderDetail/OrderDetail/BaseItemDetail/BuyerPartNum/PartNum/PartID").FirstOrDefault();
            if (mapperBuyerSku != null && poBuyerSku != null)
            {
                if (!mapperBuyerSku.Value.Equals(poBuyerSku.Value))
                {
                    return false;
                }
            }

            if (mapperBuyerSku != null && asnQueueEntry != null && !asnQueueEntry.BuyerSKU.Equals(mapperBuyerSku.Value))
            {
                return false;
            }

            Console.WriteLine("Delivery Preference from profile settings page is: {0}", deliveryPreference);
            Console.WriteLine("Delivery Preference from mapper xml is: {0}", mapperXml.XPathSelectElement("//DeliveryPreference").Value);
            if (asnQueueEntry != null)
            {
                Console.WriteLine("Delivery Preference from database is: {0}", asnQueueEntry.DeliveryPreference.ToString(CultureInfo.InvariantCulture));

                var mapperDeliveryPreference = mapperXml.XPathSelectElement("//DeliveryPreference");
                if (!mapperDeliveryPreference.Value.Equals(deliveryPreference))
                {
                    return false;
                }

                if (!asnQueueEntry.DeliveryPreference.ToString(CultureInfo.InvariantCulture).Equals(mapperDeliveryPreference.Value))
                {
                    return false;
                }
            }

            //Compare PO Fields in Mapper Xml
            //PO#
            if (!mapperXml.XPathSelectElement("//PORequestXml/PurchaseOrder/OrderHeader/OrderReference/BuyerRefNum/Reference/RefNum").Value.Equals(savedPoCbl.XPathSelectElement("//BuyerRefNum/Reference/RefNum").Value))
            {
                return false;
            }

            //Partner
            if (!mapperXml.XPathSelectElement("//PORequestXml/PurchaseOrder/OrderHeader/OrderParty/BuyerParty/Party/ListOfIdentifier/Identifier/Agency").Attribute("AgencyOther").Value.ToUpper().Equals(savedPoCbl.XPathSelectElement("//BuyerParty/Party/ListOfIdentifier/Identifier/Agency")
                     .Attribute("AgencyOther")
                     .Value.ToUpper()))
            {
                return false;
            }

            //LineItem#
            if (!mapperXml.XPathSelectElement("//PORequestXml/PurchaseOrder/ListOfOrderDetail/OrderDetail/BaseItemDetail/LineItemNum").Value.
                Equals(savedPoCbl.XPathSelectElement("//BaseItemDetail/LineItemNum").Value))
            {
                return false;
            }

            quote = "-" + quote;

            //Quote #
            if (quote.Contains("\\") && !mapperXml.XPathSelectElement("//PORequestXml/PurchaseOrder/ListOfOrderDetail/OrderDetail/BaseItemDetail/SupplierPartNum/PartNum/PartIDExt").Value.
                Equals(quote))//OR
            {
                return false;
            }
            else if (quote.Contains("EQ:") && !mapperXml.XPathSelectElement("//PORequestXml/PurchaseOrder/ListOfOrderDetail/OrderDetail/BaseItemDetail/SupplierPartNum/PartNum/PartID").Value.
                Equals(quote))//Equote
            {
                return false;
            }
            else if (quote.Contains("Q:") && !mapperXml.XPathSelectElement("//PORequestXml/PurchaseOrder/ListOfOrderDetail/OrderDetail/BaseItemDetail/SupplierPartNum/PartNum/PartID").Value.
                Equals(quote))//Doms
            {
                return false;
            }
            else if (quote.Contains("BHC:") && !mapperXml.XPathSelectElement("//PORequestXml/PurchaseOrder/ListOfOrderDetail/OrderDetail/BaseItemDetail/SupplierPartNum/PartNum/PartIDExt").Value.
            Equals(quote))//OR
            {
                return false;
            }
            else if (!mapperXml.XPathSelectElement("//PORequestXml/PurchaseOrder/ListOfOrderDetail/OrderDetail/BaseItemDetail/SupplierPartNum/PartNum/PartIDExt").Value.
                Equals(quote))//Cif
            {
                return false;
            }

            //Buyer SKUID
            var poBuyrSku = savedPoCbl.XPathSelectElements("//ListOfOrderDetail/OrderDetail/BaseItemDetail/BuyerPartNum/PartNum/PartID").FirstOrDefault();
            if (poBuyrSku != null && !mapperXml.XPathSelectElement("//PORequestXml/PurchaseOrder/ListOfOrderDetail/OrderDetail/BaseItemDetail/BuyerPartNum/PartNum/PartID").Value.
                Equals(poBuyrSku.Value))
            {
                return false;
            }

            //Vendor PartID
            var poVendorPartId = savedPoCbl.XPathSelectElements("//ListOfOrderDetail/OrderDetail/BaseItemDetail/ManufacturerPartNum/PartNum/PartID").FirstOrDefault();
            if (poVendorPartId != null && !mapperXml.XPathSelectElement("//PORequestXml/PurchaseOrder/ListOfOrderDetail/OrderDetail/BaseItemDetail/ManufacturerPartNum/PartNum/PartID").Value.
                Equals(poVendorPartId.Value))
            {
                return false;
            }

            //Quantity
            var poQty = savedPoCbl.XPathSelectElements("//ListOfOrderDetail/OrderDetail/BaseItemDetail/Quantity/Qty ").FirstOrDefault();
            if (poQty != null && !mapperXml.XPathSelectElement("//PORequestXml/PurchaseOrder/ListOfOrderDetail/OrderDetail/BaseItemDetail/Quantity/Qty").Value.
                Equals(poQty.Value))
            {
                return false;
            }

            //Unit Price
            var poUnitPrice = savedPoCbl.XPathSelectElements("//ListOfOrderDetail/OrderDetail/BuyerExpectedUnitPrice/Price/UnitPrice ").FirstOrDefault();
            if (poUnitPrice != null && !mapperXml.XPathSelectElement("//PORequestXml/PurchaseOrder/ListOfOrderDetail/OrderDetail/BuyerExpectedUnitPrice/Price/UnitPrice").Value.
                Equals(poUnitPrice.Value))
            {
                return false;
            }

            return true;
        }
        public bool MatchValuesInPoXmlAndMapperXmlProd(

            string poNumber,
            string expectedDpidMessage,
            string mapperRequestMessage,
            string profileName, string quote)
        {
            //B2BHomePage.ClickB2BProfileList();
            //B2BCustomerProfileListPage.SearchProfile(null, profileName);
            //B2BCustomerProfileListPage.ClickSearchedProfile(profileName);
            //B2BProfileSettingsGeneralPage.GoToAsnTab();
            //var deliveryPreference = B2BProfileSettingsAsnPage.GetDeliveryPreference();
            //B2BProfileSettingsAsnPage.GoToHomePage();

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

            var strXml = B2BLogDetailPage.GetLogDetail();
            strXml = PrepareXml(strXml);

            var mapperXml = XDocument.Parse(strXml);
            var savedPoCbl = XDocument.Load("CblTemplate.xml");


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

            //if (asnQueueEntry != null && !asnQueueEntry.Partner.ToUpper().Equals(mapperXml.XPathSelectElement("//Partner").Value.ToUpper()))
            //{
            //    return false;
            //}

            var mapperLineItemNumber = mapperXml.XPathSelectElements("//LineItems/MapperRequestPOLine/LineItemNumber").FirstOrDefault();
            var poLineItemNumber = savedPoCbl.XPathSelectElements("//ListOfOrderDetail/OrderDetail/BaseItemDetail/LineItemNum").FirstOrDefault();
            if (mapperLineItemNumber != null && poLineItemNumber != null)
            {
                if (!mapperLineItemNumber.Value.Equals(poLineItemNumber.Value))
                {
                    return false;
                }
                Console.WriteLine("LineItem Number verified");
            }

            var mapperVendorPartNumber = mapperXml.XPathSelectElements("//LineItems/MapperRequestPOLine/VendorPartNumber").FirstOrDefault();
            var poVendorPartNumber = savedPoCbl.XPathSelectElements("//ListOfOrderDetail/OrderDetail/BaseItemDetail/ManufacturerPartNum/PartNum/PartID").FirstOrDefault();
            if (mapperVendorPartNumber != null && poVendorPartNumber != null)
            {
                if (!mapperVendorPartNumber.Value.Equals(poVendorPartNumber.Value))
                {
                    return false;
                }
                Console.WriteLine("Vendor Part Number verified");
            }

            //if (mapperVendorPartNumber != null && asnQueueEntry != null && !asnQueueEntry.VendorPartNumber.Equals(mapperVendorPartNumber.Value))
            //{
            //    return false;
            //}

            var mapperBuyerSku = mapperXml.XPathSelectElements("//LineItems/MapperRequestPOLine/BuyerSKU").FirstOrDefault();
            var poBuyerSku = savedPoCbl.XPathSelectElements("//ListOfOrderDetail/OrderDetail/BaseItemDetail/BuyerPartNum/PartNum/PartID").FirstOrDefault();
            if (mapperBuyerSku != null && poBuyerSku != null)
            {
                if (!mapperBuyerSku.Value.Equals(poBuyerSku.Value))
                {
                    return false;
                }
                Console.WriteLine("Buyer SKU Number verified");
            }

            //if (mapperBuyerSku != null && asnQueueEntry != null && !asnQueueEntry.BuyerSKU.Equals(mapperBuyerSku.Value))
            //{
            //    return false;
            //}

            //Console.WriteLine("Delivery Preference from profile settings page is: {0}", deliveryPreference);
            //Console.WriteLine("Delivery Preference from mapper xml is: {0}", mapperXml.XPathSelectElement("//DeliveryPreference").Value);
            //if (asnQueueEntry != null)
            //{
            //    Console.WriteLine("Delivery Preference from database is: {0}", asnQueueEntry.DeliveryPreference.ToString(CultureInfo.InvariantCulture));

            //    var mapperDeliveryPreference = mapperXml.XPathSelectElement("//DeliveryPreference");
            //    if (!mapperDeliveryPreference.Value.Equals(deliveryPreference))
            //    {
            //        return false;
            //    }

            //    if (!asnQueueEntry.DeliveryPreference.ToString(CultureInfo.InvariantCulture).Equals(mapperDeliveryPreference.Value))
            //    {
            //        return false;
            //    }
            //}

            //Compare PO Fields in Mapper Xml
            //PO#
            if (!mapperXml.XPathSelectElement("//PORequestXml/PurchaseOrder/OrderHeader/OrderReference/BuyerRefNum/Reference/RefNum").Value.Equals(savedPoCbl.XPathSelectElement("//BuyerRefNum/Reference/RefNum").Value))
            {
                return false;
            }
            Console.WriteLine("PO# Number verified");

            //Partner
            if (!mapperXml.XPathSelectElement("//PORequestXml/PurchaseOrder/OrderHeader/OrderParty/BuyerParty/Party/ListOfIdentifier/Identifier/Agency").Attribute("AgencyOther").Value.ToUpper().Equals(savedPoCbl.XPathSelectElement("//BuyerParty/Party/ListOfIdentifier/Identifier/Agency")
                     .Attribute("AgencyOther")
                     .Value.ToUpper()))
            {
                return false;
            }
            Console.WriteLine("Partner verified");
            //LineItem#
            if (!mapperXml.XPathSelectElement("//PORequestXml/PurchaseOrder/ListOfOrderDetail/OrderDetail/BaseItemDetail/LineItemNum").Value.
                Equals(savedPoCbl.XPathSelectElement("//BaseItemDetail/LineItemNum").Value))
            {
                return false;
            }
            Console.WriteLine("LineItem Number verified");

            //Quote #
            if (quote.Contains("\\") && !mapperXml.XPathSelectElement("//PORequestXml/PurchaseOrder/ListOfOrderDetail/OrderDetail/BaseItemDetail/SupplierPartNum/PartNum/PartIDExt").Value.
                Equals(quote.Replace("\\", "-")))//CIF
            {
                return false;
            }
            else if (quote.Contains("EQ:") && !mapperXml.XPathSelectElement("//PORequestXml/PurchaseOrder/ListOfOrderDetail/OrderDetail/BaseItemDetail/SupplierPartNum/PartNum/PartID").Value.
                Equals(quote))//Equote
            {
                return false;
            }
            else if (quote.Contains("Q:") && !mapperXml.XPathSelectElement("//PORequestXml/PurchaseOrder/ListOfOrderDetail/OrderDetail/BaseItemDetail/SupplierPartNum/PartNum/PartID").Value.
                Equals(quote))//Doms
            {
                return false;
            }
            else if (quote.Contains("BHC:") && !mapperXml.XPathSelectElement("//PORequestXml/PurchaseOrder/ListOfOrderDetail/OrderDetail/BaseItemDetail/SupplierPartNum/PartNum/PartIDExt").Value.
            Equals(quote))//BHC
            {
                return false;
            }
            else if (quote.Contains("-") && !mapperXml.XPathSelectElement("//PORequestXml/PurchaseOrder/ListOfOrderDetail/OrderDetail/BaseItemDetail/SupplierPartNum/PartNum/PartIDExt").Value.
                Equals(quote))//OR
            {
                return false;
            }
            Console.WriteLine("Quote Number verified");
            //Buyer SKUID
            var poBuyrSku = savedPoCbl.XPathSelectElements("//ListOfOrderDetail/OrderDetail/BaseItemDetail/BuyerPartNum/PartNum/PartID").FirstOrDefault();
            if (poBuyrSku != null && !mapperXml.XPathSelectElement("//PORequestXml/PurchaseOrder/ListOfOrderDetail/OrderDetail/BaseItemDetail/BuyerPartNum/PartNum/PartID").Value.
                Equals(poBuyrSku.Value))
            {
                return false;
            }
            Console.WriteLine("Buyer SKU verified");
            //Vendor PartID
            var poVendorPartId = savedPoCbl.XPathSelectElements("//ListOfOrderDetail/OrderDetail/BaseItemDetail/ManufacturerPartNum/PartNum/PartID").FirstOrDefault();
            if (poVendorPartId != null && !mapperXml.XPathSelectElement("//PORequestXml/PurchaseOrder/ListOfOrderDetail/OrderDetail/BaseItemDetail/ManufacturerPartNum/PartNum/PartID").Value.
                Equals(poVendorPartId.Value))
            {
                return false;
            }
            Console.WriteLine("VPN verified");
            //Quantity
            var poQty = savedPoCbl.XPathSelectElements("//ListOfOrderDetail/OrderDetail/BaseItemDetail/Quantity/Qty ").FirstOrDefault();
            if (poQty != null && !mapperXml.XPathSelectElement("//PORequestXml/PurchaseOrder/ListOfOrderDetail/OrderDetail/BaseItemDetail/Quantity/Qty").Value.
                Equals(poQty.Value))
            {
                return false;
            }
            Console.WriteLine("Qty verified");
            //Unit Price
            var poUnitPrice = savedPoCbl.XPathSelectElements("//ListOfOrderDetail/OrderDetail/BuyerExpectedUnitPrice/Price/UnitPrice ").FirstOrDefault();
            if (poUnitPrice != null && !mapperXml.XPathSelectElement("//PORequestXml/PurchaseOrder/ListOfOrderDetail/OrderDetail/BuyerExpectedUnitPrice/Price/UnitPrice").Value.
                Equals(poUnitPrice.Value))
            {
                return false;
            }
            Console.WriteLine("Unit price verified");
            return true;
        }

        public string PrepareXml(string strXml)
        {
            strXml = decode.HttpUtility.HtmlDecode(strXml);
            strXml = decode.HttpUtility.HtmlDecode(strXml);
            strXml = Regex.Replace(strXml, @"<\?xml.*?>", "");
            return strXml;
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

            return this.VerifyItemIdsInOgXmlMapperXmlAndDb(ogXmlMessage, mapperRequestMessage, poNumber);
        }

        public bool MatchItemIdInOgXmlAndMapperRequestAndDbProd(
           string poNumber,
           string ogXmlMessage,
           string mapperRequestMessage)
        {
            if (!this.SearchPoInLogReportPage(poNumber))
            {
                return false;
            }

            return this.VerifyItemIdsInOgXmlMapperXmlAndDb(ogXmlMessage, mapperRequestMessage, poNumber);
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
            if (string.IsNullOrEmpty(dellPurchaseId) || Convert.ToInt64(dellPurchaseId).Equals(-1))
            {
                Console.WriteLine("Dpid not found. Stopping verification");
                return false;
            }

            Console.WriteLine("Starting GCM verification");
            if (!VerifyOrderStatusInGcm(gcmUrl, dellPurchaseId, true))
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

            Console.WriteLine("Begin retrieval from ASNOrderMapping");
            var orderNumbersFromDb = AsnDataAccess.FetchBackendOrderNumber(poNumber);
            Console.WriteLine("End retrieval from ASNOrderMapping");

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

        public List<POLine> FetchPurchaseOrderAndPoLinesFromAsnDb(string poNumber, List<PurchaseOrder> purchaseOrderDetails)
        {
            return AsnDataAccess.FetchPurchaseOrderDetails(poNumber, out purchaseOrderDetails);
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

        private bool VerifyOrderStatusInGcm(string gcmUrl, string dellPurchaseId, bool retry = false)
        {
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

            if (retry)
            {
                var startTime = DateTime.UtcNow;
                while (DateTime.UtcNow - startTime < TimeSpan.FromMinutes(15))
                {
                    status = this.GcmFindEOrderPage.SearchByDpidAndGetOrderStatus(dellPurchaseId);
                    if (status.ToUpper().Trim().Equals("COMPLETE"))
                    {
                        return true;
                    }
                }
            }

            Console.WriteLine("GCM status is ** {0} **", status);
            return false;
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

            for (var i = 0; i < listOfItemInfo.Count(); i++)
            {
                Console.WriteLine(
                    "PO Template PO Line Item {0} --> Quantity: {1}\t Price: {2}",
                    i + 1,
                    listOfItemInfo[i].Quantity,
                    listOfItemInfo[i].Price);
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

        private bool VerifyItemIdsInOgXmlMapperXmlAndDb(string ogXmlMessage, string mapperRequestMessage, string poNumber)
        {
            if (!B2BLogReportPage.FindMessageAndGoToLogDetailPage(ogXmlMessage))
            {
                return false;
            }

            var ogXml = XDocument.Parse(B2BLogDetailPage.GetLogDetail());

            B2BLogDetailPage.ReturnToLogReport();

            var countOfOrderFormItemElements =
                ogXml.XPathSelectElements("//OrderGroup/OrderForms/OrderForm/Items/Item").ToList().Count;

            var itemIds = new List<string>();

            for (var i = 0; i < countOfOrderFormItemElements; i++)
            {
                var idValue = ogXml.XPathSelectElements("//OrderGroup/OrderForms/OrderForm/Items/Item/Id").FirstOrDefault();
                if (idValue !=
                    null)
                    itemIds.Add(idValue.Value);

                var compIdValue =
                    ogXml.XPathSelectElements(
                        "//OrderGroup/OrderForms/OrderForm/Items/Item/ConfigDetails/Modules/Module/Options/Option/CompositeItems/Item/Id").FirstOrDefault();

                if (compIdValue != null)
                {
                    itemIds.Add(compIdValue.Value);
                }

                var orderFormItem = ogXml.XPathSelectElements("//OrderGroup/OrderForms/OrderForm/Items/Item").FirstOrDefault();
                if (orderFormItem != null)
                    orderFormItem.Remove();
            }

            if (!itemIds.Any())
            {
                return false;
            }

            var itemIdsFromFulfillment =
                ogXml.XPathSelectElements(
                    "//OrderGroup/OrderForms/OrderForm/FulfillmentUnits/FulfillmentUnit/FulfillmentItemInformation/FulfillmentItemInformation/ItemId")
                    .Select(i => i.Value);

            var idsFromFulfillment = itemIdsFromFulfillment as string[] ?? itemIdsFromFulfillment.ToArray();
            if (!idsFromFulfillment.Any())
            {
                return false;
            }
            if (!itemIds.OrderBy(x => x).SequenceEqual(idsFromFulfillment.OrderBy(x => x)))
            {
                return false;
            }

            if (!B2BLogReportPage.FindMessageAndGoToLogDetailPage(mapperRequestMessage))
            {
                return false;
            }

            var itemIdsFromMapperRequestXml = B2BLogDetailPage.GetItemIdsFromMapperRequestXml();

            var idsFromMapperRequestXml = itemIdsFromMapperRequestXml as string[] ?? itemIdsFromMapperRequestXml.ToArray();
            if (!idsFromMapperRequestXml.Any())
            {
                return false;
            }
            if (!itemIds.OrderBy(x => x).SequenceEqual(idsFromMapperRequestXml.OrderBy(x => x)))
            {
                return false;
            }

            Console.WriteLine("Begin retrieval from ASNItemMapping");
            var itemIdsFromDb = AsnDataAccess.FetchItemId(poNumber);
            Console.WriteLine("End retrieval from ASNItemMapping");

            if (!itemIdsFromDb.Any())
            {
                return false;
            }
            if (!itemIds.OrderBy(x => x).SequenceEqual(itemIdsFromDb.Select(i => i.ToLower()).OrderBy(x => x)))
            {
                return false;
            }

            return true;
        }
        private bool VerifyItemIdsInOgXmlMapperXmlAndDbProd(string ogXmlMessage, string mapperRequestMessage, string poNumber)
        {
            if (!B2BLogReportPage.FindMessageAndGoToLogDetailPage(ogXmlMessage))
            {
                return false;
            }

            var ogXml = XDocument.Parse(B2BLogDetailPage.GetLogDetail());

            B2BLogDetailPage.ReturnToLogReport();

            var countOfOrderFormItemElements =
                ogXml.XPathSelectElements("//OrderGroup/OrderForms/OrderForm/Items/Item").ToList().Count;

            var itemIds = new List<string>();

            for (var i = 0; i < countOfOrderFormItemElements; i++)
            {
                var idValue = ogXml.XPathSelectElements("//OrderGroup/OrderForms/OrderForm/Items/Item/Id").FirstOrDefault();
                if (idValue !=
                    null)
                    itemIds.Add(idValue.Value);

                var compIdValue =
                    ogXml.XPathSelectElements(
                        "//OrderGroup/OrderForms/OrderForm/Items/Item/ConfigDetails/Modules/Module/Options/Option/CompositeItems/Item/Id").FirstOrDefault();

                if (compIdValue != null)
                {
                    itemIds.Add(compIdValue.Value);
                }

                var orderFormItem = ogXml.XPathSelectElements("//OrderGroup/OrderForms/OrderForm/Items/Item").FirstOrDefault();
                if (orderFormItem != null)
                    orderFormItem.Remove();
            }

            if (!itemIds.Any())
            {
                return false;
            }

            var itemIdsFromFulfillment =
                ogXml.XPathSelectElements(
                    "//OrderGroup/OrderForms/OrderForm/FulfillmentUnits/FulfillmentUnit/FulfillmentItemInformation/FulfillmentItemInformation/ItemId")
                    .Select(i => i.Value);

            var idsFromFulfillment = itemIdsFromFulfillment as string[] ?? itemIdsFromFulfillment.ToArray();
            if (!idsFromFulfillment.Any())
            {
                return false;
            }
            if (!itemIds.OrderBy(x => x).SequenceEqual(idsFromFulfillment.OrderBy(x => x)))
            {
                return false;
            }

            if (!B2BLogReportPage.FindMessageAndGoToLogDetailPage(mapperRequestMessage))
            {
                return false;
            }

            var itemIdsFromMapperRequestXml = B2BLogDetailPage.GetItemIdsFromMapperRequestXml();

            var idsFromMapperRequestXml = itemIdsFromMapperRequestXml as string[] ?? itemIdsFromMapperRequestXml.ToArray();
            if (!idsFromMapperRequestXml.Any())
            {
                return false;
            }
            if (!itemIds.OrderBy(x => x).SequenceEqual(idsFromMapperRequestXml.OrderBy(x => x)))
            {
                return false;
            }

            Console.WriteLine("Begin retrieval from ASNItemMapping");
            var itemIdsFromDb = AsnDataAccess.FetchItemId(poNumber);
            Console.WriteLine("End retrieval from ASNItemMapping");

            if (!itemIdsFromDb.Any())
            {
                return false;
            }
            if (!itemIds.OrderBy(x => x).SequenceEqual(itemIdsFromDb.Select(i => i.ToLower()).OrderBy(x => x)))
            {
                return false;
            }

            return true;
        }

        public List<PurchaseOrder> GetPurchaseOrdersDetailsBasedOnPoNumber(string poNumber)
        {
            return AsnDataAccess.GetPurchaseOrdersDetailsBasedOnPoNumber(poNumber);
        }

        public List<POLine> GetLineDetailsBasedOnPoNumber(string poNumber)
        {
            return AsnDataAccess.GetLineDetailsBasedOnPoNumber(poNumber);
        }

        public List<Order> GetOrderDetailsBasedOnPoNumber(string poNumber)
        {
            return AsnDataAccess.GetOrderDetailsBasedOnPoNumber(poNumber);
        }

        public List<ShippingInfo> GetShipDetailsBasedOnOrderNumber(string orderId)
        {
            return AsnDataAccess.GetShipDetailsBasedOnOrderNumber(orderId);
        }

        public int? GetNumberOfUnitsShippedBasedOnOrderNumber(string orderId)
        {
            return AsnDataAccess.GetNumberOfUnitsShippedBasedOnOrderNumber(orderId);
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
