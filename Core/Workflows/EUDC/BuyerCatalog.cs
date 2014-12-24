using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using Modules.Channel.B2B.Core.Pages;
using Modules.Channel.B2B.Core.Workflows.Common;
using Modules.Channel.B2B.Common;
using System.Xml.Linq;

namespace Modules.Channel.B2B.Core.Workflows.EUDC
{
    public class BuyerCatalog
    {
        private IWebDriver webDriver;

        private PoOperations poOperations;

        private B2BQaToolsPage B2BQaToolsPage
        {
            get
            {
                return new B2BQaToolsPage(webDriver);
            }
        }

        public BuyerCatalog(IWebDriver driver)
        {
            this.webDriver = driver;
            poOperations = new PoOperations(webDriver);
        }

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

        private B2BCatalogViewer B2BCatalogViewer
        {
            get
            {
                return new B2BCatalogViewer(webDriver);
            }
        }

        // Creates Buyer Catalog and generates XML for PO submission
        public bool CreateBhcPo(
            string customerName,
            string identityName,
            string validityEnd,
            string emailAddress,
            string configurationType,
            string deploymentMode,
            string orderIdBase,
            string customerId,
            RunEnvironment environment,
            string targetUrl,
            Workflow workflow,
            string crtFilePath,
            string templateName,
            string gcmUrl)
        {
             B2BHomePage.ClickOnBuyerCatalogLink();
            var threadId = B2BCreateBuyerCatalogPage.GenerateCatalog(
                customerName,
                identityName,
                validityEnd,
                emailAddress,
                configurationType);
            B2BCreateBuyerCatalogPage.GoToBuyerCatalogListPage();
            B2BBuyerCatalogListPage.SearchForBuyerCatalog(customerName);
            if (!B2BBuyerCatalogListPage.CheckCatalogAvailabilityAndAct(threadId))
            {
                Console.WriteLine("The catalog status is not = 'Available'. Retrying....");
                for (var i = 0; i < 3; i++)
                {
                    System.Threading.Thread.Sleep(10000);
                    Console.WriteLine("Retry No. {0}", i + 1);
                    B2BBuyerCatalogListPage.SearchForBuyerCatalog(customerName);
                    if (B2BBuyerCatalogListPage.CheckCatalogAvailabilityAndAct(threadId))
                    {
                        break;
                    }

                    if (i == 2)
                    {
                        Console.WriteLine("The catalog status is still not 'Available'. No. of retries {0}", i + 1);
                        return false;
                    }
                }
            }

            var orderId = orderIdBase + DateTime.Today.ToString("yyyyMMdd") + DateTime.Now.ToString("hhmmss");
            string baseItemPrice;
            var catalogPartId = B2BCatalogViewer.GetCatalogPartIdAndBaseUnitPrice(out baseItemPrice);
            catalogPartId = "BHC:" + catalogPartId;

            var poXml = PoXmlGenerator.GeneratorPoCXml(
                templateName,
                identityName,
                deploymentMode,
                orderId,
                baseItemPrice,
                catalogPartId,
                customerId);

            var parentWindow = webDriver.CurrentWindowHandle;

            string poNumber;

            B2BCatalogViewer.ClickQaTools3();

            if (!poOperations.SubmitXmlForPoCreation(poXml, environment.ToString(), targetUrl, out poNumber))
            {
                return false;
            }

            webDriver.SwitchTo().Window(parentWindow);
            B2BCatalogViewer.GoToHomePage();
            return this.VerifyPoCreation(poNumber, workflow, environment, crtFilePath, customerId, gcmUrl, baseItemPrice);
        }

        public bool VerifyPoCreation(string poNumber, Workflow workflow, RunEnvironment environment, string crtFilePath, string crtEndUserId, string gcmUrl, string baseItemPrice)
        {
            return poOperations.AllOperations(poNumber, workflow, environment, crtFilePath, crtEndUserId, gcmUrl, baseItemPrice);
        }

        public void SelectEnvironment(string environment)
        {
            B2BHomePage.SelectEnvironment(environment);
        }
    }
}
