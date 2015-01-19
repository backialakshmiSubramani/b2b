using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using Modules.Channel.B2B.Core.Pages;
using Modules.Channel.B2B.Common;
using System.Xml.Linq;

namespace Modules.Channel.B2B.Core.Workflows.Common
{
    public class BuyerCatalog
    {
        private IWebDriver webDriver;
        private PoOperations poOperations;
        private string poNumber;
        private string baseItemPrice;
        private string itemDescription;

        public BuyerCatalog(IWebDriver driver)
        {
            this.webDriver = driver;
            poOperations = new PoOperations(webDriver);
        }

        ////************************************************************
        public string ProfileName { get; set; }
        public string IdentityName { get; set; }
        public string ValidityEnd { get; set; }
        public string NotificationEmail { get; set; }
        public string ConfigurationType { get; set; }
        public string DeploymentMode { get; set; }
        public string OrderIdBase { get; set; }
        public string B2BCrtEndUserId { get; set; }
        public RunEnvironment RunEnvironment { get; set; }
        public string TargetUrl { get; set; }
        public Workflow Workflow { get; set; }
        public string CrtFilePath { get; set; }
        public PoXmlFormat PoXmlFormat { get; set; }
        public string GcmUrl { get; set; }
        public string Quantity { get; set; }
        public QuoteType QuoteType
        {
            get
            {
                return QuoteType.Bhc;
            }
        }
        ////************************************************************

        private B2BHomePage B2BHomePage
        {
            get
            {
                return new B2BHomePage(webDriver);
            }
        }

        private B2BCreateBuyerCatalogPage B2BCreateBuyerCatalogPage
        {
            get
            {
                return new B2BCreateBuyerCatalogPage(webDriver);
            }
        }

        private B2BBuyerCatalogListPage B2BBuyerCatalogListPage
        {
            get
            {
                return new B2BBuyerCatalogListPage(webDriver);
            }
        }

        private B2BCatalogViewerPage B2BCatalogViewer
        {
            get
            {
                return new B2BCatalogViewerPage(webDriver);
            }
        }

        // Creates Buyer Catalog and generates XML for PO submission
        public void CreateBhcPo()
        {
            B2BHomePage.SelectEnvironment(RunEnvironment.ToString());
            B2BHomePage.ClickOnBuyerCatalogLink();
            var threadId = B2BCreateBuyerCatalogPage.GenerateCatalog(
                ProfileName,
                IdentityName,
                ValidityEnd,
                NotificationEmail,
                ConfigurationType);
            B2BCreateBuyerCatalogPage.GoToBuyerCatalogListPage();
            B2BBuyerCatalogListPage.SearchForBuyerCatalog(ProfileName);
            if (!B2BBuyerCatalogListPage.CheckCatalogAvailabilityAndAct(threadId))
            {
                Console.WriteLine("The catalog status is not = 'Available'. Retrying....");
                for (var i = 0; i < 3; i++)
                {
                    System.Threading.Thread.Sleep(10000);
                    Console.WriteLine("Retry No. {0}", i + 1);
                    B2BBuyerCatalogListPage.SearchForBuyerCatalog(ProfileName);
                    if (B2BBuyerCatalogListPage.CheckCatalogAvailabilityAndAct(threadId))
                    {
                        break;
                    }

                    if (i == 2)
                    {
                        Console.WriteLine("The catalog status is still not 'Available'. No. of retries {0}", i + 1);
                        return;
                    }
                }
            }

            var orderId = OrderIdBase + DateTime.Today.ToString("yyMMdd") + DateTime.Now.ToString("HHmmss");
            var catalogPartId = B2BCatalogViewer.GetCatalogPartIdAndBaseUnitPrice(out baseItemPrice, out itemDescription);
            catalogPartId = "BHC:" + catalogPartId;

            string poXml;

            if (Workflow == Workflow.Eudc)
            {
                poXml = PoXmlGenerator.GeneratePoCxmlCblForEudc(
                     PoXmlFormat,
                     IdentityName,
                     DeploymentMode,
                     orderId,
                     baseItemPrice,
                     catalogPartId,
                     B2BCrtEndUserId);
            }
            else
            {
                poXml = PoXmlGenerator.GeneratePoCblForAsn(QuoteType, PoXmlFormat, orderId, IdentityName, catalogPartId, Quantity, baseItemPrice);
            }

            var parentWindow = webDriver.CurrentWindowHandle;

            B2BCatalogViewer.ClickQaTools3();

            if (!poOperations.SubmitXmlForPoCreation(poXml, RunEnvironment.ToString(), TargetUrl, out poNumber))
            {
                return;
            }

            if (!webDriver.CurrentWindowHandle.Equals(parentWindow))
            {
                webDriver.Close();
            }

            webDriver.SwitchTo().Window(parentWindow);
            B2BCatalogViewer.GoToHomePage();
        }

        public bool VerifyEudcPoCreation(string expectedDpidMessage, string expectedPurchaseOderMessage)
        {
            return !string.IsNullOrEmpty(this.poNumber)
                   && this.poOperations.VerifyEudcPoCreation(
                       this.poNumber,
                       this.RunEnvironment,
                       this.CrtFilePath,
                       this.B2BCrtEndUserId,
                       this.GcmUrl,
                       this.baseItemPrice,
                       expectedDpidMessage,
                       expectedPurchaseOderMessage);
        }

        public bool VerifyQmsQuoteCreationForAsn(
            string quoteRetrievedMessagePrefix,
            string quoteRetrievedMessageSuffix,
            string enteringMasterOrderGroupMessage)
        {
            return !string.IsNullOrEmpty(this.poNumber)
                   && this.poOperations.VerifyQmsQuoteCreation(
                       this.poNumber,
                       quoteRetrievedMessagePrefix,
                       quoteRetrievedMessageSuffix,
                       enteringMasterOrderGroupMessage,
                       itemDescription,
                       Quantity,
                       baseItemPrice);
        }

        public bool VerifyMapperRequestXmlDataInLogDetailPageForAsn(string mapperRequestMessage)
        {
            //this.poNumber = "POTestPQV1";

            return !string.IsNullOrEmpty(this.poNumber)
                   && this.poOperations.VerifyMapperRequestXmlDataInLogDetailPage(this.poNumber, mapperRequestMessage);
        }

        public bool VerifyDpidInMapperXmlAndDbForAsn(string expectedDpidMessage, string mapperRequestMessage)
        {
            //this.poNumber = "POTestPQV1";

            return !string.IsNullOrEmpty(this.poNumber)
                   && this.poOperations.VerifyDpidInMapperXmlAndDb(
                       this.poNumber,
                       expectedDpidMessage,
                       mapperRequestMessage);
        }

        public bool MatchItemIdInOgXmlAndMapperRequestAndDbForAsn(string ogXmlMessage, string mapperRequestMessage)
        {
            //this.poNumber = "DCS0201E2ETestJan7";

            return !string.IsNullOrEmpty(this.poNumber)
                   && this.poOperations.MatchItemIdInOgXmlAndMapperRequestAndDb(
                       this.poNumber,
                       ogXmlMessage,
                       mapperRequestMessage);
        }

        public bool CaptureBackendOrderNumberFromOgXmlAndDbForAsn(string ogXmlMessage, string expectedDpidMessage)
        {
            //this.poNumber = "DCS0201E2ETest3";

            return !string.IsNullOrEmpty(this.poNumber)
                   && this.poOperations.CaptureBackendOrderNumberFromOgXmlAndDb(
                       this.poNumber,
                       ogXmlMessage,
                       this.GcmUrl,
                       expectedDpidMessage);
        }

        public bool VerifyFulfillmentUnitsForAsn(string expectedDpidMessage, string mapperRequestMessage, string ogXmlMessage)
        {
            //this.poNumber = "DCS0201E2ETest3";

            return !string.IsNullOrEmpty(this.poNumber)
                   && this.poOperations.VerifyFulfillmentUnits(
                       this.poNumber,
                       expectedDpidMessage,
                       mapperRequestMessage,
                       ogXmlMessage,
                       this.GcmUrl);
        }

        public bool MatchValuesInPoXmlAndMapperXml(string expectedDpidMessage, string mapperRequestMessage)
        {
            //this.poNumber = "DCS0201E2ETest3";

            return !string.IsNullOrEmpty(this.poNumber)
                   && this.poOperations.MatchValuesInPoXmlAndMapperXml(
                       this.poNumber,
                       expectedDpidMessage,
                       mapperRequestMessage);
        }
    }
}
