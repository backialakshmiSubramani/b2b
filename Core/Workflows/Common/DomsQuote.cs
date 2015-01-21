﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using Modules.Channel.B2B.Core.Pages;
using Modules.Channel.B2B.Common;

namespace Modules.Channel.B2B.Core.Workflows.Common
{
    public class DomsQuote
    {
        private IWebDriver webDriver;
        private PoOperations poOperations;
        private string poNumber;

        public DomsQuote(IWebDriver driver)
        {
            this.webDriver = driver;
            poOperations = new PoOperations(webDriver);
        }

        private B2BHomePage B2BHomePage
        {
            get
            {
                return new B2BHomePage(this.webDriver);
            }
        }

        public RunEnvironment RunEnvironment { get; set; }
        public string OrderIdBase { get; set; }
        public string QuoteId { get; set; }
        public string Price { get; set; }
        public string IdentityName { get; set; }
        public string DeploymentMode { get; set; }
        public string CrtId { get; set; }
        public string Quantity { get; set; }
        public Workflow Workflow { get; set; }
        public PoXmlFormat PoXmlFormat { get; set; }
        public QuoteType QuoteType
        {
            get
            {
                return QuoteType.Doms;
            }
        }
        public string TargetUrl { get; set; }

        public bool CreateDomsPo()
        {
            B2BHomePage.SelectEnvironment(RunEnvironment.ToString());
            var orderId = OrderIdBase + DateTime.Today.ToString("yyMMdd") + DateTime.Now.ToString("HHmmss");
            var partId = "Q:" + QuoteId;

            string poXml;

            if (Workflow == Workflow.Eudc)
            {
                poXml = PoXmlGenerator.GeneratePoCxmlCblForEudc(
                    PoXmlFormat,
                    IdentityName,
                    DeploymentMode,
                    orderId,
                    Price,
                    partId,
                    CrtId);
            }
            else
            {
                var quoteDetails = new List<QuoteDetail>
                                       {
                                           new QuoteDetail()
                                               {
                                                   CrtId = this.CrtId,
                                                   Price = this.Price,
                                                   Quantity = this.Quantity,
                                                   QuoteType = this.QuoteType,
                                                   SupplierPartId = partId
                                               }
                                       };

                poXml = PoXmlGenerator.GeneratePoCblForAsn(PoXmlFormat, orderId, IdentityName, quoteDetails);

            }

            var parentWindow = webDriver.CurrentWindowHandle;

            B2BHomePage.ClickQaTools3();

            if (!poOperations.SubmitXmlForPoCreation(poXml, RunEnvironment.ToString(), TargetUrl, out poNumber))
            {
                return false;
            }

            if (!webDriver.CurrentWindowHandle.Equals(parentWindow))
            {
                webDriver.Close();
            }

            webDriver.SwitchTo().Window(parentWindow);
            return true;
        }

        public bool VerifyEudcPoCreation(string expectedDpidMessage, string expectedPurchaseOderMessage, string crtFilePath, string gcmUrl)
        {
            return !string.IsNullOrEmpty(this.poNumber)
                    && poOperations.VerifyEudcPoCreation(
                        this.poNumber,
                        this.RunEnvironment,
                        crtFilePath,
                        this.CrtId,
                        gcmUrl,
                        this.Price,
                        expectedDpidMessage,
                        expectedPurchaseOderMessage);
        }

        public bool VerifyQmsQuoteCreationForAsn(
            string quoteRetrievedMessagePrefix,
            string quoteRetrievedMessageSuffix,
            string enteringMasterOrderGroupMessage,
            string itemDescription)
        {
            return !string.IsNullOrEmpty(this.poNumber)
                   && this.poOperations.VerifyQmsQuoteCreation(
                       this.poNumber,
                       quoteRetrievedMessagePrefix,
                       quoteRetrievedMessageSuffix,
                       enteringMasterOrderGroupMessage,
                       itemDescription,
                       Quantity,
                       Price);
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

        public bool CaptureBackendOrderNumberFromOgXmlAndDbForAsn(string ogXmlMessage, string gcmUrl, string expectedDpidMessage)
        {
            //this.poNumber = "DCS0201E2ETest3";

            return !string.IsNullOrEmpty(this.poNumber)
                   && this.poOperations.CaptureBackendOrderNumberFromOgXmlAndDb(
                       this.poNumber,
                       ogXmlMessage,
                       gcmUrl,
                       expectedDpidMessage);
        }

        public bool VerifyFulfillmentUnitsForAsn(
            string expectedDpidMessage,
            string mapperRequestMessage,
            string ogXmlMessage,
            string gcmUrl)
        {
            //this.poNumber = "DCS0201E2ETest3";

            return !string.IsNullOrEmpty(this.poNumber)
                   && this.poOperations.VerifyFulfillmentUnits(
                       this.poNumber,
                       expectedDpidMessage,
                       mapperRequestMessage,
                       ogXmlMessage,
                       gcmUrl);
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
