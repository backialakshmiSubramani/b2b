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
    public class CifQuote
    {
        private IWebDriver webDriver;
        private PoOperations poOperations;
        private string poNumber;

        public CifQuote(IWebDriver driver)
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
        public string ProfileName { get; set; }
        public string IdentityName { get; set; }
        public string DeploymentMode { get; set; }
        public Workflow Workflow { get; set; }
        public PoXmlFormat PoXmlFormat { get; set; }
        public string TargetUrl { get; set; }

        public bool CreateCifPo(List<QuoteDetail> listOfQuoteDetail, string testEnvironment, bool removeInternalVendorNumber = false)
        {
            B2BHomePage.SelectEnvironment(RunEnvironment.ToString());
            var orderId = OrderIdBase + DateTime.Today.ToString("yyMMdd") + DateTime.Now.ToString("HHmmss");

            string poXml;

            poXml = PoXmlGenerator.GeneratePoCblForAsn(PoXmlFormat, orderId, IdentityName, listOfQuoteDetail, removeInternalVendorNumber);

            var parentWindow = webDriver.CurrentWindowHandle;

            B2BHomePage.ClickQaTools3();

            if (!poOperations.SubmitXmlForPoCreation(poXml, RunEnvironment.ToString(), TargetUrl, testEnvironment, out poNumber))
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

        public bool VerifyQmsQuoteCreationForAsn(
            string quoteRetrievedMessagePrefix,
            string quoteRetrievedMessageSuffix,
            string enteringMasterOrderGroupMessage,
            List<QuoteDetail> listOfQuoteDetail)
        {
            return !string.IsNullOrEmpty(this.poNumber)
                   && this.poOperations.VerifyQmsQuoteCreation(
                       this.poNumber,
                       quoteRetrievedMessagePrefix,
                       quoteRetrievedMessageSuffix,
                       enteringMasterOrderGroupMessage,
                       listOfQuoteDetail);
        }

        public bool VerifyMapperRequestXmlDataInLogDetailPageForAsn(string mapperRequestMessage)
        {
            return !string.IsNullOrEmpty(this.poNumber)
                   && this.poOperations.VerifyMapperRequestXmlDataInLogDetailPage(this.poNumber, mapperRequestMessage);
        }

        public bool VerifyDpidInMapperXmlAndDbForAsn(string expectedDpidMessage, string mapperRequestMessage)
        {
            return !string.IsNullOrEmpty(this.poNumber)
                   && this.poOperations.VerifyDpidInMapperXmlAndDb(
                       this.poNumber,
                       expectedDpidMessage,
                       mapperRequestMessage);
        }

        public bool MatchItemIdInOgXmlAndMapperRequestAndDbForAsn(string ogXmlMessage, string mapperRequestMessage)
        {
            return !string.IsNullOrEmpty(this.poNumber)
                   && this.poOperations.MatchItemIdInOgXmlAndMapperRequestAndDb(
                       this.poNumber,
                       ogXmlMessage,
                       mapperRequestMessage);
        }
        public bool MatchItemIdInOgXmlAndMapperRequestAndDbForAsnProd(string ogXmlMessage, string mapperRequestMessage)
        {
            return !string.IsNullOrEmpty(this.poNumber)
                   && this.poOperations.MatchItemIdInOgXmlAndMapperRequestAndDb(
                       this.poNumber,
                       ogXmlMessage,
                       mapperRequestMessage);
        }

        public bool CaptureBackendOrderNumberFromOgXmlAndDbForAsn(string ogXmlMessage, string gcmUrl, string expectedDpidMessage)
        {
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
            return !string.IsNullOrEmpty(this.poNumber)
                   && this.poOperations.VerifyFulfillmentUnits(
                       this.poNumber,
                       expectedDpidMessage,
                       mapperRequestMessage,
                       ogXmlMessage,
                       gcmUrl);
        }

        public bool MatchValuesInPoXmlAndMapperXml(string expectedDpidMessage, string mapperRequestMessage,string quote)
        {
            return !string.IsNullOrEmpty(this.poNumber)
                   && this.poOperations.MatchValuesInPoXmlAndMapperXml(
                       this.poNumber,
                       expectedDpidMessage,
                       mapperRequestMessage,
                       this.ProfileName, quote);
        }

        public bool VerifyMappingEntriesForChannelAsnEnabledProfileForAsn(List<string> asnLogEventMessages, List<string> asnLogDetailMessages, string mapperRequestMessage)
        {
            return !string.IsNullOrEmpty(this.poNumber)
                   && this.poOperations.VerifyMappingEntriesForChannelAsnEnabledProfile(
                   this.poNumber,
                   asnLogEventMessages,
                   asnLogDetailMessages,
                   mapperRequestMessage);
        }

        public bool MatchValuesInPoXmlAndMapperXmlProd(string expectedDpidMessage, string mapperRequestMessage, string quote)
        {
            return !string.IsNullOrEmpty(this.poNumber)
                    && this.poOperations.MatchValuesInPoXmlAndMapperXmlProd(
                        this.poNumber,
                        expectedDpidMessage,
                        mapperRequestMessage,
                        this.ProfileName, quote);
        }
    }
}
