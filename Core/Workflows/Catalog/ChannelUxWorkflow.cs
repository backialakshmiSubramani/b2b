using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.UI.WebControls;
using Dell.Adept.UI.Web.Support.Extensions.WebElement;
using FluentAssertions;
using Microsoft.BusinessData.MetadataModel;
using Modules.Channel.B2B.Core.Pages;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using Modules.Channel.B2B.Common;
using System.IO;
using System.Configuration;
using CatalogTests.Common.CatalogXMLTemplates;
using Modules.Channel.Utilities;
using Modules.Channel.B2B.DAL.ChannelCatalog;
using Modules.Channel.B2B.DAL;
using Microsoft.Exchange.WebServices.Data;

namespace Modules.Channel.B2B.Core.Workflows.Catalog
{
    public class ChannelUxWorkflow
    {
        private IWebDriver webDriver;
        private B2BChannelUx B2BChannelUx;
        private IJavaScriptExecutor javaScriptExecutor;

        /// <summary>
        /// Constructor for ChannelUxWorkflow
        /// </summary>
        /// <param name="webDriver"></param>
        public ChannelUxWorkflow(IWebDriver webDriver)
        {
            this.webDriver = webDriver;
            javaScriptExecutor = (IJavaScriptExecutor)this.webDriver;
        }

        /// <summary>
        /// Verifies links on Channel UX page.
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="linksTextTestData"></param>
        public bool Verifyandretrievelinks(string environment, string linksTextTestData)
        {
            string[] LinkTestStringValue = linksTextTestData.Split(',');
            B2BChannelUx = new B2BChannelUx(webDriver);
            B2BChannelUx.SelectEnvironment(environment);
            webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromMinutes(1));
            for (int j = 1; j <= LinkTestStringValue.Length; j++)
            {
                string TestData = LinkTestStringValue[j - 1];
                var Link_Locator = webDriver.FindElement(By.XPath("//table/tbody/tr[" + j + "]/td/a/h4"));
                string LinkLocatorText = Link_Locator.Text;
                if (!LinkLocatorText.Contains(TestData))
                {
                    return false;
                }
                else
                {
                    Link_Locator.Click();
                    webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromMinutes(2));
                    if (webDriver.PageSource.Contains("Auto Catalog List") ||
                        webDriver.PageSource.Contains("Packaging Data") ||
                        webDriver.PageSource.Contains("Retrieve/Create Catalog") ||
                        webDriver.PageSource.Contains("Auto Catalog Part Viewer")
                        )
                    {
                        B2BChannelUx.Rightside_Menu.Click();
                        B2BChannelUx.Rightside_dropdown_Home.Click();
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public string SearchAndDownloadCatalog(B2BEnvironment environment, Region region, string profileName, string identityName, DateTime beforeSchedTime, CatalogOperation operation)
        {
            webDriver.Navigate().GoToUrl(ConfigurationManager.AppSettings["AutoCatalogListPageUrl"] + ((environment == B2BEnvironment.Production) ? "P" : "U"));

            CPTAutoCatalogInventoryListPage autoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            autoCatalogListPage.SearchCatalogs(region, profileName, identityName);
            autoCatalogListPage.WaitForCatalogInSearchResult(beforeSchedTime.ConvertToUtcTimeZone(), operation);

            autoCatalogListPage.CatalogsTable.GetCellValue(1, "Last Status Date").Trim().ConvertToDateTime().AddMinutes(1).Should().BeAfter(beforeSchedTime.ConvertToUtcTimeZone(), "Catalog is not displayed in Search Result");
            autoCatalogListPage.CatalogsTable.GetCellValue(1, "Type").Should().Be("Original", "Expected Catalog type is incorrect");
            autoCatalogListPage.CatalogsTable.GetCellValue(1, "Status").Should().Be(operation == CatalogOperation.Create ? "Created" : "Published", "Catalog creation failed");

            autoCatalogListPage.GetDownloadButton(1).Click();
            string downloadPath = ConfigurationManager.AppSettings["CatalogDownloadPath"];

            webDriver.WaitForDownLoadToComplete(downloadPath, identityName, beforeSchedTime, TimeSpan.FromMinutes(1));
            string fileName = new DirectoryInfo(downloadPath).GetFiles().AsEnumerable()
                .Where(file => file.Name.Contains(identityName.ToUpper()) && file.CreationTime > beforeSchedTime)
                .FirstOrDefault().FullName;

            return fileName;
        }

        /// <summary>
        /// Compares the content of Catalog XML
        /// </summary>
        /// <param name="catalogItemType">Catalog Item Type like ConfigWithDefaultOptions or SNP</param>
        /// <param name="catalogType">Catalog Type like Original or Delta</param>
        /// <param name="identityName">Name of the Identity</param>
        /// <param name="filePath">XML file path</param>
        /// <param name="anyTimeAfter">Time after which the XML is created</param>
        /// <param name="catalogItemBaseSKU">One of the Catalog Item SKU for which data needs to be validated</param>
        /// <returns></returns>
        public bool ValidateCatalogXML(Region region, CatalogItemType catalogItemType, CatalogType catalogType, string identityName, string filePath, DateTime anyTimeAfter)
        {
            string schemaPath = Path.Combine(System.Environment.CurrentDirectory, "CatalogSchema.xsd");

            string message = XMLSchemaValidator.ValidateSchema(filePath, schemaPath);
            message.Should().Be(string.Empty, "Error: One or more tags failed scehma validation. Please check the log for complete details");

            B2BXML xmlObj = XMLDeserializer<B2BXML>.DeserializeFromXmlFile(filePath);
            string fileName = new FileInfo(filePath).Name;
            string catalogName = fileName.Split('.')[0];

            CatalogProp catalogProp = new CatalogProp(region);

            //CatalogMaster_Auto catalogMaster = ChannelCatalogProdDataAccess.GetCatalog(identityName, anyTimeAfter);

            bool matchFlag = true;
            Console.WriteLine("Catalog Header Data Validation");
            matchFlag &= UtilityMethods.CompareValues<string>("CatalogFormatType", xmlObj.BuyerCatalog.CatalogHeader.CatalogFormatType, catalogProp.CatalogFormatType);
            matchFlag &= UtilityMethods.CompareValues<string>("CatalogName", xmlObj.BuyerCatalog.CatalogHeader.CatalogName, catalogName);
            matchFlag &= UtilityMethods.CompareValues<string>("EffectiveDate", xmlObj.BuyerCatalog.CatalogHeader.EffectiveDate.ToString(), DateTime.Now.ConvertToUtcTimeZone().ToString("yyyy-MM-ddT00:00:00", System.Globalization.CultureInfo.InstalledUICulture));
            matchFlag &= UtilityMethods.CompareValues<string>("ExpirationDate", xmlObj.BuyerCatalog.CatalogHeader.ExpirationDate.ToString(), DateTime.Now.ConvertToUtcTimeZone().AddMonths(6).ToString("yyyy-MM-ddT00:00:00", System.Globalization.CultureInfo.InstalledUICulture));
            matchFlag &= UtilityMethods.CompareValues<string>("CountryCode", xmlObj.BuyerCatalog.CatalogHeader.CountryCode, catalogProp.CountryCode);
            matchFlag &= UtilityMethods.CompareValues<string>("SubLocationCode", xmlObj.BuyerCatalog.CatalogHeader.SubLocationCode, catalogProp.SubLocationCode);
            matchFlag &= UtilityMethods.CompareValues<string>("Buyer", xmlObj.BuyerCatalog.CatalogHeader.Buyer, identityName.ToUpper());
            matchFlag &= UtilityMethods.CompareValues<string>("RequesterEmailId", xmlObj.BuyerCatalog.CatalogHeader.RequesterEmailId, catalogProp.RequesterEmailId);
            //matchFlag &= UtilityMethods.CompareValues<long>(xmlObj.BuyerCatalog.CatalogHeader.CatalogId, catalogMaster.CatalogId); // Cannot validate as its dynamically generated
            xmlObj.BuyerCatalog.CatalogHeader.ItemCount.Should().BeGreaterThan(0, "Error: Item Count in header is invalid");
            matchFlag &= UtilityMethods.CompareValues<int>("ItemCount", xmlObj.BuyerCatalog.CatalogHeader.ItemCount, UtilityMethods.ConvertValue<byte>(xmlObj.BuyerCatalog.CatalogDetails.CatalogItem.Count()));
            matchFlag &= UtilityMethods.CompareValues<string>("SupplierId", xmlObj.BuyerCatalog.CatalogHeader.SupplierId, catalogProp.SupplierId);
            matchFlag &= UtilityMethods.CompareValues<string>("Comments", xmlObj.BuyerCatalog.CatalogHeader.Comments, catalogProp.Comments);
            matchFlag &= UtilityMethods.CompareValues<bool>("SNPEnabled", xmlObj.BuyerCatalog.CatalogHeader.SNPEnabled, (catalogItemType == CatalogItemType.SNP ? true : false));
            matchFlag &= UtilityMethods.CompareValues<bool>("SYSConfigEnabled", xmlObj.BuyerCatalog.CatalogHeader.SYSConfigEnabled, (catalogItemType == CatalogItemType.Systems ? true : false));
            matchFlag &= UtilityMethods.CompareValues<bool>("StdConfigEnabled", xmlObj.BuyerCatalog.CatalogHeader.StdConfigEnabled, (catalogItemType == CatalogItemType.ConfigWithDefaultOptions ? true : false));
            matchFlag &= UtilityMethods.CompareValues<bool>("StdConfigUpSellDownSellEnabled", xmlObj.BuyerCatalog.CatalogHeader.StdConfigUpSellDownSellEnabled, (catalogItemType == CatalogItemType.ConfigWithUpsellDownsell ? true : false));
            matchFlag &= UtilityMethods.CompareValues<string>("Region", xmlObj.BuyerCatalog.CatalogHeader.Region, catalogProp.Region.ConvertToString());
            matchFlag &= UtilityMethods.CompareValues<bool>("GPEnabled", xmlObj.BuyerCatalog.CatalogHeader.GPEnabled, catalogProp.GPEnabled);
            matchFlag &= UtilityMethods.CompareValues<object>("GPShipToCurrency", xmlObj.BuyerCatalog.CatalogHeader.GPShipToCurrency, catalogProp.GPShipToCurrency);
            matchFlag &= UtilityMethods.CompareValues<string>("GPShipToCountry", xmlObj.BuyerCatalog.CatalogHeader.GPShipToCountry, catalogProp.GPShipToCountry);
            matchFlag &= UtilityMethods.CompareValues<string>("GPShipToLanguage", xmlObj.BuyerCatalog.CatalogHeader.GPShipToLanguage, catalogProp.GPShipToLanguage);
            matchFlag &= UtilityMethods.CompareValues<string>("GPPurchaseOption", xmlObj.BuyerCatalog.CatalogHeader.GPPurchaseOption, catalogProp.GPPurchaseOption);
            matchFlag &= UtilityMethods.CompareValues<bool>("CPFEnabled", xmlObj.BuyerCatalog.CatalogHeader.CPFEnabled, catalogProp.CPFEnabled);
            matchFlag &= UtilityMethods.CompareValues<string>("IdentityUserName", xmlObj.BuyerCatalog.CatalogHeader.IdentityUserName, identityName.ToUpper());
            matchFlag &= UtilityMethods.CompareValues<int>("GracePeriod", xmlObj.BuyerCatalog.CatalogHeader.GracePeriod, catalogProp.GracePeriod);
            matchFlag &= UtilityMethods.CompareValues<long>("ProfileId", xmlObj.BuyerCatalog.CatalogHeader.ProfileId, catalogProp.ProfileId);
            matchFlag &= UtilityMethods.CompareValues<string>("CustomerID", xmlObj.BuyerCatalog.CatalogHeader.CustomerID, catalogProp.CustomerID);
            matchFlag &= UtilityMethods.CompareValues<string>("AccessGroup", xmlObj.BuyerCatalog.CatalogHeader.AccessGroup, catalogProp.AccessGroup);
            matchFlag &= UtilityMethods.CompareValues<string>("MessageType", xmlObj.BuyerCatalog.CatalogHeader.MessageType, catalogProp.MessageType);
            matchFlag &= UtilityMethods.CompareValues<string>("CatalogType", xmlObj.BuyerCatalog.CatalogHeader.CatalogType, catalogType.ToString());
            matchFlag &= UtilityMethods.CompareValues<string>("Sender", xmlObj.BuyerCatalog.CatalogHeader.Sender, catalogProp.Sender);
            matchFlag &= UtilityMethods.CompareValues<string>("Receiver", xmlObj.BuyerCatalog.CatalogHeader.Receiver, identityName.ToUpper());
            matchFlag &= UtilityMethods.CompareValues<string>("CatalogDate", xmlObj.BuyerCatalog.CatalogHeader.CatalogDate.ToString(), DateTime.Now.ConvertToUtcTimeZone().ToString("MM/dd/yyyy 0:00:00", System.Globalization.CultureInfo.InvariantCulture));

            Console.WriteLine("Catalog Items Data Validation");
            CatalogItem catalogItem = xmlObj.BuyerCatalog.CatalogDetails.CatalogItem.FirstOrDefault();
            catalogItem.Should().NotBeNull("Error: No Catalog Items found");
            matchFlag &= UtilityMethods.CompareValues<CatalogItemType>("CatalogItemType", catalogItem.CatalogItemType, catalogItemType);

            return matchFlag;
        }

        public void ValidateCatalogEMails(string identityName, DateTime anyTimeAfter, CatalogOperation operation)
        {
            EmailHelper emailHelper = new EmailHelper();
            List<Item> emails = emailHelper.GetEmails("US B2B Support", identityName, anyTimeAfter, operation);

            if (operation == CatalogOperation.Create)
            {
                emails.Count().Should().Be(1, "Error: Email count validation failed");
                emails.ElementAt(0).Subject.Should().Contain("Test - B2B Original Catalog Create SUCCESS", "Error: Email Subject validation failed");
            }
            else if (operation == CatalogOperation.CreateAndPublish)
            {
                emails.Count().Should().Be(2, "Error: Email count validation failed");
                emails.Where(e => e.Subject.Contains("Create")).ElementAt(0).Subject.Should().Contain("Test - B2B Original Catalog Create SUCCESS", "Error: Create Email Subject validation failed");
                emails.Where(e => e.Subject.Contains("Publish")).ElementAt(0).Subject.Should().Contain("Test - B2B Original Catalog Publish SUCCESS", "Error: Publish Email Subject validation failed");
            }
        }

        internal void PublishCatalogByClickOnce(B2BEnvironment environment, string profileName, string identityName, CatalogType catalogType)
        {
            webDriver.Navigate().GoToUrl(ConfigurationManager.AppSettings["TestHarnessPageUrl"] + ((environment == B2BEnvironment.Production) ? "P" : "U"));
            B2BChannelUx b2BChannelUx = new B2BChannelUx(webDriver);

            if (environment == B2BEnvironment.Production)
                b2BChannelUx.ProductionEnvRadioButton.Click();
            else if (environment == B2BEnvironment.Preview)
                b2BChannelUx.PreviewEnvRadioButton.Click();

            b2BChannelUx.SelectOption(b2BChannelUx.SelectCustomerProfileDiv, profileName);
            b2BChannelUx.SelectOption(b2BChannelUx.SelectProfileIdentityDiv, identityName.ToUpper());

            if (catalogType == CatalogType.Original)
                b2BChannelUx.OriginalRadioButton.Click();
            else if (catalogType == CatalogType.Delta)
                b2BChannelUx.DeltaRadioButton.Click();

            b2BChannelUx.ClickToPublishButton.Click();

            IAlert successAlert = webDriver.WaitGetAlert(CatalogTimeOuts.AlertTimeOut);
            successAlert.Accept();
        }
    }
}
