﻿using System;
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
        public string CrtId { get; set; }
        public RunEnvironment RunEnvironment { get; set; }
        public string TargetUrl { get; set; }
        public Workflow Workflow { get; set; }
        public string CrtFilePath { get; set; }
        public PoXmlFormat PoXmlFormat { get; set; }
        public string GcmUrl { get; set; }
        public string Quantity { get; set; }
        public string TestEnvironment { get; set; }
        public QuoteType QuoteType
        {
            get
            {
                return QuoteType.Bhc;
            }
        }
        public List<string> ConfigTitles { get; set; }

        public List<QuoteDetail> ListOfQuoteDetail { get; set; }

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

        /// <summary>
        /// Creates Buyer Catalog and processes PO
        /// </summary>
        public void CreateBhcPo()
        {
            B2BHomePage.SelectEnvironment(RunEnvironment.ToString());
            if (ListOfQuoteDetail == null || !ListOfQuoteDetail.Any()
                || string.IsNullOrEmpty(ListOfQuoteDetail.FirstOrDefault().SupplierPartId))
            {
                const int NumberOfRetries = 10;
                B2BHomePage.ClickOnBuyerCatalogLink();
                var threadId = B2BCreateBuyerCatalogPage.GenerateCatalog(
                    Workflow,
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
                    for (var i = 0; i < NumberOfRetries; i++)
                    {
                        System.Threading.Thread.Sleep(20000);
                        Console.WriteLine("Retry No. {0}", i + 1);
                        B2BBuyerCatalogListPage.SearchForBuyerCatalog(ProfileName);
                        if (B2BBuyerCatalogListPage.CheckCatalogAvailabilityAndAct(threadId))
                        {
                            break;
                        }

                        if (i != (NumberOfRetries - 1))
                        {
                            continue;
                        }

                        Console.WriteLine("The catalog status is still not 'Available'. No. of retries {0}", i + 1);
                        return;
                    }
                }

                if (Workflow == Workflow.Eudc)
                {
                    ListOfQuoteDetail = B2BCatalogViewer.GetQuoteDetails(CrtId, Quantity, QuoteType);
                }
                else
                {
                    ListOfQuoteDetail = B2BCatalogViewer.GetQuoteDetails(CrtId, Quantity, QuoteType, ConfigTitles);
                }
            }

            var orderId = OrderIdBase + DateTime.Today.ToString("yyMMdd") + DateTime.Now.ToString("HHmmss");

            string poXml;

            if (Workflow == Workflow.Eudc)
            {
                poXml = PoXmlGenerator.GeneratePoCxmlCblForEudc(
                     PoXmlFormat,
                     IdentityName,
                     DeploymentMode,
                     orderId,
                     ListOfQuoteDetail.FirstOrDefault().Price,
                     ListOfQuoteDetail.FirstOrDefault().SupplierPartId,
                     ListOfQuoteDetail.FirstOrDefault().CrtId);
            }
            else
            {
                poXml = PoXmlGenerator.GeneratePoCblForAsn(PoXmlFormat, orderId, IdentityName, ListOfQuoteDetail);
            }

            var parentWindow = webDriver.CurrentWindowHandle;

            B2BCatalogViewer.ClickQaTools3();

            if (!poOperations.SubmitXmlForPoCreation(poXml, RunEnvironment.ToString(), TargetUrl, TestEnvironment, out poNumber))
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
                       this.ListOfQuoteDetail.FirstOrDefault().CrtId,
                       this.GcmUrl,
                       this.ListOfQuoteDetail.FirstOrDefault().Price,
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
                       ListOfQuoteDetail);
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

        public bool CaptureBackendOrderNumberFromOgXmlAndDbForAsn(string ogXmlMessage, string expectedDpidMessage)
        {
            return !string.IsNullOrEmpty(this.poNumber)
                   && this.poOperations.CaptureBackendOrderNumberFromOgXmlAndDb(
                       this.poNumber,
                       ogXmlMessage,
                       this.GcmUrl,
                       expectedDpidMessage);
        }

        public bool VerifyFulfillmentUnitsForAsn(string expectedDpidMessage, string mapperRequestMessage, string ogXmlMessage)
        {
            return !string.IsNullOrEmpty(this.poNumber)
                   && this.poOperations.VerifyFulfillmentUnits(
                       this.poNumber,
                       expectedDpidMessage,
                       mapperRequestMessage,
                       ogXmlMessage,
                       this.GcmUrl);
        }

        public bool MatchValuesInPoXmlAndMapperXml(string expectedDpidMessage, string mapperRequestMessage, string quote)
        {
            return !string.IsNullOrEmpty(this.poNumber)
                   && this.poOperations.MatchValuesInPoXmlAndMapperXml(
                       this.poNumber,
                       expectedDpidMessage,
                       mapperRequestMessage,
                       this.ProfileName, quote);
        }
    }
}
