using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Dell.Adept.UI.Web.Support.Extensions.WebElement;
using FluentAssertions;
using Modules.Channel.B2B.Core.Pages;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using Modules.Channel.B2B.Common;
using System.IO;
using System.Configuration;
using Modules.Channel.Utilities;
using Microsoft.Exchange.WebServices.Data;
using Modules.Channel.B2B.CatalogXMLTemplates;
using System.Security.Principal;
using System.Xml;
using OpenQA.Selenium.Support.UI;

namespace Modules.Channel.B2B.Core.Workflows.Catalog
{
    public class ChannelUxWorkflow
    {
        private IWebDriver webDriver;
        private B2BChannelUx B2BChannelUx;
        private IJavaScriptExecutor javaScriptExecutor;
        private string windowsLogin;
        private const string MMDDYYYY = "MM/dd/yyyy";
        /// <summary>
        /// Constructor for ChannelUxWorkflow
        /// </summary>
        /// <param name="webDriver"></param>
        public ChannelUxWorkflow(IWebDriver webDriver)
        {
            this.webDriver = webDriver;
            javaScriptExecutor = (IJavaScriptExecutor)this.webDriver;
            windowsLogin = WindowsIdentity.GetCurrent().Name.Split('\\')[1].ToLowerInvariant();
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
                string TestData = LinkTestStringValue[j - 1].Replace("-", "&");
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
                    if (webDriver.PageSource.Contains("Inventory List") ||
                        webDriver.PageSource.Contains("Packaging Data") ||
                        webDriver.PageSource.Contains("Create Instant Catalog") ||
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
        public bool ValidateCatalogXML(CatalogItemType[] catalogItemType, CatalogType catalogType, string identityName, string filePath, DateTime anyTimeAfter, ConfigRules configRules)
        {
            string schemaPath = string.Empty;
            if (configRules == ConfigRules.SPL)
            {
                schemaPath = Path.Combine(System.Environment.CurrentDirectory, "SPLCatalogSchema.xsd");
            string message = XMLSchemaValidator.ValidateSchema(filePath, schemaPath);
            message.Should().Be(string.Empty, "Error: One or more tags failed scehma validation. Please check the log for complete details");
            }
            else
            {
                schemaPath = Path.Combine(System.Environment.CurrentDirectory, "CatalogSchema.xsd");
                string message = XMLSchemaValidator.ValidateSchema(filePath, schemaPath);
                message.Should().Be(string.Empty, "Error: One or more tags failed scehma validation. Please check the log for complete details");
            }

            B2BXML actualcatalogXML = XMLDeserializer<B2BXML>.DeserializeFromXmlFile(filePath);
            CatalogDetails actualCatalogDetails = actualcatalogXML.BuyerCatalog.CatalogDetails;
            CatalogHeader actualCatalogHeader = actualcatalogXML.BuyerCatalog.CatalogHeader;

            string expectedCatalogFilePath = catalogType == CatalogType.Original
                ? Path.Combine(System.Environment.CurrentDirectory, "Catalog_OC_Expected.xml")
                : Path.Combine(System.Environment.CurrentDirectory, "Catalog_DC_Expected.xml");

            B2BXML expectedCatalogXML = XMLDeserializer<B2BXML>.DeserializeFromXmlFile(expectedCatalogFilePath);
            CatalogDetails expectedCatalogDetails = expectedCatalogXML.BuyerCatalog.CatalogDetails;
            CatalogHeader expectedCatalogHeader = expectedCatalogXML.BuyerCatalog.CatalogHeader;

            string fileName = new FileInfo(filePath).Name;
            string catalogName = fileName.Split('.')[0];

            //int itemCount = 0;
            bool matchFlag = true;
            foreach (CatalogItemType itemType in catalogItemType)
            {
                Console.WriteLine("Catalog Items validation for : " + itemType.ConvertToString());

                IEnumerable<CatalogItem> actualCatalogItems = actualCatalogDetails.CatalogItem.Where(ci => ci.CatalogItemType == itemType);
                IEnumerable<CatalogItem> expectedCatalogItems = expectedCatalogDetails.CatalogItem.Where(ci => ci.CatalogItemType == itemType);

                string matchingOrderCode = string.Empty;

                if (itemType.Equals(CatalogItemType.ConfigWithDefaultOptions) || itemType.Equals(CatalogItemType.ConfigWithUpsellDownsell) || itemType.Equals(CatalogItemType.Systems))
                {
                    switch (configRules)
                    {
                        case ConfigRules.DuplicateBPN:
                            expectedCatalogItems = expectedCatalogItems.Where(ci => ci.ShortName.StartsWith("Duplicate BPN"));

                            matchingOrderCode = actualCatalogItems.Select(ci => ci.ItemOrderCode).ToList().Intersect(expectedCatalogItems.Select(ci => ci.ItemOrderCode).ToList()).First();
                            actualCatalogItems = actualCatalogItems.Where(ci => ci.ItemOrderCode == matchingOrderCode);
                            expectedCatalogItems = expectedCatalogItems.Where(ci => ci.ItemOrderCode == matchingOrderCode);
                            break;

                        case ConfigRules.NullBPN:
                            expectedCatalogItems = expectedCatalogItems.Where(ci => ci.ShortName.StartsWith("Null BPN"));
                            matchingOrderCode = actualCatalogItems.Select(ci => ci.ItemOrderCode).ToList().Intersect(expectedCatalogItems.Select(ci => ci.ItemOrderCode).ToList()).First();
                            actualCatalogItems = actualCatalogItems.Where(ci => ci.ItemOrderCode == matchingOrderCode);
                            expectedCatalogItems = expectedCatalogItems.Where(ci => ci.ItemOrderCode == matchingOrderCode);
                            break;

                        case ConfigRules.WithDefOptions:
                            expectedCatalogItems = expectedCatalogItems.Where(ci => ci.Modules.Module.Count != 0);
                            matchingOrderCode = actualCatalogItems.Select(ci => ci.ItemOrderCode).ToList().Intersect(expectedCatalogItems.Select(ci => ci.ItemOrderCode).ToList()).First();
                            actualCatalogItems = actualCatalogItems.Where(ci => ci.ItemOrderCode == matchingOrderCode);
                            expectedCatalogItems = expectedCatalogItems.Where(ci => ci.ItemOrderCode == matchingOrderCode);
                            break;

                        case ConfigRules.LeadTimeOff:
                            expectedCatalogItems = expectedCatalogDetails.CatalogItem.Where(ci => ci.ShortName.StartsWith("Lead Time"));
                            matchingOrderCode = actualCatalogItems.Select(ci => ci.ItemOrderCode).ToList().Intersect(expectedCatalogItems.Select(ci => ci.ItemOrderCode).ToList()).First();
                            actualCatalogItems = actualCatalogItems.Where(ci => ci.ItemOrderCode == matchingOrderCode);
                            expectedCatalogItems = expectedCatalogItems.Where(ci => ci.ItemOrderCode == matchingOrderCode);
                            break;

                        case ConfigRules.LeadTimeON:
                            expectedCatalogItems = expectedCatalogDetails.CatalogItem.Where(ci => ci.ShortName.StartsWith("Lead Time") && (ci.ItemType.Equals("") ||
                           ci.ItemType.Equals("BTO") || (ci.ItemType.Equals("BTS") && ci.LeadTime < 3)));
                            matchingOrderCode = actualCatalogItems.Select(ci => ci.ItemOrderCode).ToList().Intersect(expectedCatalogItems.Select(ci => ci.ItemOrderCode).ToList()).First();
                            actualCatalogItems = actualCatalogItems.Where(ci => ci.ItemOrderCode == matchingOrderCode);
                            expectedCatalogItems = expectedCatalogItems.Where(ci => ci.ItemOrderCode == matchingOrderCode);
                            break;

                        case ConfigRules.SPL:
                            expectedCatalogItems = expectedCatalogItems.Where(ci => ci.AutoCategory == "SPL" && ci.CatalogItemType.Equals(itemType));
                            matchingOrderCode = actualCatalogItems.Select(ci => ci.ItemOrderCode).ToList().Intersect(expectedCatalogItems.Select(ci => ci.ItemOrderCode).ToList()).FirstOrDefault();
                            actualCatalogItems = actualCatalogItems.Where(ci => ci.ItemOrderCode == matchingOrderCode);
                            expectedCatalogItems = expectedCatalogItems.Where(ci => ci.ItemOrderCode == matchingOrderCode);
                            break;

                        default:
                            matchingOrderCode = actualCatalogItems.Select(ci => ci.ItemOrderCode).ToList().Intersect(expectedCatalogItems.Select(ci => ci.ItemOrderCode).ToList()).First();
                            actualCatalogItems = actualCatalogItems.Where(ci => ci.ItemOrderCode == matchingOrderCode);
                            expectedCatalogItems = expectedCatalogItems.Where(ci => ci.ItemOrderCode == matchingOrderCode);
                            break;
                    }
                }
                else if (itemType.Equals(CatalogItemType.SNP))
                {
                    if (configRules == ConfigRules.SPL)
                    {
                        expectedCatalogItems = expectedCatalogItems.Where(ci => ci.AutoCategory == "SPL" && ci.CatalogItemType.Equals(itemType));
                        matchingOrderCode = actualCatalogItems.Select(ci => ci.BaseSKUId).ToList().Intersect(expectedCatalogItems.Select(ci => ci.BaseSKUId).ToList()).FirstOrDefault();
                        actualCatalogItems = actualCatalogItems.Where(ci => ci.BaseSKUId == matchingOrderCode);
                        expectedCatalogItems = expectedCatalogItems.Where(ci => ci.BaseSKUId == matchingOrderCode);                  
                    }
                    else
                    {
                    matchingOrderCode = actualCatalogItems.Select(ci => ci.BaseSKUId).ToList().Intersect(expectedCatalogItems.Select(ci => ci.BaseSKUId).ToList()).First();
                    actualCatalogItems = actualCatalogItems.Where(ci => ci.BaseSKUId == matchingOrderCode);
                    expectedCatalogItems = expectedCatalogItems.Where(ci => ci.BaseSKUId == matchingOrderCode);
                }
                }

                matchFlag &= ValidateCatalogItems(actualCatalogItems, expectedCatalogItems, configRules);
                //itemCount += expectedCatalogItems.Count();
            }

            matchFlag &= ValidateCatalogHeader(actualCatalogHeader, expectedCatalogHeader, catalogItemType, identityName, catalogName, configRules);

            return matchFlag;
        }


        public bool ValidateDeltaCatalog(CatalogItemType[] catalogItemType, CatalogType catalogType, string identityName, string filePath, DateTime anyTimeAfter, DeltaChange[] deltaChanges, bool excludeNonChangedItems = false)
        {
            string schemaPath = Path.Combine(System.Environment.CurrentDirectory, "CatalogSchema.xsd");

            string message = XMLSchemaValidator.ValidateSchema(filePath, schemaPath);
            message.Should().Be(string.Empty, "Error: One or more tags failed scehma validation. Please check the log for complete details");

            B2BXML actualcatalogXML = XMLDeserializer<B2BXML>.DeserializeFromXmlFile(filePath);
            CatalogDetails actualCatalogDetails = actualcatalogXML.BuyerCatalog.CatalogDetails;
            CatalogHeader actualCatalogHeader = actualcatalogXML.BuyerCatalog.CatalogHeader;

            B2BXML expectedCatalogXML = XMLDeserializer<B2BXML>.DeserializeFromXmlFile(Path.Combine(System.Environment.CurrentDirectory, "Catalog_DC_Expected.xml"));
            CatalogDetails expectedCatalogDetails = expectedCatalogXML.BuyerCatalog.CatalogDetails;
            CatalogHeader expectedCatalogHeader = expectedCatalogXML.BuyerCatalog.CatalogHeader;

            if (excludeNonChangedItems)
                actualCatalogDetails.CatalogItem.Where(ci => ci.DeltaComments.Operation == "No Change").Count().Should().Be(0, "Error: Delta catalog contains Non-Changed items when Exclude Unchanged items flag is ON");

            string fileName = new FileInfo(filePath).Name;
            string catalogName = fileName.Split('.')[0];

            bool matchFlag = true;
            foreach (CatalogItemType itemType in catalogItemType)
            {
                IEnumerable<CatalogItem> actualCatalogItems = actualCatalogDetails.CatalogItem.Where(ci => ci.CatalogItemType == itemType);
                IEnumerable<CatalogItem> expectedCatalogItems = expectedCatalogDetails.CatalogItem.Where(ci => ci.CatalogItemType == itemType);

                expectedCatalogItems.Count().Should().BeGreaterThan(0, string.Format("Expected catalog does not contain any products of type {0}", itemType));
                actualCatalogItems.Count().Should().BeGreaterThan(0, string.Format("Actual catalog does not contain any products of type {0}", itemType));

                actualCatalogItems.Count().Should().Be(deltaChanges.Count(), "Error: Unexpected catalog items are added to the catalog XML");

                if (itemType.Equals(CatalogItemType.ConfigWithDefaultOptions))
                {
                    foreach (DeltaChange deltaChange in deltaChanges)
                    {
                        switch (deltaChange)
                        {
                            case DeltaChange.Add:
                                expectedCatalogItems = expectedCatalogItems.Where(ci => ci.AutoCategory == "STD_DC_Add");
                                break;
                            case DeltaChange.Remove:
                                expectedCatalogItems = expectedCatalogItems.Where(ci => ci.DeltaComments.Operation == "STD_DC_Remove");
                                break;
                            case DeltaChange.Modify:
                                expectedCatalogItems = expectedCatalogItems.Where(ci => ci.DeltaComments.Operation == "STD_DC_Modify");
                                break;
                            case DeltaChange.NoChange:
                                expectedCatalogItems = expectedCatalogItems.Where(ci => ci.DeltaComments.Operation == "STD_DC_NoChange");
                                break;
                        }

                        actualCatalogItems = actualCatalogItems.Where(ci => expectedCatalogItems.Select(ec => ec.ItemOrderCode).Contains(ci.ItemOrderCode));

                        expectedCatalogItems.Count().Should().Be(actualCatalogItems.Count(), string.Format("Expected catalog items count did not match with actual catalog items count for {0}", deltaChange));

                        foreach (CatalogItem expectedcatalogItem in expectedCatalogItems)
                        {
                            CatalogItem actualCatalogItem = actualCatalogItems.Where(aci => aci.ItemOrderCode == expectedcatalogItem.ItemOrderCode).FirstOrDefault();

                            actualCatalogItem.Should().NotBeNull(string.Format("Actual catalog does not contain product with ordercode {0}", expectedcatalogItem.ItemOrderCode));

                            matchFlag &= ValidateCatalogItem(expectedcatalogItem, actualCatalogItem);
                        }
                    }
                }
            }

            matchFlag &= ValidateCatalogHeader(actualCatalogHeader, expectedCatalogHeader, catalogItemType, identityName, catalogName);

            return matchFlag;
        }

        ///// <summary>
        ///// Compares the content of Catalog XML
        ///// </summary>
        ///// <param name="catalogItemType">Catalog Item Type like ConfigWithDefaultOptions or SNP</param>
        ///// <param name="catalogType">Catalog Type like Original or Delta</param>
        ///// <param name="identityName">Name of the Identity</param>
        ///// <param name="filePath">XML file path</param>
        ///// <param name="anyTimeAfter">Time after which the XML is created</param>
        ///// <param name="catalogItemBaseSKU">One of the Catalog Item SKU for which data needs to be validated</param>
        ///// <returns></returns>
        //public bool ValidateCatalogXML(CatalogItemType[] catalogItemType, CatalogType catalogType, string identityName, string filePath, DateTime anyTimeAfter, ConfigRules configRules, DefaultOptions defaultOptions)
        //{
        //    string schemaPath = Path.Combine(System.Environment.CurrentDirectory, "CatalogSchema.xsd");

        //    string message = XMLSchemaValidator.ValidateSchema(filePath, schemaPath);
        //    message.Should().Be(string.Empty, "Error: One or more tags failed scehma validation. Please check the log for complete details");

        //    B2BXML actualcatalogXML = XMLDeserializer<B2BXML>.DeserializeFromXmlFile(filePath);
        //    CatalogDetails actualCatalogDetails = actualcatalogXML.BuyerCatalog.CatalogDetails;
        //    CatalogHeader actualCatalogHeader = actualcatalogXML.BuyerCatalog.CatalogHeader;

        //    string expectedCatalogFilePath = string.Empty;

        //    expectedCatalogFilePath = catalogType == CatalogType.Original
        //        ? Path.Combine(System.Environment.CurrentDirectory, "Catalog_OC_Expected.xml")
        //        : Path.Combine(System.Environment.CurrentDirectory, "Catalog_DC_Expected.xml");

        //    B2BXML expectedCatalogXML = XMLDeserializer<B2BXML>.DeserializeFromXmlFile(expectedCatalogFilePath);
        //    CatalogDetails expectedCatalogDetails = expectedCatalogXML.BuyerCatalog.CatalogDetails;
        //    CatalogHeader expectedCatalogHeader = expectedCatalogXML.BuyerCatalog.CatalogHeader;

        //    string fileName = new FileInfo(filePath).Name;
        //    string catalogName = fileName.Split('.')[0];

        //    //int itemCount = 0;
        //    bool matchFlag = true;
        //    foreach (CatalogItemType itemType in catalogItemType)
        //    {
        //        Console.WriteLine("Catalog Items validation for : " + itemType.ConvertToString());

        //        IEnumerable<CatalogItem> actualCatalogItems = actualCatalogDetails.CatalogItem.Where(ci => ci.CatalogItemType == itemType);
        //        IEnumerable<CatalogItem> expectedCatalogItems =
        //                expectedCatalogDetails.CatalogItem.Where(ci => ci.CatalogItemType == itemType);

        //        if (itemType.Equals(CatalogItemType.ConfigWithDefaultOptions) ||
        //            itemType.Equals(CatalogItemType.ConfigWithUpsellDownsell) ||
        //            itemType.Equals(CatalogItemType.Systems))
        //        {
        //            switch (configRules)
        //            {
        //                case ConfigRules.DuplicateBPN:
        //                    expectedCatalogItems = expectedCatalogItems.Where(ci => ci.ShortName.StartsWith("Duplicate BPN"));
        //                    break;
        //                case ConfigRules.NullBPN:
        //                    expectedCatalogItems = expectedCatalogItems.Where(ci => ci.ShortName.StartsWith("Null BPN"));
        //                    break;
        //                case ConfigRules.LeadTimeOff:
        //                    expectedCatalogItems =
        //                      expectedCatalogDetails.CatalogItem.Where(ci => ci.ShortName.StartsWith("Lead Time"));
        //                    break;
        //                case ConfigRules.LeadTimeON:
        //                    expectedCatalogItems =
        //                      expectedCatalogDetails.CatalogItem.Where(ci => ci.ShortName.StartsWith("Lead Time") && (ci.ItemType.Equals("") ||
        //                       ci.ItemType.Equals("BTO") || (ci.ItemType.Equals("BTS") && ci.LeadTime < 3)));
        //                    break;
        //                default:
        //                    string matchingOrderCode = actualCatalogItems.Select(ci => ci.ItemOrderCode).ToList().Intersect(expectedCatalogItems.Select(ci => ci.ItemOrderCode).ToList()).First();
        //                    actualCatalogItems = actualCatalogItems.Where(ci => ci.ItemOrderCode == matchingOrderCode);
        //                    expectedCatalogItems = expectedCatalogItems.Where(ci => ci.ItemOrderCode == matchingOrderCode);

        //                    //expectedCatalogItems = expectedCatalogItems.Where(ci => ci.ShortName.StartsWith("STD Config"));
        //                    break;
        //            }

        //        }

        //        else if(itemType.Equals(CatalogItemType.SNP))
        //        {
        //            string matchingOrderCode = actualCatalogItems.Select(ci => ci.BaseSKUId).ToList().Intersect(expectedCatalogItems.Select(ci => ci.BaseSKUId).ToList()).First();
        //            actualCatalogItems = actualCatalogItems.Where(ci => ci.BaseSKUId == matchingOrderCode);
        //            expectedCatalogItems = expectedCatalogItems.Where(ci => ci.BaseSKUId == matchingOrderCode);
        //        }

        //        matchFlag &= ValidateCatalogItems(actualCatalogItems, expectedCatalogItems, defaultOptions);

        //        //itemCount += expectedCatalogItems.Count();
        //    }

        //    matchFlag &= ValidateCatalogHeader(actualCatalogHeader, expectedCatalogHeader, catalogItemType, identityName, catalogName);
        //    return matchFlag;
        //}

        /// <summary>
        /// Validate all the tags of Catalog Header
        /// </summary>
        /// <param name="actualCatalog"></param>
        /// <param name="expectedCatalog"></param>
        /// <param name="catalogItemType"></param>
        /// <param name="identityName"></param>
        /// <param name="catalogName"></param>
        /// <returns></returns>
        public bool ValidateCatalogHeader(CatalogHeader actualCatalogHeader, CatalogHeader expectedCatalogHeader, CatalogItemType[] catalogItemType, string identityName, string catalogName, ConfigRules configRules = ConfigRules.None)
        {
            bool matchFlag = true;
            Console.WriteLine("Catalog Header Data Validation");
            matchFlag &= UtilityMethods.CompareValues<string>("CatalogFormatType", actualCatalogHeader.CatalogFormatType, expectedCatalogHeader.CatalogFormatType);
            matchFlag &= UtilityMethods.CompareValues<string>("CatalogName", actualCatalogHeader.CatalogName, catalogName);
            matchFlag &= UtilityMethods.CompareValues<string>("EffectiveDate", actualCatalogHeader.EffectiveDate.ToString(), DateTime.Now.ConvertToUtcTimeZone().ToString("yyyy-MM-ddT00:00:00", System.Globalization.CultureInfo.InstalledUICulture));
            matchFlag &= UtilityMethods.CompareValues<string>("ExpirationDate", actualCatalogHeader.ExpirationDate.ToString(), DateTime.Now.ConvertToUtcTimeZone().AddDays(180).ToString("yyyy-MM-ddT00:00:00", System.Globalization.CultureInfo.InstalledUICulture));
            matchFlag &= UtilityMethods.CompareValues<string>("CountryCode", actualCatalogHeader.CountryCode, expectedCatalogHeader.CountryCode);
            matchFlag &= UtilityMethods.CompareValues<string>("SubLocationCode", actualCatalogHeader.SubLocationCode, expectedCatalogHeader.SubLocationCode);
            matchFlag &= UtilityMethods.CompareValues<string>("Buyer", actualCatalogHeader.Buyer, identityName.ToUpper());
            //matchFlag &= UtilityMethods.CompareValues<string>("RequesterEmailId", actualCatalogHeader.RequesterEmailId, expectedCatalogHeader.RequesterEmailId); // Excluding this field as this is dependent on the user who modifies the profile
            //matchFlag &= UtilityMethods.CompareValues<int>("ItemCount", actualCatalogHeader.ItemCount, itemCount);
            matchFlag &= UtilityMethods.CompareValues<string>("SupplierId", actualCatalogHeader.SupplierId, expectedCatalogHeader.SupplierId);
            matchFlag &= UtilityMethods.CompareValues<string>("Comments", actualCatalogHeader.Comments, expectedCatalogHeader.Comments);
            matchFlag &= UtilityMethods.CompareValues<bool>("SNPEnabled", actualCatalogHeader.SNPEnabled, (catalogItemType.Contains(CatalogItemType.SNP) ? true : false));
            matchFlag &= UtilityMethods.CompareValues<bool>("SYSConfigEnabled", actualCatalogHeader.SYSConfigEnabled, (catalogItemType.Contains(CatalogItemType.Systems) ? true : false));
            matchFlag &= UtilityMethods.CompareValues<bool>("StdConfigEnabled", actualCatalogHeader.StdConfigEnabled, (catalogItemType.Contains(CatalogItemType.ConfigWithDefaultOptions) ? true : false));
            matchFlag &= UtilityMethods.CompareValues<bool>("StdConfigUpSellDownSellEnabled", actualCatalogHeader.StdConfigUpSellDownSellEnabled, (catalogItemType.Contains(CatalogItemType.ConfigWithUpsellDownsell) ? true : false));
            matchFlag &= UtilityMethods.CompareValues<string>("Region", actualCatalogHeader.Region, expectedCatalogHeader.Region);
            matchFlag &= UtilityMethods.CompareValues<bool>("GPEnabled", actualCatalogHeader.GPEnabled, expectedCatalogHeader.GPEnabled);
            matchFlag &= UtilityMethods.CompareValues<object>("GPShipToCurrency", actualCatalogHeader.GPShipToCurrency, expectedCatalogHeader.GPShipToCurrency);
            matchFlag &= UtilityMethods.CompareValues<string>("GPShipToCountry", actualCatalogHeader.GPShipToCountry, expectedCatalogHeader.GPShipToCountry);
            matchFlag &= UtilityMethods.CompareValues<string>("GPShipToLanguage", actualCatalogHeader.GPShipToLanguage, expectedCatalogHeader.GPShipToLanguage);
            matchFlag &= UtilityMethods.CompareValues<string>("GPPurchaseOption", actualCatalogHeader.GPPurchaseOption, expectedCatalogHeader.GPPurchaseOption);
            matchFlag &= UtilityMethods.CompareValues<bool>("CPFEnabled", actualCatalogHeader.CPFEnabled, expectedCatalogHeader.CPFEnabled);
            matchFlag &= UtilityMethods.CompareValues<string>("IdentityUserName", actualCatalogHeader.IdentityUserName, identityName.ToUpper());
            matchFlag &= UtilityMethods.CompareValues<int>("GracePeriod", actualCatalogHeader.GracePeriod, expectedCatalogHeader.GracePeriod);
            matchFlag &= UtilityMethods.CompareValues<long>("ProfileId", actualCatalogHeader.ProfileId, expectedCatalogHeader.ProfileId);
            matchFlag &= UtilityMethods.CompareValues<string>("CustomerID", actualCatalogHeader.CustomerID, expectedCatalogHeader.CustomerID);
            matchFlag &= UtilityMethods.CompareValues<string>("AccessGroup", actualCatalogHeader.AccessGroup, expectedCatalogHeader.AccessGroup);
            matchFlag &= UtilityMethods.CompareValues<string>("MessageType", actualCatalogHeader.MessageType, expectedCatalogHeader.MessageType);
            matchFlag &= UtilityMethods.CompareValues<string>("CatalogType", actualCatalogHeader.CatalogType, expectedCatalogHeader.CatalogType);
            matchFlag &= UtilityMethods.CompareValues<string>("Sender", actualCatalogHeader.Sender, expectedCatalogHeader.Sender);
            matchFlag &= UtilityMethods.CompareValues<string>("Receiver", actualCatalogHeader.Receiver, identityName.ToUpper());
            matchFlag &= UtilityMethods.CompareValues<string>("CatalogDate", actualCatalogHeader.CatalogDate.ToString(), DateTime.Now.ConvertToUtcTimeZone().ToString("MM/dd/yyyy 0:00:00", System.Globalization.CultureInfo.InvariantCulture));
            return matchFlag;
        }

        /// <summary>
        /// Validate all the tags of each Catalog Item
        /// </summary>
        /// <param name="actualCatalog"></param>
        /// <param name="expectedCatalog"></param>
        /// <returns></returns>
        public bool ValidateCatalogItems(IEnumerable<CatalogItem> actualCatalogItems, IEnumerable<CatalogItem> expectedCatalogItems, ConfigRules configRules)
        {
            bool matchFlag = true;

            Console.WriteLine("Catalog Items Data Validation");

            actualCatalogItems.Count().Should().Be(expectedCatalogItems.Count(), "ERROR: Actual and Expected Catalog item count did not match");

            foreach (CatalogItem actualCatalogItem in actualCatalogItems)
            {
                CatalogItem expectedCatalogItem = null;

                if (actualCatalogItem.CatalogItemType == CatalogItemType.SNP)
                {
                    expectedCatalogItem = expectedCatalogItems.Where(ci => ci.BaseSKUId == actualCatalogItem.BaseSKUId).FirstOrDefault();
                    matchFlag &= (actualCatalogItem.PartId.Contains("BHC:"));
                    Console.WriteLine("Catalog Item Base SKU Id under comparison: " + actualCatalogItem.BaseSKUId);
                }
                else
                {
                    expectedCatalogItem = expectedCatalogItems.Where(ci => ci.ItemOrderCode == actualCatalogItem.ItemOrderCode).FirstOrDefault();
                    matchFlag &= !(actualCatalogItem.PartId != "BHC:" + actualCatalogItem.QuoteId);
                    Console.WriteLine("Catalog Item Order Code under comparison: " + actualCatalogItem.ItemOrderCode);
                }

                //var methodInfo = typeof(UtilityMethods).GetMethod("CompareValues");
                //var catalogItemType = typeof(CatalogItem);

                //foreach (var pi in actualCatalogItem.GetType().GetProperties())
                //{
                //    var propertyType = pi.PropertyType;
                //    var propertyName = pi.Name;
                //    var actualValue = catalogItemType.GetProperty(propertyName).GetValue(actualCatalogItem);
                //    var expectedValue = catalogItemType.GetProperty(propertyName).GetValue(expectedCatalogItem);

                //    var methodToinvoke = methodInfo.MakeGenericMethod(propertyType);
                //    matchFlag &=  (bool)methodToinvoke.Invoke(null, new[] { propertyName, actualValue, expectedValue });

                //    //matchFlag &= UtilityMethods.CompareValues<CatalogItemType>(propertyName, actualValue, expectedValue);
                //}

                matchFlag &= UtilityMethods.CompareValues<CatalogItemType>("CatalogItemType", actualCatalogItem.CatalogItemType, expectedCatalogItem.CatalogItemType);
                matchFlag &= UtilityMethods.CompareValues<string>("PrimaryCurrency", actualCatalogItem.PrimaryCurrency.CurrencyCode, actualCatalogItem.PrimaryCurrency.CurrencyCode);
                matchFlag &= UtilityMethods.CompareValues<string>("AlternateCurrency", actualCatalogItem.AlternateCurrency.CurrencyCode, actualCatalogItem.AlternateCurrency.CurrencyCode);
                matchFlag &= UtilityMethods.CompareValues<string>("ShortName", actualCatalogItem.ShortName, expectedCatalogItem.ShortName);
                matchFlag &= UtilityMethods.CompareValues<string>("ItemDescription", actualCatalogItem.ItemDescription, expectedCatalogItem.ItemDescription);
                matchFlag &= UtilityMethods.CompareValues<string>("UNSPSC", actualCatalogItem.UNSPSC, expectedCatalogItem.UNSPSC);
                matchFlag &= UtilityMethods.CompareValues<string>("UOM", actualCatalogItem.UOM, expectedCatalogItem.UOM);
                matchFlag &= UtilityMethods.CompareValues<decimal>("UnitPrice", actualCatalogItem.UnitPrice, expectedCatalogItem.UnitPrice);
                matchFlag &= UtilityMethods.CompareValues<string>("SuplierPartAuxilaryId", actualCatalogItem.SuplierPartAuxilaryId, expectedCatalogItem.SuplierPartAuxilaryId);
                matchFlag &= UtilityMethods.CompareValues<int>("LeadTime", actualCatalogItem.LeadTime, 0, Computation.GreaterThanOrEqualTo);
                matchFlag &= UtilityMethods.CompareValues<string>("SupplierURL", actualCatalogItem.SupplierURL, expectedCatalogItem.SupplierURL);
                matchFlag &= UtilityMethods.CompareValues<string>("ImageURL", actualCatalogItem.ImageURL, expectedCatalogItem.ImageURL);
                matchFlag &= UtilityMethods.CompareValues<string>("ManufacturerPartNumber", actualCatalogItem.ManufacturerPartNumber, expectedCatalogItem.ManufacturerPartNumber);
                matchFlag &= UtilityMethods.CompareValues<string>("ManufacturerName", actualCatalogItem.ManufacturerName, expectedCatalogItem.ManufacturerName);
                matchFlag &= UtilityMethods.CompareValues<string>("LongDescription", actualCatalogItem.LongDescription, expectedCatalogItem.LongDescription);
                matchFlag &= UtilityMethods.CompareValues<string>("GrossWeight", actualCatalogItem.GrossWeight, expectedCatalogItem.GrossWeight);
                matchFlag &= UtilityMethods.CompareValues<string>("Availability", actualCatalogItem.Availability, expectedCatalogItem.Availability);
                matchFlag &= UtilityMethods.CompareValues<string>("CategoryLevel1", actualCatalogItem.CategoryLevel1, expectedCatalogItem.CategoryLevel1);
                matchFlag &= UtilityMethods.CompareValues<string>("CategoryLevel2", actualCatalogItem.CategoryLevel2, expectedCatalogItem.CategoryLevel2);
                matchFlag &= UtilityMethods.CompareValues<string>("DomsQuote", actualCatalogItem.DomsQuote, expectedCatalogItem.DomsQuote);
                matchFlag &= UtilityMethods.CompareValues<string>("ItemOrderCode", actualCatalogItem.ItemOrderCode, expectedCatalogItem.ItemOrderCode);
                matchFlag &= UtilityMethods.CompareValues<string>("BaseSKUId", actualCatalogItem.BaseSKUId, expectedCatalogItem.BaseSKUId);
                matchFlag &= UtilityMethods.CompareValues<string>("FGASKUId", actualCatalogItem.FGASKUId, expectedCatalogItem.FGASKUId);
                matchFlag &= UtilityMethods.CompareValues<string>("ReplacementQuoteId", actualCatalogItem.ReplacementQuoteId, expectedCatalogItem.ReplacementQuoteId);
                // matchFlag &= UtilityMethods.CompareValues<string>("ItemType", actualCatalogItem.ItemType, expectedCatalogItem.ItemType);
                // matchFlag &= UtilityMethods.CompareValues<string>("ItemSKUinfo", actualCatalogItem.ItemSKUinfo, expectedCatalogItem.ItemSKUinfo);
               // matchFlag &= UtilityMethods.CompareValues<string>("FGAModNumber", actualCatalogItem.FGAModNumber, expectedCatalogItem.FGAModNumber);
                matchFlag &= UtilityMethods.CompareValues<int>("InventoryQty", actualCatalogItem.InventoryQty, -9999, Computation.GreaterThanOrEqualTo);
                matchFlag &= UtilityMethods.CompareValues<string>("ListPrice", actualCatalogItem.ListPrice, expectedCatalogItem.ListPrice);
                matchFlag &= UtilityMethods.CompareValues<string>("UPC", actualCatalogItem.UPC, expectedCatalogItem.UPC);
                matchFlag &= UtilityMethods.CompareValues<string>("ManufacturerCode", actualCatalogItem.ManufacturerCode, expectedCatalogItem.ManufacturerCode);
                matchFlag &= UtilityMethods.CompareValues<string>("VPNReplacement", actualCatalogItem.VPNReplacement, expectedCatalogItem.VPNReplacement);
                matchFlag &= UtilityMethods.CompareValues<string>("VPNEOLDate", actualCatalogItem.VPNEOLDate, expectedCatalogItem.VPNEOLDate);
                matchFlag &= UtilityMethods.CompareValues<int>("PackageLength", actualCatalogItem.PackageLength, expectedCatalogItem.PackageLength);
                matchFlag &= UtilityMethods.CompareValues<int>("PackageWidth", actualCatalogItem.PackageWidth, expectedCatalogItem.PackageWidth);
                matchFlag &= UtilityMethods.CompareValues<int>("PackageHeight", actualCatalogItem.PackageHeight, expectedCatalogItem.PackageHeight);
                matchFlag &= UtilityMethods.CompareValues<string>("PalletLength", String.IsNullOrEmpty(actualCatalogItem.PalletLength)? "0":actualCatalogItem.PalletLength, expectedCatalogItem.PalletLength);
                matchFlag &= UtilityMethods.CompareValues<string>("PalletWidth", String.IsNullOrEmpty(actualCatalogItem.PalletWidth)?"0": actualCatalogItem.PalletWidth, expectedCatalogItem.PalletWidth);
                matchFlag &= UtilityMethods.CompareValues<string>("PalletHeight", String.IsNullOrEmpty(actualCatalogItem.PalletHeight)?"0": actualCatalogItem.PalletHeight, expectedCatalogItem.PalletHeight);
                matchFlag &= UtilityMethods.CompareValues<string>("PalletUnitsPerLayer", String.IsNullOrEmpty(actualCatalogItem.PalletUnitsPerLayer) ? "0" : actualCatalogItem.PalletUnitsPerLayer, expectedCatalogItem.PalletUnitsPerLayer);
                matchFlag &= UtilityMethods.CompareValues<string>("PalletLayerPerPallet", String.IsNullOrEmpty(actualCatalogItem.PalletLayerPerPallet) ? "0" : actualCatalogItem.PalletLayerPerPallet, expectedCatalogItem.PalletLayerPerPallet);
                matchFlag &= UtilityMethods.CompareValues<string>("PalletUnitsPerPallet", String.IsNullOrEmpty(actualCatalogItem.PalletUnitsPerPallet) ? "0" : actualCatalogItem.PalletUnitsPerPallet, expectedCatalogItem.PalletUnitsPerPallet);

                Console.WriteLine("PartId: " + actualCatalogItem.PartId);
                Console.WriteLine("QuoteId: " + actualCatalogItem.QuoteId);

                // Part and Quote Id
                matchFlag &= !(String.IsNullOrEmpty(actualCatalogItem.PartId));
                //Console.WriteLine("PartId is empty");
                matchFlag &= !(String.IsNullOrEmpty(actualCatalogItem.QuoteId));
                //Console.WriteLine("PartId is empty");

                // Modules
                if (configRules == ConfigRules.WithDefOptions)
                {
                    actualCatalogItem.Modules.Module.Count().Should().Be(expectedCatalogItem.Modules.Module.Count(), "ERROR: Module count mismatch for Order code: " + actualCatalogItem.ItemOrderCode);

                    foreach (CatalogItemModule actualmodule in actualCatalogItem.Modules.Module)
                    {
                        CatalogItemModule expectedModule = expectedCatalogItem.Modules.Module.Where(em => em.ModuleId == actualmodule.ModuleId).FirstOrDefault();

                        actualmodule.Options.Option.Count().Should().Be(expectedModule.Options.Option.Count(), "ERROR: Option count mismatch for Module ID: " + actualmodule.ModuleId.ToString());

                        foreach (CatalogItemOption actualOption in actualmodule.Options.Option)
                        {
                            CatalogItemOption expectedOption = expectedModule.Options.Option.Where(eo => eo.OptionDesc == actualOption.OptionDesc).FirstOrDefault();

                            actualOption.OptionSkuList.OptionSku.Count().Should().Be(expectedOption.OptionSkuList.OptionSku.Count(), "ERROR: Option SKU count mismtach for Option Desc: " + actualOption.OptionDesc);

                            foreach (OptionSku actualOptionSku in actualOption.OptionSkuList.OptionSku)
                            {
                                OptionSku expectedOptionSku = expectedOption.OptionSkuList.OptionSku.Where(eo => eo.SkuId == actualOptionSku.SkuId).FirstOrDefault();

                                matchFlag &= UtilityMethods.CompareValues<string>("Modules.Module.Options.Option.OptionSkuList.OptionSku.SkuId", actualOptionSku.SkuId, expectedOptionSku.SkuId);
                                matchFlag &= UtilityMethods.CompareValues<string>("Modules.Module.Options.Option.OptionSkuList.OptionSku.SkuDescription", actualOptionSku.SkuDescription, expectedOptionSku.SkuDescription);
                                matchFlag &= UtilityMethods.CompareValues<string>("Modules.Module.Options.Option.OptionSkuList.OptionSku.OptionId", actualOptionSku.OptionId, expectedOptionSku.OptionId);
                                matchFlag &= UtilityMethods.CompareValues<decimal>("Modules.Module.Options.Option.OptionSkuList.OptionSku.SkuPrice", actualOptionSku.SkuPrice, expectedOptionSku.SkuPrice);
                            }
                            //Kishore
                            // matchFlag &= UtilityMethods.CompareValues<string>("Modules.Module.Options.Option.OptionId", actualOption.OptionId, expectedOption.OptionId);
                            matchFlag &= UtilityMethods.CompareValues<string>("Modules.Module.Options.Option.OptionDesc", actualOption.OptionDesc, expectedOption.OptionDesc);
                            matchFlag &= UtilityMethods.CompareValues<bool>("Modules.Module.Options.Option.Selected", actualOption.Selected, expectedOption.Selected);
                            matchFlag &= UtilityMethods.CompareValues<decimal>("Modules.Module.Options.Option.DeltaPrice", actualOption.DeltaPrice, expectedOption.DeltaPrice);
                            matchFlag &= UtilityMethods.CompareValues<decimal>("Modules.Module.Options.Option.FinalPrice", actualOption.FinalPrice, expectedOption.FinalPrice);
                        }
                        matchFlag &= UtilityMethods.CompareValues<int>("Modules.Module.ModuleId", actualmodule.ModuleId, expectedModule.ModuleId);
                        matchFlag &= UtilityMethods.CompareValues<string>("Modules.Module.ModuleDesc", actualmodule.ModuleDesc, expectedModule.ModuleDesc);
                        matchFlag &= UtilityMethods.CompareValues<bool>("Modules.Module.Required", actualmodule.Required, expectedModule.Required);
                        matchFlag &= UtilityMethods.CompareValues<bool>("Modules.Module.MultiSelect", actualmodule.MultiSelect, expectedModule.MultiSelect);
                        matchFlag &= UtilityMethods.CompareValues<string>("Modules.Module.DefaultOptionId", actualmodule.DefaultOptionId, expectedModule.DefaultOptionId);
                        matchFlag &= UtilityMethods.CompareValues<decimal>("Modules.Module.DefaultOptionPrice", actualmodule.DefaultOptionPrice, expectedModule.DefaultOptionPrice);
                    }
                }
                // Lease
                matchFlag &= UtilityMethods.CompareValues<string>("Lease.LeaseTerms", actualCatalogItem.Lease.LeaseTerms, expectedCatalogItem.Lease.LeaseTerms);
                matchFlag &= UtilityMethods.CompareValues<string>("Lease.Frequency", actualCatalogItem.Lease.Frequency, expectedCatalogItem.Lease.Frequency);
                matchFlag &= UtilityMethods.CompareValues<decimal>("Lease.LeaseTerms", actualCatalogItem.Lease.LRF, expectedCatalogItem.Lease.LRF);
                matchFlag &= UtilityMethods.CompareValues<string>("Lease.LeaseTerms", actualCatalogItem.Lease.Disposition, expectedCatalogItem.Lease.Disposition);
                matchFlag &= UtilityMethods.CompareValues<decimal>("Lease.LeaseTerms", actualCatalogItem.Lease.ExpensedTotal, expectedCatalogItem.Lease.ExpensedTotal);

                // Delta Comments
                matchFlag &= UtilityMethods.CompareValues<string>("DeltaComments.Operation", actualCatalogItem.DeltaComments.Operation, expectedCatalogItem.DeltaComments.Operation);

                // Delta Change
                matchFlag &= UtilityMethods.CompareValues<DeltaStatus>("DeltaChange", actualCatalogItem.DeltaChange, expectedCatalogItem.DeltaChange);

                //For SPL Additional Nodes
                if (configRules == ConfigRules.SPL)
                {
                    matchFlag &= UtilityMethods.CompareValues<string>("BoxDimensions", actualCatalogItem.BoxDimensions == null ? "" : actualCatalogItem.BoxDimensions.ToString(), expectedCatalogItem.BoxDimensions == null ? "" : expectedCatalogItem.BoxDimensions.ToString());

                    matchFlag &= UtilityMethods.CompareValues<string>("MinimumOrderQty", actualCatalogItem.MinimumOrderQty == null ? "" : actualCatalogItem.MinimumOrderQty.ToString(), expectedCatalogItem.MinimumOrderQty == null ? "" : expectedCatalogItem.MinimumOrderQty.ToString());

                    matchFlag &= UtilityMethods.CompareValues<string>("PackSeparate", actualCatalogItem.PackSeparate == null ? "" : actualCatalogItem.PackSeparate.ToString(), expectedCatalogItem.PackSeparate == null ? "" : expectedCatalogItem.PackSeparate.ToString());

                    matchFlag &= UtilityMethods.CompareValues<string>("CasePackQuantity", actualCatalogItem.CasePackQuantity == null ? "" : actualCatalogItem.CasePackQuantity.ToString(), expectedCatalogItem.CasePackQuantity == null ? "" : expectedCatalogItem.CasePackQuantity.ToString());

                    matchFlag &= UtilityMethods.CompareValues<string>("MCQ", actualCatalogItem.MCQ == null ? "" : actualCatalogItem.MCQ.ToString(), expectedCatalogItem.MCQ == null ? "" : expectedCatalogItem.MCQ.ToString());

                    matchFlag &= UtilityMethods.CompareValues<string>("CustomsCode", actualCatalogItem.CustomsCode == null ? "" : actualCatalogItem.CustomsCode.ToString(), expectedCatalogItem.CustomsCode == null ? "" : expectedCatalogItem.CustomsCode.ToString());

                    matchFlag &= UtilityMethods.CompareValues<string>("ShippingInstructions", actualCatalogItem.ShippingInstructions == null ? "" : actualCatalogItem.ShippingInstructions.ToString(), expectedCatalogItem.ShippingInstructions == null ? "" : expectedCatalogItem.ShippingInstructions.ToString());

                    matchFlag &= UtilityMethods.CompareValues<string>("DropShip", actualCatalogItem.DropShip == null ? "" : actualCatalogItem.DropShip.ToString(), expectedCatalogItem.DropShip == null ? "" : expectedCatalogItem.DropShip.ToString());

                    matchFlag &= UtilityMethods.CompareValues<string>("CountryofOrigin", actualCatalogItem.CountryofOrigin == null ? "" : actualCatalogItem.CountryofOrigin.ToString(), expectedCatalogItem.CountryofOrigin == null ? "" : expectedCatalogItem.CountryofOrigin.ToString());

                    matchFlag &= UtilityMethods.CompareValues<string>("EPEAT", actualCatalogItem.EPEAT == null ? "" : actualCatalogItem.EPEAT.ToString(), expectedCatalogItem.EPEAT == null ? "" : expectedCatalogItem.EPEAT.ToString());

                    matchFlag &= UtilityMethods.CompareValues<string>("ECCN", actualCatalogItem.ECCN == null ? "" : actualCatalogItem.ECCN.ToString(), expectedCatalogItem.ECCN == null ? "" : expectedCatalogItem.ECCN.ToString());

                    matchFlag &= UtilityMethods.CompareValues<string>("UHG", actualCatalogItem.UHG == null ? "" : actualCatalogItem.UHG.ToString(), expectedCatalogItem.UHG == null ? "" : expectedCatalogItem.UHG.ToString());

                    matchFlag &= UtilityMethods.CompareValues<string>("Levy", actualCatalogItem.Levy == null ? "" : actualCatalogItem.Levy.ToString(), expectedCatalogItem.Levy == null ? "" : expectedCatalogItem.Levy.ToString());

                    matchFlag &= UtilityMethods.CompareValues<string>("SerialScan", actualCatalogItem.SerialScan == null ? "" : actualCatalogItem.SerialScan.ToString(), expectedCatalogItem.SerialScan == null ? "" : expectedCatalogItem.SerialScan.ToString());

                    matchFlag &= UtilityMethods.CompareValues<string>("Comments", actualCatalogItem.Comments == null ? "" : actualCatalogItem.Comments.ToString(), expectedCatalogItem.Comments == null ? "" : expectedCatalogItem.Comments.ToString());

                    matchFlag &= UtilityMethods.CompareValues<string>("Promo", actualCatalogItem.Promo == null ? "" : actualCatalogItem.Promo.ToString(), expectedCatalogItem.Promo == null ? "" : expectedCatalogItem.Promo.ToString());

                    matchFlag &= UtilityMethods.CompareValues<string>("CountryCode", actualCatalogItem.CountryCode == null ? "" : actualCatalogItem.CountryCode.ToString(), expectedCatalogItem.CountryCode == null ? "" : expectedCatalogItem.CountryCode.ToString());

                    matchFlag &= UtilityMethods.CompareValues<string>("Model", actualCatalogItem.Model == null ? "" : actualCatalogItem.Model.ToString(), expectedCatalogItem.Model == null ? "" : expectedCatalogItem.Model.ToString());

                    matchFlag &= UtilityMethods.CompareValues<string>("EAN", actualCatalogItem.EAN == null ? "" : actualCatalogItem.EAN.ToString(), expectedCatalogItem.EAN == null ? "" : expectedCatalogItem.EAN.ToString());

                    matchFlag &= UtilityMethods.CompareValues<string>("JAN", actualCatalogItem.JAN == null ? "" : actualCatalogItem.JAN.ToString(), expectedCatalogItem.JAN == null ? "" : expectedCatalogItem.JAN.ToString());

                    matchFlag &= UtilityMethods.CompareValues<string>("LineofBusiness", actualCatalogItem.LineofBusiness == null ? "" : actualCatalogItem.LineofBusiness.ToString(), expectedCatalogItem.LineofBusiness == null ? "" : expectedCatalogItem.LineofBusiness.ToString());

                    matchFlag &= UtilityMethods.CompareValues<string>("Type", actualCatalogItem.Type == null ? "" : actualCatalogItem.Type.ToString(), expectedCatalogItem.Type == null ? "" : expectedCatalogItem.Type.ToString());

                    matchFlag &= UtilityMethods.CompareValues<string>("ProductCategory", actualCatalogItem.ProductCategory == null ? "" : actualCatalogItem.ProductCategory.ToString(), expectedCatalogItem.ProductCategory == null ? "" : expectedCatalogItem.ProductCategory.ToString());

                    matchFlag &= UtilityMethods.CompareValues<string>("ProductSubCategory", actualCatalogItem.ProductSubCategory == null ? "" : actualCatalogItem.ProductSubCategory.ToString(), expectedCatalogItem.ProductSubCategory == null ? "" : expectedCatalogItem.ProductSubCategory.ToString());

                    matchFlag &= UtilityMethods.CompareValues<string>("ShortDescription", actualCatalogItem.ShortDescription == null ? "" : actualCatalogItem.ShortDescription.ToString(), expectedCatalogItem.ShortDescription == null ? "" : expectedCatalogItem.ShortDescription.ToString());

                  //  matchFlag &= UtilityMethods.CompareValues<string>("Images", actualCatalogItem.Images == null ? "" : actualCatalogItem.Images.ToString(), expectedCatalogItem.Images == null ? "" : expectedCatalogItem.Images.ToString());

                    matchFlag &= UtilityMethods.CompareValues<string>("Aio", actualCatalogItem.Aio == null ? "" : actualCatalogItem.Aio.ToString(), expectedCatalogItem.Aio == null ? "" : expectedCatalogItem.Aio.ToString());

                    matchFlag &= UtilityMethods.CompareValues<string>("EStar", actualCatalogItem.EStar == null ? "" : actualCatalogItem.EStar.ToString(), expectedCatalogItem.EStar == null ? "" : expectedCatalogItem.EStar.ToString());

                    matchFlag &= UtilityMethods.CompareValues<string>("MfgPartNumber", actualCatalogItem.MfgPartNumber == null ? "" : actualCatalogItem.MfgPartNumber.ToString(), expectedCatalogItem.MfgPartNumber == null ? "" : expectedCatalogItem.MfgPartNumber.ToString());

                    matchFlag &= UtilityMethods.CompareValues<string>("ResellerPrice", actualCatalogItem.ResellerPrice == null ? "" : actualCatalogItem.ResellerPrice.ToString(), expectedCatalogItem.ResellerPrice == null ? "" : expectedCatalogItem.ResellerPrice.ToString());

                    matchFlag &= UtilityMethods.CompareValues<string>("ResellerDeltaPrice", actualCatalogItem.ResellerDeltaPrice == null ? "" : actualCatalogItem.ResellerDeltaPrice.ToString(), expectedCatalogItem.ResellerDeltaPrice == null ? "" : expectedCatalogItem.ResellerDeltaPrice.ToString());

                   // matchFlag &= UtilityMethods.CompareValues<string>("TechSpec", actualCatalogItem.TechSpec == null ? "" : actualCatalogItem.TechSpec.ToString(), expectedCatalogItem.TechSpec == null ? "" : expectedCatalogItem.TechSpec.ToString());

                    matchFlag &= UtilityMethods.CompareValues<string>("AssociatedSNPSkus", actualCatalogItem.AssociatedSNPSkus == null ? "" : actualCatalogItem.AssociatedSNPSkus.ToString(), expectedCatalogItem.AssociatedSNPSkus == null ? "" : expectedCatalogItem.AssociatedSNPSkus.ToString());

                    matchFlag &= UtilityMethods.CompareValues<string>("AssociatedServiceSkus", actualCatalogItem.AssociatedServiceSkus == null ? "" : actualCatalogItem.AssociatedServiceSkus.ToString(), expectedCatalogItem.AssociatedServiceSkus == null ? "" : expectedCatalogItem.AssociatedServiceSkus.ToString());
        
                //SPL End
                }
            }

            return matchFlag;
        }

        public bool ValidateCatalogItem(CatalogItem actualCatalogItem, CatalogItem expectedCatalogItem)
        {
            bool matchFlag = true;

            if (actualCatalogItem.CatalogItemType == CatalogItemType.SNP)
                matchFlag &= (actualCatalogItem.PartId.Contains("BHC:"));
            else
                matchFlag &= !(actualCatalogItem.PartId != "BHC:" + actualCatalogItem.QuoteId);

            matchFlag &= UtilityMethods.CompareValues<CatalogItemType>("CatalogItemType", actualCatalogItem.CatalogItemType, expectedCatalogItem.CatalogItemType);
            matchFlag &= UtilityMethods.CompareValues<string>("PrimaryCurrency", actualCatalogItem.PrimaryCurrency.CurrencyCode, actualCatalogItem.PrimaryCurrency.CurrencyCode);
            matchFlag &= UtilityMethods.CompareValues<string>("AlternateCurrency", actualCatalogItem.AlternateCurrency.CurrencyCode, actualCatalogItem.AlternateCurrency.CurrencyCode);
            matchFlag &= UtilityMethods.CompareValues<string>("ShortName", actualCatalogItem.ShortName, expectedCatalogItem.ShortName);
            matchFlag &= UtilityMethods.CompareValues<string>("ItemDescription", actualCatalogItem.ItemDescription, expectedCatalogItem.ItemDescription);
            matchFlag &= UtilityMethods.CompareValues<string>("UNSPSC", actualCatalogItem.UNSPSC, expectedCatalogItem.UNSPSC);
            matchFlag &= UtilityMethods.CompareValues<string>("UOM", actualCatalogItem.UOM, expectedCatalogItem.UOM);
            matchFlag &= UtilityMethods.CompareValues<decimal>("UnitPrice", actualCatalogItem.UnitPrice, expectedCatalogItem.UnitPrice);
            matchFlag &= UtilityMethods.CompareValues<string>("SuplierPartAuxilaryId", actualCatalogItem.SuplierPartAuxilaryId, expectedCatalogItem.SuplierPartAuxilaryId);
            matchFlag &= UtilityMethods.CompareValues<int>("LeadTime", actualCatalogItem.LeadTime, expectedCatalogItem.LeadTime, Computation.GreaterThanOrEqualTo);
            matchFlag &= UtilityMethods.CompareValues<string>("SupplierURL", actualCatalogItem.SupplierURL, expectedCatalogItem.SupplierURL);
            matchFlag &= UtilityMethods.CompareValues<string>("ImageURL", actualCatalogItem.ImageURL, expectedCatalogItem.ImageURL);
            matchFlag &= UtilityMethods.CompareValues<string>("ManufacturerPartNumber", actualCatalogItem.ManufacturerPartNumber, expectedCatalogItem.ManufacturerPartNumber);
            matchFlag &= UtilityMethods.CompareValues<string>("ManufacturerName", actualCatalogItem.ManufacturerName, expectedCatalogItem.ManufacturerName);
            matchFlag &= UtilityMethods.CompareValues<string>("LongDescription", actualCatalogItem.LongDescription, expectedCatalogItem.LongDescription);
            matchFlag &= UtilityMethods.CompareValues<string>("GrossWeight", actualCatalogItem.GrossWeight, expectedCatalogItem.GrossWeight);
            matchFlag &= UtilityMethods.CompareValues<string>("Availability", actualCatalogItem.Availability, expectedCatalogItem.Availability);
            matchFlag &= UtilityMethods.CompareValues<string>("CategoryLevel1", actualCatalogItem.CategoryLevel1, expectedCatalogItem.CategoryLevel1);
            matchFlag &= UtilityMethods.CompareValues<string>("CategoryLevel2", actualCatalogItem.CategoryLevel2, expectedCatalogItem.CategoryLevel2);
            matchFlag &= UtilityMethods.CompareValues<string>("DomsQuote", actualCatalogItem.DomsQuote, expectedCatalogItem.DomsQuote);
            matchFlag &= UtilityMethods.CompareValues<string>("ItemOrderCode", actualCatalogItem.ItemOrderCode, expectedCatalogItem.ItemOrderCode);
            matchFlag &= UtilityMethods.CompareValues<string>("BaseSKUId", actualCatalogItem.BaseSKUId, expectedCatalogItem.BaseSKUId);
            matchFlag &= UtilityMethods.CompareValues<string>("FGASKUId", actualCatalogItem.FGASKUId, expectedCatalogItem.FGASKUId);
            matchFlag &= UtilityMethods.CompareValues<string>("ReplacementQuoteId", actualCatalogItem.ReplacementQuoteId, expectedCatalogItem.ReplacementQuoteId);
            // matchFlag &= UtilityMethods.CompareValues<string>("ItemType", actualCatalogItem.ItemType, expectedCatalogItem.ItemType);
            // matchFlag &= UtilityMethods.CompareValues<string>("ItemSKUinfo", actualCatalogItem.ItemSKUinfo, expectedCatalogItem.ItemSKUinfo);
            matchFlag &= UtilityMethods.CompareValues<string>("FGAModNumber", actualCatalogItem.FGAModNumber, expectedCatalogItem.FGAModNumber);
            matchFlag &= UtilityMethods.CompareValues<int>("InventoryQty", actualCatalogItem.InventoryQty, -9999, Computation.GreaterThanOrEqualTo);
            matchFlag &= UtilityMethods.CompareValues<string>("ListPrice", actualCatalogItem.ListPrice, expectedCatalogItem.ListPrice);
            matchFlag &= UtilityMethods.CompareValues<string>("UPC", actualCatalogItem.UPC, expectedCatalogItem.UPC);
            matchFlag &= UtilityMethods.CompareValues<string>("ManufacturerCode", actualCatalogItem.ManufacturerCode, expectedCatalogItem.ManufacturerCode);
            matchFlag &= UtilityMethods.CompareValues<string>("VPNReplacement", actualCatalogItem.VPNReplacement, expectedCatalogItem.VPNReplacement);
            matchFlag &= UtilityMethods.CompareValues<string>("VPNEOLDate", actualCatalogItem.VPNEOLDate, expectedCatalogItem.VPNEOLDate);
            matchFlag &= UtilityMethods.CompareValues<int>("PackageLength", actualCatalogItem.PackageLength, expectedCatalogItem.PackageLength);
            matchFlag &= UtilityMethods.CompareValues<int>("PackageWidth", actualCatalogItem.PackageWidth, expectedCatalogItem.PackageWidth);
            matchFlag &= UtilityMethods.CompareValues<int>("PackageHeight", actualCatalogItem.PackageHeight, expectedCatalogItem.PackageHeight);
            matchFlag &= UtilityMethods.CompareValues<string>("PalletLength", actualCatalogItem.PalletLength, expectedCatalogItem.PalletLength);
            matchFlag &= UtilityMethods.CompareValues<string>("PalletWidth", actualCatalogItem.PalletWidth, expectedCatalogItem.PalletWidth);
            matchFlag &= UtilityMethods.CompareValues<string>("PalletHeight", actualCatalogItem.PalletHeight, expectedCatalogItem.PalletHeight);
            matchFlag &= UtilityMethods.CompareValues<string>("PalletUnitsPerLayer", actualCatalogItem.PalletUnitsPerLayer, expectedCatalogItem.PalletUnitsPerLayer);
            matchFlag &= UtilityMethods.CompareValues<string>("PalletLayerPerPallet", actualCatalogItem.PalletLayerPerPallet, expectedCatalogItem.PalletLayerPerPallet);
            matchFlag &= UtilityMethods.CompareValues<string>("PalletUnitsPerPallet", actualCatalogItem.PalletUnitsPerPallet, expectedCatalogItem.PalletUnitsPerPallet);

            Console.WriteLine("PartId: " + actualCatalogItem.PartId);
            Console.WriteLine("QuoteId: " + actualCatalogItem.QuoteId);

            // Part and Quote Id
            matchFlag &= !(String.IsNullOrEmpty(actualCatalogItem.PartId));
            //Console.WriteLine("PartId is empty");
            matchFlag &= !(String.IsNullOrEmpty(actualCatalogItem.QuoteId));
            //Console.WriteLine("PartId is empty");

            // Lease
            matchFlag &= UtilityMethods.CompareValues<string>("Lease.LeaseTerms", actualCatalogItem.Lease.LeaseTerms, expectedCatalogItem.Lease.LeaseTerms);
            matchFlag &= UtilityMethods.CompareValues<string>("Lease.Frequency", actualCatalogItem.Lease.Frequency, expectedCatalogItem.Lease.Frequency);
            matchFlag &= UtilityMethods.CompareValues<decimal>("Lease.LeaseTerms", actualCatalogItem.Lease.LRF, expectedCatalogItem.Lease.LRF);
            matchFlag &= UtilityMethods.CompareValues<string>("Lease.LeaseTerms", actualCatalogItem.Lease.Disposition, expectedCatalogItem.Lease.Disposition);
            matchFlag &= UtilityMethods.CompareValues<decimal>("Lease.LeaseTerms", actualCatalogItem.Lease.ExpensedTotal, expectedCatalogItem.Lease.ExpensedTotal);

            // Delta Comments
            matchFlag &= UtilityMethods.CompareValues<string>("DeltaComments.Operation", actualCatalogItem.DeltaComments.Operation, expectedCatalogItem.DeltaComments.Operation);
            matchFlag &= UtilityMethods.CompareValues<int>("DeltaComments.ChangedModules.DeltaModules Count", actualCatalogItem.DeltaComments.ChangedModules.DeltaModules.Count, expectedCatalogItem.DeltaComments.ChangedModules.DeltaModules.Count);

            foreach (DeltaModules expectedDeltaModule in expectedCatalogItem.DeltaComments.ChangedModules.DeltaModules)
            {
                DeltaModules actualDeltaModule = actualCatalogItem.DeltaComments.ChangedModules.DeltaModules.Where(dm => dm.Id == expectedDeltaModule.Id).FirstOrDefault();
                if (actualDeltaModule == null)
                    throw new Exception(string.Format("Module with Id {0} is not found in actual catalog Delta Comments section", expectedDeltaModule.Id));

                matchFlag &= UtilityMethods.CompareValues<string>("DeltaComments.ChangedModules.Module.Operation", expectedDeltaModule.Operation, actualDeltaModule.Operation);

                matchFlag &= UtilityMethods.CompareValues<int>("DeltaComments.ChangedModules.DeltaModules.Options.OptionList Count", actualDeltaModule.Options.OptionList.Count, expectedDeltaModule.Options.OptionList.Count);
                foreach (Option expectedOption in expectedDeltaModule.Options.OptionList)
                {
                    Option actualOption = actualDeltaModule.Options.OptionList.Where(opt => opt.Id.Replace(actualCatalogItem.QuoteId, "") == expectedOption.Id.Replace(expectedCatalogItem.QuoteId, "")).FirstOrDefault();
                    if (actualOption == null)
                        throw new Exception(string.Format("Option with Id {0} is not found in actual catalog Option section", expectedOption.Id));

                    matchFlag &= UtilityMethods.CompareValues<string>("DeltaComments.ChangedModules.Module.Options.Option.Operation", expectedOption.Operation, actualOption.Operation);

                    matchFlag &= UtilityMethods.CompareValues<int>("DeltaComments.ChangedModules.DeltaModules.Options.OptionList.OptionSkuList.OptionSkus Count", actualOption.OptionSkuList.OptionSkus.Count, expectedOption.OptionSkuList.OptionSkus.Count);
                    foreach (DeltaOptionSku expectedOptionSku in expectedOption.OptionSkuList.OptionSkus)
                    {
                        DeltaOptionSku actualOptionSku = actualOption.OptionSkuList.OptionSkus.Where(optSku => optSku.SkuId == expectedOptionSku.SkuId).FirstOrDefault();
                        if (actualOptionSku == null)
                            throw new Exception(string.Format("Sku with Id {0} is not found in actual catalog Option Sku List", expectedOptionSku.SkuId));

                        matchFlag &= UtilityMethods.CompareValues<string>("DeltaComments.ChangedModules.Module.Options.Option.OptionSkuList.OptionSku.SkuInfo", expectedOptionSku.Operation, actualOptionSku.Operation);
                    }
                }
            }

            // Delta Change
            matchFlag &= UtilityMethods.CompareValues<DeltaStatus>("DeltaChange", actualCatalogItem.DeltaChange, expectedCatalogItem.DeltaChange);

            return matchFlag;
        }

        ///// <summary>
        ///// Validate all the tags of each Catalog Item
        ///// </summary>
        ///// <param name="actualCatalog"></param>
        ///// <param name="expectedCatalog"></param>
        ///// <returns></returns>
        //public bool ValidateCatalogItems(IEnumerable<CatalogItem> actualCatalogItems, IEnumerable<CatalogItem> expectedCatalogItems, DefaultOptions defaultOptions)
        //{
        //    bool matchFlag = true;

        //    Console.WriteLine("Catalog Items Data Validation");

        //    actualCatalogItems.Count().Should().Be(expectedCatalogItems.Count(), "ERROR: Actual and Expected Catalog item count did not match");

        //    foreach (CatalogItem actualCatalogItem in actualCatalogItems)
        //    {
        //        CatalogItem expectedCatalogItem = null;

        //        if (actualCatalogItem.CatalogItemType == CatalogItemType.SNP)
        //        {
        //            expectedCatalogItem = expectedCatalogItems.Where(ci => ci.BaseSKUId == actualCatalogItem.BaseSKUId).FirstOrDefault();
        //            matchFlag &= (actualCatalogItem.PartId.Contains("BHC:"));
        //        }
        //        else
        //        {
        //            expectedCatalogItem = expectedCatalogItems.Where(ci => ci.ItemOrderCode == actualCatalogItem.ItemOrderCode).FirstOrDefault();
        //            matchFlag &= !(actualCatalogItem.PartId != "BHC:" + actualCatalogItem.QuoteId);
        //        }
        //        matchFlag &= UtilityMethods.CompareValues<CatalogItemType>("CatalogItemType", actualCatalogItem.CatalogItemType, expectedCatalogItem.CatalogItemType);
        //        matchFlag &= UtilityMethods.CompareValues<string>("PrimaryCurrency", actualCatalogItem.PrimaryCurrency.CurrencyCode, actualCatalogItem.PrimaryCurrency.CurrencyCode);
        //        matchFlag &= UtilityMethods.CompareValues<string>("AlternateCurrency", actualCatalogItem.AlternateCurrency.CurrencyCode, actualCatalogItem.AlternateCurrency.CurrencyCode);
        //        matchFlag &= UtilityMethods.CompareValues<string>("ShortName", actualCatalogItem.ShortName, expectedCatalogItem.ShortName);
        //        matchFlag &= UtilityMethods.CompareValues<string>("ItemDescription", actualCatalogItem.ItemDescription, expectedCatalogItem.ItemDescription);
        //        matchFlag &= UtilityMethods.CompareValues<string>("UNSPSC", actualCatalogItem.UNSPSC, expectedCatalogItem.UNSPSC);
        //        matchFlag &= UtilityMethods.CompareValues<string>("UOM", actualCatalogItem.UOM, expectedCatalogItem.UOM);
        //        matchFlag &= UtilityMethods.CompareValues<decimal>("UnitPrice", actualCatalogItem.UnitPrice, expectedCatalogItem.UnitPrice);
        //        matchFlag &= UtilityMethods.CompareValues<string>("SuplierPartAuxilaryId", actualCatalogItem.SuplierPartAuxilaryId, expectedCatalogItem.SuplierPartAuxilaryId);
        //        matchFlag &= UtilityMethods.CompareValues<int>("LeadTime", actualCatalogItem.LeadTime, expectedCatalogItem.LeadTime, Computation.GreaterThanOrEqualTo);
        //        matchFlag &= UtilityMethods.CompareValues<string>("SupplierURL", actualCatalogItem.SupplierURL, expectedCatalogItem.SupplierURL);
        //        matchFlag &= UtilityMethods.CompareValues<string>("ImageURL", actualCatalogItem.ImageURL, expectedCatalogItem.ImageURL);
        //        matchFlag &= UtilityMethods.CompareValues<string>("ManufacturerPartNumber", actualCatalogItem.ManufacturerPartNumber, expectedCatalogItem.ManufacturerPartNumber);
        //        matchFlag &= UtilityMethods.CompareValues<string>("ManufacturerName", actualCatalogItem.ManufacturerName, expectedCatalogItem.ManufacturerName);
        //        matchFlag &= UtilityMethods.CompareValues<string>("LongDescription", actualCatalogItem.LongDescription, expectedCatalogItem.LongDescription);
        //        matchFlag &= UtilityMethods.CompareValues<string>("GrossWeight", actualCatalogItem.GrossWeight, expectedCatalogItem.GrossWeight);
        //        matchFlag &= UtilityMethods.CompareValues<string>("Availability", actualCatalogItem.Availability, expectedCatalogItem.Availability);
        //        matchFlag &= UtilityMethods.CompareValues<string>("CategoryLevel1", actualCatalogItem.CategoryLevel1, expectedCatalogItem.CategoryLevel1);
        //        matchFlag &= UtilityMethods.CompareValues<string>("CategoryLevel2", actualCatalogItem.CategoryLevel2, expectedCatalogItem.CategoryLevel2);
        //        matchFlag &= UtilityMethods.CompareValues<string>("DomsQuote", actualCatalogItem.DomsQuote, expectedCatalogItem.DomsQuote);
        //        matchFlag &= UtilityMethods.CompareValues<string>("ItemOrderCode", actualCatalogItem.ItemOrderCode, expectedCatalogItem.ItemOrderCode);
        //        matchFlag &= UtilityMethods.CompareValues<string>("BaseSKUId", actualCatalogItem.BaseSKUId, expectedCatalogItem.BaseSKUId);
        //        matchFlag &= UtilityMethods.CompareValues<string>("FGASKUId", actualCatalogItem.FGASKUId, expectedCatalogItem.FGASKUId);
        //        matchFlag &= UtilityMethods.CompareValues<string>("ReplacementQuoteId", actualCatalogItem.ReplacementQuoteId, expectedCatalogItem.ReplacementQuoteId);
        //        //matchFlag &= UtilityMethods.CompareValues<string>("ItemType", actualCatalogItem.ItemType, expectedCatalogItem.ItemType);
        //        //matchFlag &= UtilityMethods.CompareValues<string>("ItemSKUinfo", actualCatalogItem.ItemSKUinfo, expectedCatalogItem.ItemSKUinfo);
        //        matchFlag &= UtilityMethods.CompareValues<string>("FGAModNumber", actualCatalogItem.FGAModNumber, expectedCatalogItem.FGAModNumber);
        //        matchFlag &= UtilityMethods.CompareValues<int>("InventoryQty", actualCatalogItem.InventoryQty, expectedCatalogItem.InventoryQty, Computation.GreaterThanOrEqualTo);
        //        matchFlag &= UtilityMethods.CompareValues<string>("ListPrice", actualCatalogItem.ListPrice, expectedCatalogItem.ListPrice);
        //        matchFlag &= UtilityMethods.CompareValues<string>("UPC", actualCatalogItem.UPC, expectedCatalogItem.UPC);
        //        matchFlag &= UtilityMethods.CompareValues<string>("ManufacturerCode", actualCatalogItem.ManufacturerCode, expectedCatalogItem.ManufacturerCode);
        //        matchFlag &= UtilityMethods.CompareValues<string>("VPNReplacement", actualCatalogItem.VPNReplacement, expectedCatalogItem.VPNReplacement);
        //        matchFlag &= UtilityMethods.CompareValues<string>("VPNEOLDate", actualCatalogItem.VPNEOLDate, expectedCatalogItem.VPNEOLDate);
        //        matchFlag &= UtilityMethods.CompareValues<int>("PackageLength", actualCatalogItem.PackageLength, expectedCatalogItem.PackageLength);
        //        matchFlag &= UtilityMethods.CompareValues<int>("PackageWidth", actualCatalogItem.PackageWidth, expectedCatalogItem.PackageWidth);
        //        matchFlag &= UtilityMethods.CompareValues<int>("PackageHeight", actualCatalogItem.PackageHeight, expectedCatalogItem.PackageHeight);
        //        matchFlag &= UtilityMethods.CompareValues<int>("PalletLength", actualCatalogItem.PalletLength, expectedCatalogItem.PalletLength);
        //        matchFlag &= UtilityMethods.CompareValues<int>("PalletWidth", actualCatalogItem.PalletWidth, expectedCatalogItem.PalletWidth);
        //        matchFlag &= UtilityMethods.CompareValues<int>("PalletHeight", actualCatalogItem.PalletHeight, expectedCatalogItem.PalletHeight);
        //        matchFlag &= UtilityMethods.CompareValues<int>("PalletUnitsPerLayer", actualCatalogItem.PalletUnitsPerLayer, expectedCatalogItem.PalletUnitsPerLayer);
        //        matchFlag &= UtilityMethods.CompareValues<int>("PalletLayerPerPallet", actualCatalogItem.PalletLayerPerPallet, expectedCatalogItem.PalletLayerPerPallet);
        //        matchFlag &= UtilityMethods.CompareValues<int>("PalletUnitsPerPallet", actualCatalogItem.PalletUnitsPerPallet, expectedCatalogItem.PalletUnitsPerPallet);

        //        Console.WriteLine("PartId: " + actualCatalogItem.PartId);
        //        Console.WriteLine("QuoteId: " + actualCatalogItem.QuoteId);

        //        // Part and Quote Id
        //        matchFlag &= !(String.IsNullOrEmpty(actualCatalogItem.PartId));
        //        //Console.WriteLine("PartId is empty");
        //        matchFlag &= !(String.IsNullOrEmpty(actualCatalogItem.QuoteId));
        //        //Console.WriteLine("PartId is empty");

        //        // Modules
        //        if (defaultOptions == DefaultOptions.On)
        //        {
        //            actualCatalogItem.Modules.Module.Count()
        //                .Should()
        //                .Be(expectedCatalogItem.Modules.Module.Count(),
        //                    "ERROR: Module count mismatch for Order code: " + actualCatalogItem.ItemOrderCode);

        //            foreach (CatalogItemModule actualmodule in actualCatalogItem.Modules.Module)
        //            {
        //                CatalogItemModule expectedModule =
        //                    expectedCatalogItem.Modules.Module.Where(em => em.ModuleId == actualmodule.ModuleId)
        //                        .FirstOrDefault();

        //                actualmodule.Options.Option.Count()
        //                    .Should()
        //                    .Be(expectedModule.Options.Option.Count(),
        //                        "ERROR: Option count mismatch for Module ID: " + actualmodule.ModuleId.ToString());

        //                foreach (CatalogItemOption actualOption in actualmodule.Options.Option)
        //                {
        //                    CatalogItemOption expectedOption =
        //                        expectedModule.Options.Option.Where(eo => eo.OptionDesc == actualOption.OptionDesc)
        //                            .FirstOrDefault();

        //                    actualOption.OptionSkuList.OptionSku.Count()
        //                        .Should()
        //                        .Be(expectedOption.OptionSkuList.OptionSku.Count(),
        //                            "ERROR: Option SKU count mismtach for Option Desc: " + actualOption.OptionDesc);

        //                    foreach (OptionSku actualOptionSku in actualOption.OptionSkuList.OptionSku)
        //                    {
        //                        OptionSku expectedOptionSku =
        //                            expectedOption.OptionSkuList.OptionSku.Where(eo => eo.SkuId == actualOptionSku.SkuId)
        //                                .FirstOrDefault();

        //                        matchFlag &=
        //                            UtilityMethods.CompareValues<string>(
        //                                "Modules.Module.Options.Option.OptionSkuList.OptionSku.SkuId",
        //                                actualOptionSku.SkuId, expectedOptionSku.SkuId);
        //                        matchFlag &=
        //                            UtilityMethods.CompareValues<string>(
        //                                "Modules.Module.Options.Option.OptionSkuList.OptionSku.SkuDescription",
        //                                actualOptionSku.SkuDescription, expectedOptionSku.SkuDescription);
        //                        matchFlag &=
        //                            UtilityMethods.CompareValues<string>(
        //                                "Modules.Module.Options.Option.OptionSkuList.OptionSku.OptionId",
        //                                actualOptionSku.OptionId, expectedOptionSku.OptionId);
        //                        matchFlag &=
        //                            UtilityMethods.CompareValues<decimal>(
        //                                "Modules.Module.Options.Option.OptionSkuList.OptionSku.SkuPrice",
        //                                actualOptionSku.SkuPrice, expectedOptionSku.SkuPrice);
        //                    }


        //                    matchFlag &= UtilityMethods.CompareValues<string>("Modules.Module.Options.Option.OptionId",
        //                    actualOption.OptionId.Substring(actualOption.OptionId.IndexOf("/", System.StringComparison.Ordinal)),
        //                    expectedOption.OptionId.Substring(expectedOption.OptionId.IndexOf("/", System.StringComparison.Ordinal)));

        //                    matchFlag &= UtilityMethods.CompareValues<string>(
        //                        "Modules.Module.Options.Option.OptionDesc", actualOption.OptionDesc,
        //                        expectedOption.OptionDesc);
        //                    matchFlag &= UtilityMethods.CompareValues<bool>("Modules.Module.Options.Option.Selected",
        //                        actualOption.Selected, expectedOption.Selected);
        //                    matchFlag &=
        //                        UtilityMethods.CompareValues<decimal>("Modules.Module.Options.Option.DeltaPrice",
        //                            actualOption.DeltaPrice, expectedOption.DeltaPrice);
        //                    matchFlag &=
        //                        UtilityMethods.CompareValues<decimal>("Modules.Module.Options.Option.FinalPrice",
        //                            actualOption.FinalPrice, expectedOption.FinalPrice);
        //                }
        //                matchFlag &= UtilityMethods.CompareValues<int>("Modules.Module.ModuleId", actualmodule.ModuleId,
        //                    expectedModule.ModuleId);
        //                matchFlag &= UtilityMethods.CompareValues<string>("Modules.Module.ModuleDesc",
        //                    actualmodule.ModuleDesc, expectedModule.ModuleDesc);
        //                matchFlag &= UtilityMethods.CompareValues<bool>("Modules.Module.Required", actualmodule.Required,
        //                    expectedModule.Required);
        //                matchFlag &= UtilityMethods.CompareValues<bool>("Modules.Module.MultiSelect",
        //                    actualmodule.MultiSelect, expectedModule.MultiSelect);
        //                matchFlag &= UtilityMethods.CompareValues<string>("Modules.Module.DefaultOptionId",
        //                    actualmodule.DefaultOptionId, expectedModule.DefaultOptionId);
        //                matchFlag &= UtilityMethods.CompareValues<decimal>("Modules.Module.DefaultOptionPrice",
        //                    actualmodule.DefaultOptionPrice, expectedModule.DefaultOptionPrice);
        //            }
        //        }
        //        // Lease
        //        matchFlag &= UtilityMethods.CompareValues<string>("Lease.LeaseTerms", actualCatalogItem.Lease.LeaseTerms, expectedCatalogItem.Lease.LeaseTerms);
        //        matchFlag &= UtilityMethods.CompareValues<string>("Lease.Frequency", actualCatalogItem.Lease.Frequency, expectedCatalogItem.Lease.Frequency);
        //        matchFlag &= UtilityMethods.CompareValues<decimal>("Lease.LeaseTerms", actualCatalogItem.Lease.LRF, expectedCatalogItem.Lease.LRF);
        //        matchFlag &= UtilityMethods.CompareValues<string>("Lease.LeaseTerms", actualCatalogItem.Lease.Disposition, expectedCatalogItem.Lease.Disposition);
        //        matchFlag &= UtilityMethods.CompareValues<decimal>("Lease.LeaseTerms", actualCatalogItem.Lease.ExpensedTotal, expectedCatalogItem.Lease.ExpensedTotal);

        //        // Delta Comments
        //        matchFlag &= UtilityMethods.CompareValues<string>("DeltaComments.Operation", actualCatalogItem.DeltaComments.Operation, expectedCatalogItem.DeltaComments.Operation);

        //        // Delta Change
        //        matchFlag &= UtilityMethods.CompareValues<DeltaStatus>("DeltaChange", actualCatalogItem.DeltaChange, expectedCatalogItem.DeltaChange);
        //    }

        //    return matchFlag;
        //}

        /// <summary>
        /// Validate emails for generated catalog
        /// </summary>
        /// <param name="identityName">Identity name</param>
        /// <param name="anyTimeAfter">Time after which the catalog was generated</param>
        /// <param name="catalogOperation">Catalog operation</param>
        public void ValidateCatalogEMails(string identityName, DateTime anyTimeAfter, CatalogOperation catalogOperation)
        {
            EmailHelper emailHelper = new EmailHelper();
            List<Item> emails = emailHelper.GetEmails("US B2B Support", identityName, anyTimeAfter, catalogOperation);

            if (catalogOperation == CatalogOperation.Create)
            {
                emails.Count().Should().Be(1, "Error: Email count validation failed");
                emails.ElementAt(0).Subject.Should().Contain("Test - B2B Original Catalog Create SUCCESS", "Error: Email Subject validation failed");
            }
            else if (catalogOperation == CatalogOperation.CreateAndPublish)
            {
                emails.Count().Should().Be(2, "Error: Email count validation failed");
                emails.Where(e => e.Subject.Contains("Create")).ElementAt(0).Subject.Should().Contain("Test - B2B Original Catalog Create SUCCESS", "Error: Create Email Subject validation failed");
                emails.Where(e => e.Subject.Contains("Publish")).ElementAt(0).Subject.Should().Contain("Test - B2B Original Catalog Publish SUCCESS", "Error: Publish Email Subject validation failed");
            }
        }

        /// <summary>
        /// Publish the catalog to Processor
        /// </summary>
        /// <param name="environment">Prod/Prev</param>
        /// <param name="profileName">Name of Profile</param>
        /// <param name="identityName">Name of Identity</param>
        /// <param name="catalogType">Original/Delta</param>
        internal void PublishCatalogByClickOnce(B2BEnvironment b2BEnvironment, string profileName, string identityName, CatalogType catalogType)
        {
            B2BChannelUx b2BChannelUx = new B2BChannelUx(webDriver);
            b2BChannelUx.OpenCreateInstantCatalogPage(b2BEnvironment);

            if (b2BEnvironment == B2BEnvironment.Production)
            {
                UtilityMethods.ClickElement(webDriver, b2BChannelUx.ProductionEnvRadioButton);
            }
            else if (b2BEnvironment == B2BEnvironment.Preview)
            {
                UtilityMethods.ClickElement(webDriver, b2BChannelUx.PreviewEnvRadioButton);
            }
            WaitForPageRefresh();
            b2BChannelUx.SelectOption(b2BChannelUx.SelectCustomerProfileDiv, profileName.ToUpper());
            b2BChannelUx.SelectOption(b2BChannelUx.SelectProfileIdentityDiv, identityName.ToUpper());

            if (!b2BChannelUx.UseExistingB2BAutoScheduleRadioButton.Selected)
                b2BChannelUx.UseExistingB2BAutoScheduleRadioButton.Click();

            if (catalogType == CatalogType.Original)
                b2BChannelUx.OriginalRadioButton.Click();
            else if (catalogType == CatalogType.Delta)
                b2BChannelUx.DeltaRadioButton.Click();

            //b2BChannelUx.ClickToPublishButton.Click();
            //IAlert successAlert = webDriver.WaitGetAlert(CatalogTimeOuts.AlertTimeOut);
            //successAlert.Accept();

            b2BChannelUx.CreateButton.Click();
            b2BChannelUx.WaitForFeedBackMessage(TimeSpan.FromMinutes(2));
            b2BChannelUx.FeedBackMessage.Text.ShouldBeEquivalentTo("Auto Catalog generation successfully initiated. Please check it on the Auto Catalog & Inventory List page after sometime.");

            //b2BChannelUx.ClickToPublishButton.Click();
        }

        public string CreateInstantCatalogSetNew(B2BEnvironment b2BEnvironment, string profileName, string identityName, CatalogType catalogType, CatalogItemType[] catalogItemTypeList)
        {
            B2BChannelUx b2BChannelUx = new B2BChannelUx(webDriver);
            b2BChannelUx.OpenCreateInstantCatalogPage(b2BEnvironment);

            if (b2BEnvironment == B2BEnvironment.Production)
                UtilityMethods.ClickElement(webDriver, b2BChannelUx.ProductionEnvRadioButton);
            else if (b2BEnvironment == B2BEnvironment.Preview)
                UtilityMethods.ClickElement(webDriver, b2BChannelUx.PreviewEnvRadioButton);

            b2BChannelUx.SelectOption(b2BChannelUx.SelectCustomerProfileDiv, profileName.ToUpper());
            b2BChannelUx.SelectOption(b2BChannelUx.SelectProfileIdentityDiv, identityName.ToUpper());

            if (!b2BChannelUx.SetNewRadioButton.Selected)
                b2BChannelUx.SetNewRadioButton.Click();

            if (b2BChannelUx.STDSetNewCheckBox.Selected)
                b2BChannelUx.STDSetNewCheckBox.Click();

            foreach (CatalogItemType catalogItemType in catalogItemTypeList)
            {
                if (catalogItemType == CatalogItemType.ConfigWithDefaultOptions && !b2BChannelUx.STDSetNewCheckBox.Selected)
                    b2BChannelUx.STDSetNewCheckBox.Click();
                if (catalogItemType == CatalogItemType.SNP && !b2BChannelUx.SNPSetNewCheckBox.Selected)
                    b2BChannelUx.SNPSetNewCheckBox.Click();
                if (catalogItemType == CatalogItemType.Systems && !b2BChannelUx.SYSSetNewCheckBox.Selected)
                    b2BChannelUx.SYSSetNewCheckBox.Click();
            }

            if (catalogType == CatalogType.Original)
                b2BChannelUx.OriginalRadioButton.Click();
            else if (catalogType == CatalogType.Delta)
                b2BChannelUx.DeltaRadioButton.Click();

            b2BChannelUx.CreateAndDownloadButton.Click();
            WebDriverWait wait = new WebDriverWait(webDriver, TimeSpan.FromMinutes(1));
            wait.Until(drv => drv.FindElement(By.Id("imgLoading")).Displayed);
            wait.Until(drv => !drv.FindElement(By.Id("imgLoading")).Displayed);

            string validationMessage = string.Empty;
            if (b2BChannelUx.FeedBackMessage.IsElementVisible())
                validationMessage = b2BChannelUx.FeedBackMessage.Text;

            return validationMessage;
        }

        internal void ValidateP2PMessage(B2BEnvironment environment, string profileName, string identityName, CatalogType catalogType)
        {
            webDriver.Navigate().GoToUrl(ConfigurationManager.AppSettings["TestHarnessPageUrl"] + ((environment == B2BEnvironment.Production) ? "P" : "U"));
            B2BChannelUx b2BChannelUx = new B2BChannelUx(webDriver);

            if (environment == B2BEnvironment.Production)
                b2BChannelUx.ProductionEnvRadioButton.Click();
            else if (environment == B2BEnvironment.Preview)
                b2BChannelUx.PreviewEnvRadioButton.Click();

            b2BChannelUx.SelectOption(b2BChannelUx.SelectCustomerProfileDiv, profileName.ToUpper());
            b2BChannelUx.SelectOption(b2BChannelUx.SelectProfileIdentityDiv, identityName.ToUpper());

            if (!b2BChannelUx.UseExistingB2BAutoScheduleRadioButton.Selected)
                b2BChannelUx.UseExistingB2BAutoScheduleRadioButton.Click();

            if (catalogType == CatalogType.Original)
                b2BChannelUx.OriginalRadioButton.Click();
            else if (catalogType == CatalogType.Delta)
                b2BChannelUx.DeltaRadioButton.Click();
            b2BChannelUx.CreateButton.Click();
            WaitForPageRefresh();
            Console.WriteLine("Actual: " + b2BChannelUx.FeedBackMessage.Text);
            b2BChannelUx.FeedBackMessage.Text.Trim().ShouldBeEquivalentTo("Catalog Creation is not allowed for this profile since Enable BHC Catalog Auto Generation is turned OFF.");
        }

        internal void ValidateErrorMessageNoConfigSelected(B2BEnvironment environment, string profileName, string identityName, CatalogType catalogType,
            SetNewValidation setnew, CatalogItemType catalogItemType)
        {
            webDriver.Navigate().GoToUrl(ConfigurationManager.AppSettings["TestHarnessPageUrl"] + ((environment == B2BEnvironment.Production) ? "P" : "U"));
            B2BChannelUx b2BChannelUx = new B2BChannelUx(webDriver);

            if (environment == B2BEnvironment.Production)
                b2BChannelUx.ProductionEnvRadioButton.Click();
            else if (environment == B2BEnvironment.Preview)
                b2BChannelUx.PreviewEnvRadioButton.Click();

            b2BChannelUx.SelectOption(b2BChannelUx.SelectCustomerProfileDiv, profileName.ToUpper());
            b2BChannelUx.SelectOption(b2BChannelUx.SelectProfileIdentityDiv, identityName.ToUpper());
            b2BChannelUx.SetNewRadioButton.Click();

            if (setnew == SetNewValidation.NoConfig && b2BChannelUx.STDSetNewCheckBox.Selected)
                b2BChannelUx.STDSetNewCheckBox.Click();
            else
            {
                b2BChannelUx.STDSetNewCheckBox.Click();
                switch (catalogItemType)
                {
                    case CatalogItemType.ConfigWithDefaultOptions:
                        b2BChannelUx.STDSetNewCheckBox.Click();
                        break;
                    case CatalogItemType.SNP:
                        b2BChannelUx.SNPSetNewCheckBox.Click();
                        break;
                    case CatalogItemType.Systems:
                        b2BChannelUx.SYSSetNewCheckBox.Click();
                        break;
                    default:
                        b2BChannelUx.STDSetNewCheckBox.Click();
                        b2BChannelUx.SNPSetNewCheckBox.Click();
                        b2BChannelUx.SYSSetNewCheckBox.Click();
                        break;
                }
            }

            if (catalogType == CatalogType.Original)
                b2BChannelUx.OriginalRadioButton.Click();
            else if (catalogType == CatalogType.Delta)
                b2BChannelUx.DeltaRadioButton.Click();

            b2BChannelUx.CreateAndDownloadButton.Click();

            b2BChannelUx.WaitForFeedBackMessage(TimeSpan.FromMinutes(2));
            switch (setnew)
            {
                case SetNewValidation.NoConfig:
                    b2BChannelUx.ValidationMessage.Text.Trim().ShouldBeEquivalentTo("Minimum one configuration is required for the catalog creation.");
                    break;
                case SetNewValidation.DeltaWithoutOriginal:
                    b2BChannelUx.ValidationMessage.Text.Trim().ShouldBeEquivalentTo("Instant Original Catalog doesn't exist. Please create an original catalog then initiate delta.");
                    break;
                case SetNewValidation.DeltaWithOriginal:
                    b2BChannelUx.ValidationMessage.Text.Trim().ShouldBeEquivalentTo("Instant Auto Catalog with original exists for STD. Please create an original with the new configuration type and then initiate delta.");
                    break;
                default:
                    break;
            }
        }

        internal void ValidateInstantCatalogErrorMessage(B2BEnvironment environment, string profileName, string identityName, CatalogType catalogType,
        CatalogItemType catalogItemType, string accessGroup, ErrorMessages errorMessages, bool isSetNew)
        {
            webDriver.Navigate().GoToUrl(ConfigurationManager.AppSettings["TestHarnessPageUrl"] + ((environment == B2BEnvironment.Production) ? "P" : "U"));
            B2BChannelUx b2BChannelUx = new B2BChannelUx(webDriver);

            if (environment == B2BEnvironment.Production)
                b2BChannelUx.ProductionEnvRadioButton.Click();
            else if (environment == B2BEnvironment.Preview)
                b2BChannelUx.PreviewEnvRadioButton.Click();

            b2BChannelUx.SelectOption(b2BChannelUx.SelectCustomerProfileDiv, profileName);
            b2BChannelUx.SelectOption(b2BChannelUx.SelectProfileIdentityDiv, identityName.ToUpper());

            if (isSetNew)
            {
                if (!b2BChannelUx.SetNewRadioButton.Selected)
                    b2BChannelUx.SetNewRadioButton.Click();

                switch (catalogItemType)
                {
                    case CatalogItemType.ConfigWithDefaultOptions:
                        if (!b2BChannelUx.STDSetNewCheckBox.Selected)
                            b2BChannelUx.STDSetNewCheckBox.Click();
                        break;
                    case CatalogItemType.SNP:
                        b2BChannelUx.SNPSetNewCheckBox.Click();
                        break;
                    case CatalogItemType.Systems:
                        b2BChannelUx.SYSSetNewCheckBox.Click();
                        break;
                    default:
                        b2BChannelUx.STDSetNewCheckBox.Click();
                        b2BChannelUx.SNPSetNewCheckBox.Click();
                        b2BChannelUx.SYSSetNewCheckBox.Click();
                        break;
                }

                if (catalogType == CatalogType.Original)
                    b2BChannelUx.OriginalRadioButton.Click();
                else if (catalogType == CatalogType.Delta)
                    b2BChannelUx.DeltaRadioButton.Click();

                b2BChannelUx.CreateAndDownloadButton.Click();

            }
            else
            {
                if (!b2BChannelUx.UseExistingB2BAutoScheduleRadioButton.Selected)
                    b2BChannelUx.UseExistingB2BAutoScheduleRadioButton.Click();

                if (catalogType == CatalogType.Original)
                    b2BChannelUx.OriginalRadioButton.Click();
                else if (catalogType == CatalogType.Delta)
                    b2BChannelUx.DeltaRadioButton.Click();

                b2BChannelUx.CreateButton.Click();

                WaitForPageRefresh();
            }

            b2BChannelUx.WaitForFeedBackMessage(TimeSpan.FromMinutes(2));
            switch (errorMessages)
            {
                case ErrorMessages.AccessGroupNotAssociated:
                    if (catalogItemType == CatalogItemType.ConfigWithDefaultOptions || catalogItemType == CatalogItemType.ConfigWithUpsellDownsell)
                    {
                        string stdError = string.Concat("Error while generating Auto Catalog XML. STD Catalog is not Enabled in the OST for the Access Group " + accessGroup);
                        b2BChannelUx.ValidationMessage.Text.Trim().ShouldBeEquivalentTo(stdError);
                    }

                    if (catalogItemType == CatalogItemType.SNP)
                    {
                        string snpError = string.Concat("Error while generating Auto Catalog XML. SNP Catalog is not Enabled in the OST for the Access Group " + accessGroup);
                        b2BChannelUx.ValidationMessage.Text.Trim().ShouldBeEquivalentTo(snpError);
                    }

                    if (catalogItemType == CatalogItemType.Systems)
                    {
                        string sysError = string.Concat("Error while generating Auto Catalog XML. SYS Catalog is not Enabled in the OST for the Access Group " + accessGroup);
                        b2BChannelUx.ValidationMessage.Text.Trim().ShouldBeEquivalentTo(sysError);
                    }

                    break;

                case ErrorMessages.ZeroCatalogItems:
                    b2BChannelUx.ValidationMessage.Text.Trim().ShouldBeEquivalentTo("Error while generating Auto Catalog XML. Auto Catalog creation failed due to zero items in the OST Store.");
                    break;
                case ErrorMessages.IdentityIsDisabled:
                    b2BChannelUx.ValidationMessage.Text.Trim().ShouldBeEquivalentTo("Auto Catalog Creation is not allowed for this profile since the selected identity is disabled for this profile.");
                    break;
                case ErrorMessages.DeltaCatalogCheckBoxIsDisabled:
                    b2BChannelUx.ValidationMessage.Text.Trim().ShouldBeEquivalentTo("Delta Catalog Creation is not allowed for this profile since Delta Catalog is turned OFF for this profile.");
                    break;
                default:
                    break;
            }
        }

        internal void ValidateErrorMessageWhileCreatingDeltaCatalog(B2BEnvironment environment, string profileName, string identityName, CatalogType catalogType)
        {
            webDriver.Navigate().GoToUrl(ConfigurationManager.AppSettings["TestHarnessPageUrl"] + ((environment == B2BEnvironment.Production) ? "P" : "U"));
            B2BChannelUx b2BChannelUx = new B2BChannelUx(webDriver);

            if (environment == B2BEnvironment.Production)
                b2BChannelUx.ProductionEnvRadioButton.Click();
            else if (environment == B2BEnvironment.Preview)
                b2BChannelUx.PreviewEnvRadioButton.Click();

            b2BChannelUx.SelectOption(b2BChannelUx.SelectCustomerProfileDiv, profileName.ToUpper());
            b2BChannelUx.SelectOption(b2BChannelUx.SelectProfileIdentityDiv, identityName.ToUpper());

            if (catalogType == CatalogType.Original)
                b2BChannelUx.OriginalRadioButton.Click();
            else if (catalogType == CatalogType.Delta)
                b2BChannelUx.DeltaRadioButton.Click();

            b2BChannelUx.CreateButton.Click();

            WaitForPageRefresh();

            Console.WriteLine("Actual: " + b2BChannelUx.FeedBackMessage.Text);
            b2BChannelUx.FeedBackMessage.Text.Trim().ShouldBeEquivalentTo("Instant Original Catalog doesn't exist. Please create an original catalog then initiate delta.");
        }

        /// <summary>
        /// Search for a catalog in Auto Catalog List & Inventory page 
        /// </summary>
        /// <param name="profileName">Profile name</param>
        /// <param name="identityName">Identity name</param>
        /// <param name="anyTimeAfter">Time after which the catalog is processed</param>
        /// <param name="operation">Catalog operation i.e. 'Create' or 'Create & Publish'</param>
        public void SearchCatalog(string profileName, string identityName, DateTime anyTimeAfter, CatalogStatus catalogStatus, CatalogTestOrLive catalogTestOrLive = CatalogTestOrLive.None)
        {
            CPTAutoCatalogInventoryListPage autoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            autoCatalogListPage.SelectOption(autoCatalogListPage.SelectRegionSpan, "US");
            autoCatalogListPage.SelectOption(autoCatalogListPage.SelectCustomerNameSpan, profileName.ToUpper());
            autoCatalogListPage.SelectOption(autoCatalogListPage.SelectIdentityNameSpan, identityName.ToUpper());
            autoCatalogListPage.SelectCatalogTestOrLive(catalogTestOrLive);
            autoCatalogListPage.CatalogRadioButton.Click();
            autoCatalogListPage.SearchRecordsLink.Click();
            autoCatalogListPage.CatalogsTable.WaitForElementVisible(TimeSpan.FromSeconds(30));
            autoCatalogListPage.WaitForCatalogInSearchResult(anyTimeAfter.ConvertToUtcTimeZone(), catalogStatus);
        }

        /// <summary>
        /// Search for a catalog in Auto Catalog List & Inventory page 
        /// </summary>
        /// <param name="profileName">Profile name</param>
        /// <param name="identityName">Identity name</param>
        /// <param name="anyTimeAfter">Time after which the catalog is processed</param>
        /// <param name="operation">Catalog operation i.e. 'Create' or 'Create & Publish'</param>
        public void SearchCatalog(string profileName, string identityName, DateTime anyTimeAfter, CatalogStatus catalogStatus, CatalogType catalogType, CatalogTestOrLive catalogTestOrLive = CatalogTestOrLive.None, bool statusFilter = true)
        {
            CPTAutoCatalogInventoryListPage autoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            autoCatalogListPage.SelectOption(autoCatalogListPage.SelectRegionSpan, "US");
            autoCatalogListPage.SelectOption(autoCatalogListPage.SelectCustomerNameSpan, profileName.ToUpper());
            autoCatalogListPage.SelectOption(autoCatalogListPage.SelectIdentityNameSpan, identityName.ToUpper());
            autoCatalogListPage.CatalogRadioButton.Click();
            if (autoCatalogListPage.OriginalCatalogCheckbox.Selected != (catalogType == CatalogType.Original))
                autoCatalogListPage.OriginalCatalogCheckbox.Click();
            else if (autoCatalogListPage.DeltaCatalogCheckbox.Selected != (catalogType == CatalogType.Delta))
                autoCatalogListPage.DeltaCatalogCheckbox.Click();
            if (statusFilter)
                autoCatalogListPage.SelectTheStatus(UtilityMethods.ConvertToString(catalogStatus));
            autoCatalogListPage.SelectCatalogTestOrLive(catalogTestOrLive);
            autoCatalogListPage.SearchRecordsLink.Click();
            autoCatalogListPage.CatalogsTable.WaitForElementVisible(TimeSpan.FromSeconds(30));
            autoCatalogListPage.WaitForCatalogInSearchResult(anyTimeAfter.ConvertToUtcTimeZone(), catalogStatus);
        }

        /// <summary>
        /// Validate catalog details in Auto Catalog List & Inventory page
        /// </summary>
        /// <param name="catalogItemType">Catalog Item Type</param>
        /// <param name="catalogType">Catalog Type</param>
        /// <param name="catalogOperation">Catalog Operation</param>
        /// <param name="anyTimeAfter">Time after which catalog was generated</param>
        public void ValidateCatalogSearchResult(CatalogItemType[] catalogItemType, CatalogType catalogType, CatalogStatus catalogStatus, DateTime anyTimeAfter)
        {
            CPTAutoCatalogInventoryListPage autoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            autoCatalogListPage.CatalogsTable.GetCellValue(1, "Last Status Date").Trim().ConvertToDateTime().AddMinutes(1).Should().BeAfter(anyTimeAfter.ConvertToUtcTimeZone(), "Catalog is not displayed in Search Result");
            autoCatalogListPage.CatalogsTable.GetCellValue(1, "Type").Should().Be(catalogType.ConvertToString(), "Expected Catalog type is incorrect");
            autoCatalogListPage.CatalogsTable.GetCellValue(1, "Status").Should().Contain(catalogStatus.ConvertToString(), "Catalog status is not as expected");
            //status == CatalogStatus.Created || status == CatalogStatus.CreatedWarning
        }

        public void ValidateCatalogSearchResult(CatalogType catalogType, CatalogStatus catalogStatus, DateTime anyTimeAfter)
        {
            CPTAutoCatalogInventoryListPage autoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            autoCatalogListPage.CatalogsTable.GetCellValue(1, "Last Status Date").Trim().ConvertToDateTime().AddMinutes(1).Should().BeAfter(anyTimeAfter.ConvertToUtcTimeZone(), "Catalog is not displayed in Search Result");
            autoCatalogListPage.CatalogsTable.GetCellValue(1, "Type").Should().Be(catalogType.ConvertToString(), "Expected Catalog type is incorrect");
            autoCatalogListPage.CatalogsTable.GetCellValue(1, "Status").Should().Be(catalogStatus.ConvertToString(), "Catalog status is not as expected");
            autoCatalogListPage.CatalogsTable.GetCellValue(1, "Requester").ToLowerInvariant().Should().Be(windowsLogin, "Requestor name is different than windows NT user name");
        }

        public string GetThreadID(CatalogItemType[] catalogItemType, CatalogType catalogType, CatalogStatus catalogStatus, DateTime anyTimeAfter)
        {
            CPTAutoCatalogInventoryListPage autoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            return autoCatalogListPage.CatalogsTable.GetCellValue(1, "Thread Id").ToString();
        }

        /// <summary>
        /// Download catalog from Auto Catalog List & Inventory page
        /// </summary>
        /// <param name="identityName">Identity Name</param>
        /// <param name="anyTimeAfter">Time after which the catalog is processed</param>
        /// <returns>File name for the downloaded catalog</returns>
        public string DownloadCatalog(string identityName, DateTime anyTimeAfter)
        {
            CPTAutoCatalogInventoryListPage autoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            UtilityMethods.ClickElement(webDriver, autoCatalogListPage.GetDownloadButton(1));
            string downloadPath = ConfigurationManager.AppSettings["CatalogDownloadPath"];

            webDriver.WaitForDownLoadToComplete(downloadPath, identityName, anyTimeAfter, TimeSpan.FromMinutes(1));
            string fileName = new DirectoryInfo(downloadPath).GetFiles().OrderByDescending(p => p.CreationTime).AsEnumerable()
                .Where(file => file.Name.StartsWith(identityName.ToUpper()) && file.CreationTime > anyTimeAfter)
                .FirstOrDefault().FullName;

            return fileName;
        }

        /// <summary>
        /// It verifies whether Order code exests or not
        /// </summary>
        /// <param name="filePath">XML file path</param>
        /// <param name="itemOrderCode">itemOrderCode that need to check</param>
        /// <returns>If itemOrderCode exists it returns true</returns>
        public bool VerifyOrderCodeExistsInCatalogFile(string filePath, string itemOrderCode)
        {

            B2BXML actualCatalog = XMLDeserializer<B2BXML>.DeserializeFromXmlFile(filePath);

            bool matchFlag = false;

            foreach (CatalogItem actualCatalogItem in actualCatalog.BuyerCatalog.CatalogDetails.CatalogItem)
            {

                if (UtilityMethods.CompareValues<string>("ItemOrderCode", actualCatalogItem.ItemOrderCode, itemOrderCode))
                {
                    matchFlag = true;
                    break;
                }
            }

            return matchFlag;
        }

        /// <summary>
        /// It verifies whether Order code exests or not
        /// </summary>
        /// <param name="filePath">XML file path</param>
        /// <param name="itemOrderCode">itemOrderCode that need to check</param>
        /// <returns>If itemOrderCode exists it returns true</returns>
        public bool VerifyOrderCodeExistsInCatalogFile(string filePath, CatalogItemType[] catalogItemType, ConfigRules configRules)
        {
            B2BXML actualCatalog = XMLDeserializer<B2BXML>.DeserializeFromXmlFile(filePath);
            bool matchFlag = true;
            IEnumerable<CatalogItem> actualCatalogItem = null;
            foreach (CatalogItemType itemType in catalogItemType)
            {
                switch (itemType)
                {
                    case CatalogItemType.ConfigWithDefaultOptions:
                        actualCatalogItem = actualCatalog.BuyerCatalog.CatalogDetails.CatalogItem.Where(ci => ci.ItemType.ToString() == "BTS");
                        break;
                    case CatalogItemType.Systems:
                        actualCatalogItem = actualCatalog.BuyerCatalog.CatalogDetails.CatalogItem.Where(ci => ci.ItemType.ToString() == "BTS");
                        break;
                    case CatalogItemType.ConfigWithUpsellDownsell:
                        actualCatalogItem = actualCatalog.BuyerCatalog.CatalogDetails.CatalogItem.Where(ci => ci.ItemType.ToString() == "BTS");
                        break;
                    default:
                        actualCatalogItem = actualCatalog.BuyerCatalog.CatalogDetails.CatalogItem.Where(ci => ci.ItemType.ToString() == "");
                        break;
                }
                foreach (CatalogItem catalogItem in actualCatalogItem)
                {
                    if (configRules == ConfigRules.LeadTimeON)
                        matchFlag &= UtilityMethods.CompareValues<int>("LeadTime", catalogItem.LeadTime, 3, Computation.LessThanOrEqualTo);
                    else if (configRules == ConfigRules.LeadTimeOff)
                        matchFlag &= UtilityMethods.CompareValues<int>("LeadTime", catalogItem.LeadTime, 0, Computation.GreaterThanOrEqualTo);
                }
            }
            return matchFlag;
        }

        /// <summary>
        /// Waits for the page to refresh after navigation
        /// </summary>
        public void WaitForPageRefresh()
        {
            var isloaded = string.Empty;
            do
            {
                Thread.Sleep(4000);
                try
                {
                    isloaded = javaScriptExecutor.ExecuteScript("return window.document.readyState") as string;
                }
                catch
                {
                    // ignored
                }
            } while (isloaded != "complete");
        }

        /// <summary>
        /// It verifies particular field value with respective Order code or SNP BaseSKUId, and if which are matched with expected values, it returns true
        /// </summary>
        /// <param name="filePath">XML file path</param>
        /// <param name="itemOrderCode">ItemOrderCode or SNP BaseSKUId</param>
        /// <param name="fieldName">Field Name</param>
        /// <param name="expectedFieldValue">Expected Field Value for a particular field</param>
        /// <param name="isSNP">If isSNP is true, it verifies the values based on Base SKU Id</param>
        /// <returns> Expected Field Value for a particular field is matched with expected value, it returns true</returns>
        public bool VerifyFieldValueforAnOrderCode(string filePath, string itemOrderCode, string fieldName, string expectedFieldValue, bool isSNP)
        {
            B2BXML actualCatalog = XMLDeserializer<B2BXML>.DeserializeFromXmlFile(filePath);

            bool matchFlag = true;
            string actualFieldValue = string.Empty;

            if (isSNP)
            {
                CatalogItem actualCatalogItem = actualCatalog.BuyerCatalog.CatalogDetails.CatalogItem.Where(ci => ci.BaseSKUId == itemOrderCode).FirstOrDefault();
                actualFieldValue = actualCatalogItem.GetType().GetProperty(fieldName).GetValue(actualCatalogItem, null).ToString();
            }
            else
            {
                CatalogItem actualCatalogItem = actualCatalog.BuyerCatalog.CatalogDetails.CatalogItem.Where(ci => ci.ItemOrderCode == itemOrderCode).FirstOrDefault();
                actualFieldValue = actualCatalogItem.GetType().GetProperty(fieldName).GetValue(actualCatalogItem, null).ToString();
            }
            matchFlag &= UtilityMethods.CompareValues<string>("fieldName", actualFieldValue, expectedFieldValue);

            return matchFlag;
        }

        internal bool ValidateCRT(B2BEnvironment b2BEnvironment, string profileName, string catalogXMLFilePath)
        {
            B2BHomePage b2BHomePage = new B2BHomePage(webDriver);
            b2BHomePage.OpenB2BHomePage(b2BEnvironment);
            //b2BHomePage.CRAssociationListLink.Click();

            B2BCrossReferenceAssociationPage b2BCrossReferenceAssociationPage = new B2BCrossReferenceAssociationPage(webDriver);
            b2BCrossReferenceAssociationPage.OpenCrossReferenceListPage(b2BEnvironment);
            b2BCrossReferenceAssociationPage.AccountName.SelectByValue(profileName);
            b2BCrossReferenceAssociationPage.Search.Click();
            //b2BCrossReferenceAssociationPage.OpenCRTXML(profileName);
            string CRId = b2BCrossReferenceAssociationPage.GetCRTID(profileName.ToUpper());
            webDriver.Navigate().GoToUrl(ConfigurationReader.GetValue("CrossReferenceXMLPage") + CRId);

            //IReadOnlyCollection<string> windowHandles = webDriver.WindowHandles;
            //webDriver.SwitchTo().Window(webDriver.WindowHandles.Last());

            BrowserName browser = webDriver.GetBrowserName();
            bool matchflag = true;
            B2BXML CatalogXML = XMLDeserializer<B2BXML>.DeserializeFromXmlFile(catalogXMLFilePath);
            List<CatalogItem> validCRTCatalogItems = CatalogXML.BuyerCatalog.CatalogDetails.CatalogItem.Where(ci => ((ci.CatalogItemType == CatalogItemType.ConfigWithDefaultOptions || ci.CatalogItemType == CatalogItemType.ConfigWithUpsellDownsell || ci.CatalogItemType == CatalogItemType.Systems)
                && !string.IsNullOrEmpty(ci.ManufacturerPartNumber)) || (ci.CatalogItemType == CatalogItemType.SNP && !string.IsNullOrEmpty(ci.BaseSKUId))).ToList();
            if (browser == BrowserName.MicrosoftEdge)
            {
                WaitForPageRefresh();
                string pageSource = webDriver.PageSource;
                foreach (CatalogItem catalogItem in validCRTCatalogItems)
                {
                    string id = string.Empty;
                    switch (catalogItem.CatalogItemType)
                    {
                        case CatalogItemType.ConfigWithDefaultOptions:
                        case CatalogItemType.ConfigWithUpsellDownsell:
                        case CatalogItemType.Systems:
                            id = catalogItem.ManufacturerPartNumber;
                            break;
                        case CatalogItemType.SNP:
                            id = catalogItem.BaseSKUId;
                            break;
                    }
                    matchflag = pageSource.Contains(catalogItem.PartId);
                    matchflag = pageSource.Contains(catalogItem.ListPrice);
                    matchflag = pageSource.Contains(id);
                }
            }
            else
            {
                string crtXMLText = webDriver.FindElement(By.CssSelector("div[class='pretty-print']")).Text;
                CRTXML CrtXML = XMLDeserializer<CRTXML>.DeserializeFromXmlString(crtXMLText);
                CrtXML.CrossReference.CRTValues.CRTValue.Count().Should().Be(validCRTCatalogItems.Count, "CRT XML Count does not match with catalog item count");

                foreach (CatalogItem catalogItem in validCRTCatalogItems)
                {
                    string id = string.Empty;
                    switch (catalogItem.CatalogItemType)
                    {
                        case CatalogItemType.ConfigWithDefaultOptions:
                        case CatalogItemType.ConfigWithUpsellDownsell:
                        case CatalogItemType.Systems:
                            id = catalogItem.ManufacturerPartNumber;
                            break;
                        case CatalogItemType.SNP:
                            id = catalogItem.BaseSKUId;
                            break;
                    }

                    matchflag &= UtilityMethods.CompareValues<string>("ID", CrtXML.CrossReference.CRTValues.CRTValue.Where(crt => crt.Id == catalogItem.ManufacturerPartNumber).First().Item.Where(item => item.Id == "ID").First().Data, id);
                    matchflag &= UtilityMethods.CompareValues<string>("Buyer Code", CrtXML.CrossReference.CRTValues.CRTValue.Where(crt => crt.Id == catalogItem.ManufacturerPartNumber).First().Item.Where(item => item.Id == "buyer_code").First().Data, catalogItem.PartId);
                    matchflag &= UtilityMethods.CompareValues<string>("Price", CrtXML.CrossReference.CRTValues.CRTValue.Where(crt => crt.Id == catalogItem.ManufacturerPartNumber).First().Item.Where(item => item.Id == "price").First().Data, catalogItem.UnitPrice.ToString());
                    matchflag &= UtilityMethods.CompareValues<string>("Buyer Code Type", CrtXML.CrossReference.CRTValues.CRTValue.Where(crt => crt.Id == catalogItem.ManufacturerPartNumber).First().Item.Where(item => item.Id == "buyer_code_type").First().Data, "B2B Quote");
                }
            }
            return matchflag;
        }

        public void VerifyCatalogExpirationDate(B2BEnvironment b2BEnvironment, CatalogType type, CatalogStatus status, ExpireDays days, 
            string profile, string identity, CatalogTestOrLive catalogTestOrLive = CatalogTestOrLive.None)
        {
            DateTime beforeSchedTime = DateTime.Now;
            CPTAutoCatalogInventoryListPage autoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver); ;
            B2BChannelUx b2bChannelUx = new B2BChannelUx(webDriver);
            DateTime catDateInListPage = DateTime.Now,
                        expDateInListPage = DateTime.Now,
                        catDateInXML = DateTime.Now, 
                        expDateInXML = DateTime.Now;
            if (profile.Contains("ChannelCatalog"))
            {
                PublishCatalogByClickOnce(b2BEnvironment, profile, identity, type);
                b2bChannelUx.OpenAutoCatalogAndInventoryListPage(b2BEnvironment);
                SearchCatalog(profile, identity, beforeSchedTime, status, type);
                ValidateCatalogSearchResult(type, status, beforeSchedTime);
            }
            else
            {
                b2bChannelUx.OpenAutoCatalogAndInventoryListPage(b2BEnvironment);
                autoCatalogListPage.SelectOption(autoCatalogListPage.SelectRegionSpan, "US");
                autoCatalogListPage.SelectOption(autoCatalogListPage.SelectCustomerNameSpan, profile.ToUpper());
                autoCatalogListPage.SelectOption(autoCatalogListPage.SelectIdentityNameSpan, identity.ToUpper());
                autoCatalogListPage.CatalogRadioButton.Click();
                if (type == CatalogType.Original)
                    autoCatalogListPage.OriginalCatalogCheckbox.Click();
                else
                    autoCatalogListPage.DeltaCatalogCheckbox.Click();

                autoCatalogListPage.SelectTheStatus(status.ToString());
                autoCatalogListPage.SelectCatalogTestOrLive(catalogTestOrLive);
                autoCatalogListPage.SearchRecordsLink.Click();
                autoCatalogListPage.CatalogsTable.WaitForElementVisible(TimeSpan.FromSeconds(30));
            }

            if (status == CatalogStatus.Failed)
            {
                catDateInListPage = DateTime.Parse(autoCatalogListPage.CatalogsTable.GetCellValue(1, "Last Status Date").Trim());
                expDateInListPage = DateTime.Parse(autoCatalogListPage.CatalogsTable.GetCellValue(1, "Expiration Date").Trim());
            }
            else
            {
                string filePath = DownloadCatalog(identity, beforeSchedTime);
            B2BXML actualcatalogXML = XMLDeserializer<B2BXML>.DeserializeFromXmlFile(filePath);
            CatalogHeader actualCatalogHeader = actualcatalogXML.BuyerCatalog.CatalogHeader;
                catDateInXML = DateTime.Parse(actualCatalogHeader.CatalogDate);
                expDateInXML = DateTime.Parse(actualCatalogHeader.ExpirationDate);
                catDateInListPage = DateTime.Parse(autoCatalogListPage.CatalogsTable.GetCellValue(1, "Last Status Date").Trim());
                expDateInListPage = DateTime.Parse(autoCatalogListPage.CatalogsTable.GetCellValue(1, "Expiration Date").Trim());
            }

            switch (days)
            {
                case ExpireDays.Thirty:
                    ValidateExpirationDate(catDateInXML, expDateInXML, catDateInListPage, expDateInListPage, status, 30);
                    break;
                case ExpireDays.Ninty:
                    ValidateExpirationDate(catDateInXML, expDateInXML, catDateInListPage, expDateInListPage, status, 90);
                    break;
                default:
                    ValidateExpirationDate(catDateInXML, expDateInXML, catDateInListPage, expDateInListPage, status, 180);
                    break;
            }
        }
        internal void ValidateExpirationDate(DateTime catDateInXML, DateTime expDateInXML, DateTime catDateInListPage, DateTime expDateInListPage, CatalogStatus status, double days)
        {
            if (status == CatalogStatus.Published || status == CatalogStatus.Created)
            {
                catDateInListPage.AddDays(days).ToString("dd/M/yyyy").ShouldBeEquivalentTo(expDateInListPage.ToString("dd/M/yyyy"), "Auto Catalog &Inventory List Page: Catalog Expiration Date is not equal to 'Effective date' + 30 days, 90 days or 180 days");
                catDateInXML.AddDays(days).ToString("dd/M/yyyy").ShouldBeEquivalentTo(expDateInXML.ToString("dd/M/yyyy"), "Catalog XML : Catalog Expiration Date is not equal to 'Effective date' + 30 days, 90 days or 180 days");
            expDateInXML.ToString("dd/M/yyyy").Should().Be(expDateInListPage.ToString("dd/M/yyyy"), "Catalog Expiration Date in Auto Catalog XML is not same as Auto Catalog & Inventory List Page");
            }
            else
            {
                catDateInListPage.AddDays(days).ToString("dd/M/yyyy").ShouldBeEquivalentTo(expDateInListPage.ToString("dd/M/yyyy"), "Auto Catalog &Inventory List Page: Catalog Expiration Date is not equal to 'Effective date' + 30 days, 90 days or 180 days");
            }
        }
        public bool ValidateUOMValue(string filePath, string orderCode, CatalogType catalogType, CatalogItemType[] catalogItemType)
        {
            bool matchFlag = true;

            B2BXML actualcatalogXML = XMLDeserializer<B2BXML>.DeserializeFromXmlFile(filePath);
            CatalogDetails actualCatalogDetails = actualcatalogXML.BuyerCatalog.CatalogDetails;

            string expectedCatalogFilePath = string.Empty;
            expectedCatalogFilePath = catalogType == CatalogType.Original
                ? Path.Combine(System.Environment.CurrentDirectory, "Catalog_OC_Expected.xml")
                : Path.Combine(System.Environment.CurrentDirectory, "Catalog_DC_Expected.xml");

            B2BXML expectedCatalogXML = XMLDeserializer<B2BXML>.DeserializeFromXmlFile(expectedCatalogFilePath);
            CatalogDetails expectedCatalogDetails = expectedCatalogXML.BuyerCatalog.CatalogDetails;
            IEnumerable<CatalogItem> actualCatalogItems = null; IEnumerable<CatalogItem> expectedCatalogItems = null;
            foreach (CatalogItemType item in catalogItemType)
            {
                actualCatalogItems = actualCatalogDetails.CatalogItem.Where(ci => ci.CatalogItemType == item);
                expectedCatalogItems = expectedCatalogDetails.CatalogItem.Where(ci => ci.CatalogItemType == item);
                try
                {
                    CatalogItem actualCatalogItem = actualCatalogItems.Where(ci => ci.BaseSKUId == orderCode).FirstOrDefault();
                    CatalogItem expectedCatalogItem = null;
                    expectedCatalogItem = actualCatalogItem.CatalogItemType == CatalogItemType.SNP
                    ? expectedCatalogItems.Where(ci => ci.BaseSKUId == actualCatalogItem.BaseSKUId).FirstOrDefault()
                    : expectedCatalogItems.Where(ci => ci.ItemOrderCode == actualCatalogItem.ItemOrderCode).FirstOrDefault();

                    Console.WriteLine(actualCatalogItem.BaseSKUId + " UOM Value: " + actualCatalogItem.UOM);
                    matchFlag &= UtilityMethods.CompareValues<string>("UOM", actualCatalogItem.UOM, expectedCatalogItem.UOM);
                }
                catch
                {
                    Console.WriteLine("Expecetd Order Code not found in Catalog XML");
                    matchFlag = false;
                }
            }
            return matchFlag;
        }
        /// <summary>
        /// Validate catalog details in Auto Catalog List & Inventory page
        /// </summary>
        /// <param name="catalogItemType">Catalog Item Type</param>
        /// <param name="catalogType">Catalog Type</param>
        /// <param name="catalogOperation">Catalog Operation</param>
        /// <param name="anyTimeAfter">Time after which catalog was generated</param>
        /// <param name="windowsLogin">Windows NT Login Account name</param>
        public void ValidateCatalogSearchResult(CatalogType catalogType, CatalogStatus catalogStatus, DateTime anyTimeAfter, RequestorValidation requestor = RequestorValidation.On)
        {
            CPTAutoCatalogInventoryListPage autoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            autoCatalogListPage.CatalogsTable.GetCellValue(1, "Last Status Date").Trim().ConvertToDateTime().AddMinutes(1).Should().BeAfter(anyTimeAfter.ConvertToUtcTimeZone(), "Catalog is not displayed in Search Result");
            autoCatalogListPage.CatalogsTable.GetCellValue(1, "Type").Should().Be(catalogType.ConvertToString(), "Expected Catalog type is incorrect");
            autoCatalogListPage.CatalogsTable.GetCellValue(1, "Status").Should().Be(catalogStatus.ConvertToString(), "Catalog status is not as expected");
            if (requestor == RequestorValidation.On)
                autoCatalogListPage.CatalogsTable.GetCellValue(1, "Requester").ToLowerInvariant().Should().Be(windowsLogin, "Requestor name is different than windows NT user name");
        }
        /// <summary>
        /// Compare Requestor Information
        /// </summary>
        /// <param name="filePath">XML file path</param>
        /// <param name="requestorName">Windows NT User Name</param>
        /// <returns>true/false</returns>
        public bool ValidateRequestorEmailIdInCatalogHeaderXML(string filePath, string requestorName)
        {
            string schemaPath = Path.Combine(System.Environment.CurrentDirectory, "CatalogSchema.xsd");
            string message = XMLSchemaValidator.ValidateSchema(filePath, schemaPath);
            message.Should().Be(string.Empty, "Error: One or more tags failed scehma validation. Please check the log for complete details");

            B2BXML actualcatalogXML = XMLDeserializer<B2BXML>.DeserializeFromXmlFile(filePath);
            CatalogHeader actualCatalogHeader = actualcatalogXML.BuyerCatalog.CatalogHeader;

            string fileName = new FileInfo(filePath).Name;
            bool matchFlag = true;
            matchFlag = UtilityMethods.CompareValues<string>("RequesterEmailId", actualCatalogHeader.RequesterEmailId.ToLowerInvariant(), requestorName.ToLowerInvariant());
            return matchFlag;
        }

        public void SearchInstantCatalog(B2BEnvironment environment, Region region, string profileName, string identityName,
            CatalogType catalogType, CatalogStatus catalogStatus, CatalogItemType[] catalogItemType, DateTime anyTimeAfter, CatalogTestOrLive catalogTestOrLive = CatalogTestOrLive.None)
        {
            webDriver.Navigate().GoToUrl(ConfigurationManager.AppSettings["AutoCatalogListPageUrl"] + ((environment == B2BEnvironment.Production) ? "P" : "U"));
            CPTAutoCatalogInventoryListPage autoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            autoCatalogListPage.SelectOption(autoCatalogListPage.SelectRegionSpan, region.ToString().ToUpper());
            autoCatalogListPage.SelectOption(autoCatalogListPage.SelectCustomerNameSpan, profileName.ToUpper());
            autoCatalogListPage.SelectOption(autoCatalogListPage.SelectIdentityNameSpan, identityName.ToUpper());
            autoCatalogListPage.CatalogRadioButton.Click();
            if (autoCatalogListPage.OriginalCatalogCheckbox.Selected != (catalogType == CatalogType.Original))
                autoCatalogListPage.OriginalCatalogCheckbox.Click();
            else if (autoCatalogListPage.DeltaCatalogCheckbox.Selected != (catalogType == CatalogType.Delta))
                autoCatalogListPage.DeltaCatalogCheckbox.Click();

            autoCatalogListPage.InstantCheckbox.Click();
            autoCatalogListPage.SelectTheStatus(UtilityMethods.ConvertToString(catalogStatus));
            foreach (CatalogItemType itemType in catalogItemType)
            {
                switch (itemType)
                {
                    case CatalogItemType.ConfigWithDefaultOptions:
                        autoCatalogListPage.STDCheckbox.Click();
                        break;
                    case CatalogItemType.SNP:
                        autoCatalogListPage.SnPCheckbox.Click();
                        break;
                    default:
                        autoCatalogListPage.SysCheckbox.Click();
                        break;
                }
            }
            autoCatalogListPage.SelectCatalogTestOrLive(catalogTestOrLive);
            autoCatalogListPage.SearchRecordsLink.Click();
            autoCatalogListPage.CatalogsTable.WaitForElementVisible(TimeSpan.FromSeconds(30));
            autoCatalogListPage.WaitForCatalogInSearchResult(anyTimeAfter.ConvertToUtcTimeZone(), catalogStatus);
        }

        public bool SearchScheduledProfiles(B2BEnvironment environment, Region region, string profileName, string identityName, string Country = null, 
            bool otherFilter = false, CatalogTestOrLive catalogTestOrLive = CatalogTestOrLive.None)
        {
            webDriver.Navigate().GoToUrl(ConfigurationManager.AppSettings["AutoCatalogListPageUrl"] + ((environment == B2BEnvironment.Production) ? "P" : "U"));
            CPTAutoCatalogInventoryListPage autoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            autoCatalogListPage.SelectOption(autoCatalogListPage.SelectRegionSpan, region.ToString().ToUpper());
            autoCatalogListPage.ScheduledCheckbox.Click(); autoCatalogListPage.CatalogRadioButton.Click();
            autoCatalogListPage.OriginalCatalogCheckbox.Click();
            if (!string.IsNullOrEmpty(Country))
                autoCatalogListPage.SelectTheCountry(Country);

            if (otherFilter)
            {
                autoCatalogListPage.CreationDateStart.SendKeys(DateTime.Now.AddDays(-15).ToString(MMDDYYYY));
                autoCatalogListPage.CreationDateEnd.SendKeys(DateTime.Now.AddDays(-12).ToString(MMDDYYYY));
            }
            autoCatalogListPage.SelectCatalogTestOrLive(catalogTestOrLive);
            autoCatalogListPage.SearchRecordsLink.Click();
            autoCatalogListPage.CatalogsTable.WaitForElementVisible(TimeSpan.FromSeconds(30));

            ICollection<IWebElement> table = autoCatalogListPage.CatalogsTable.FindElements(By.TagName("tr"));
            List<IWebElement> rows = table.ToList();
            string actualProfile = string.Empty;
            string actualIdentity = string.Empty;
            int pageNo = 1; string pageInfo = rows[0].Text;
            if (!string.IsNullOrEmpty(pageInfo))
            {
                while (Convert.ToInt32(pageInfo.Substring(pageInfo.LastIndexOf(' ') + 1)) != pageNo)
                {
                    for (int i = 1; i < table.Count - 1; i++)
                    {
                        actualIdentity = autoCatalogListPage.CatalogsTable.FindElement(By.CssSelector("tbody tr:nth-of-type(" + i + ") td:nth-of-type(" + 1 + ")")).Text;
                        actualProfile = autoCatalogListPage.CatalogsTable.FindElement(By.CssSelector("tbody tr:nth-of-type(" + i + ") td:nth-of-type(" + 2 + ")")).Text;
                        if (actualProfile.ToUpperInvariant().Equals(profileName.ToUpperInvariant()) && actualIdentity.ToUpperInvariant().Equals(identityName.ToUpperInvariant()))
                            return false;
                    }
                    autoCatalogListPage.NextButton.Click();
                    pageNo++;
                }
            }
            return true;
        }

        public bool SearchScheduledProfilesTestOrLive(B2BEnvironment environment, Region region, string profileName,
            string identityName, string country, CatalogTestOrLive catalogTestOrLive)
        {
            webDriver.Navigate().GoToUrl(ConfigurationManager.AppSettings["AutoCatalogListPageUrl"] + ((environment == B2BEnvironment.Production) ? "P" : "U"));
            CPTAutoCatalogInventoryListPage autoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            autoCatalogListPage.SelectOption(autoCatalogListPage.SelectRegionSpan, region.ToString());
            autoCatalogListPage.SelectTheCountry(country);
            autoCatalogListPage.SelectOption(autoCatalogListPage.SelectCustomerNameSpan, profileName);
            autoCatalogListPage.SelectOption(autoCatalogListPage.SelectIdentityNameSpan, identityName.ToUpper());
            autoCatalogListPage.SelectCatalogTestOrLive(catalogTestOrLive);
            autoCatalogListPage.ScheduledCheckbox.Click(); autoCatalogListPage.CatalogRadioButton.Click();
            autoCatalogListPage.SearchRecordsLink.Click();
            autoCatalogListPage.CatalogsTable.WaitForElementVisible(TimeSpan.FromSeconds(30));
            ICollection<IWebElement> table = autoCatalogListPage.CatalogsTable.FindElements(By.TagName("tr"));
            List<IWebElement> rows = table.ToList();
            string actualProfile = string.Empty;
            string actualIdentity = string.Empty;
            bool matchFlag = true;
            for (int i = 1; i < table.Count - 1; i++)
            {
                actualIdentity = autoCatalogListPage.CatalogsTable.FindElement(By.CssSelector("tbody tr:nth-of-type(" + i + ") td:nth-of-type(" + 1 + ")")).Text;
                actualProfile = autoCatalogListPage.CatalogsTable.FindElement(By.CssSelector("tbody tr:nth-of-type(" + i + ") td:nth-of-type(" + 2 + ")")).Text;
                matchFlag &= actualProfile.ToUpperInvariant().Equals(profileName.ToUpperInvariant());
                matchFlag &= actualIdentity.ToUpperInvariant().Equals(identityName.ToUpperInvariant());
            }
            return matchFlag;
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
        public bool VerifyHardcodedInvQtyLTValuesForDellBrandedSnP(CatalogItemType[] catalogItemType, CatalogType catalogType, string identityName, string filePath, DateTime anyTimeAfter)
        {
            string schemaPath = string.Empty;
            schemaPath = Path.Combine(System.Environment.CurrentDirectory, "CatalogSchema.xsd");
            string message = XMLSchemaValidator.ValidateSchema(filePath, schemaPath);
            message.Should().Be(string.Empty, "Error: One or more tags failed scehma validation. Please check the log for complete details");

            B2BXML actualcatalogXML = XMLDeserializer<B2BXML>.DeserializeFromXmlFile(filePath);
            CatalogDetails actualCatalogDetails = actualcatalogXML.BuyerCatalog.CatalogDetails;

            bool matchFlag = true;
            foreach (CatalogItemType itemType in catalogItemType)
            {
                IEnumerable<CatalogItem> actualCatalogItems = actualCatalogDetails.CatalogItem.Where(ci => ci.CatalogItemType == itemType);

                switch (itemType)
                {
                    case CatalogItemType.SNP:
                        actualCatalogItems = actualCatalogItems.Where(ci => Regex.IsMatch(ci.BaseSKUId, "^\\d{3}-"));
                        foreach (CatalogItem actualCatalogItem in actualCatalogItems)
                        {
                            matchFlag &= UtilityMethods.CompareValues<int>("LeadTime", actualCatalogItem.LeadTime, 10);
                            matchFlag &= UtilityMethods.CompareValues<int>("InventoryQty", actualCatalogItem.InventoryQty, 0);
                        }
                        break;
                    case CatalogItemType.ConfigWithDefaultOptions:
                        actualCatalogItems = actualCatalogItems.Where(ci => ci.ItemType.ToString() == "BTS");
                        foreach (CatalogItem actualCatalogItem in actualCatalogItems)
                        {
                            matchFlag &= UtilityMethods.CompareValues<int>("LeadTime", actualCatalogItem.LeadTime, 0, Computation.GreaterThanOrEqualTo);
                            matchFlag &= UtilityMethods.CompareValues<int>("InventoryQty", actualCatalogItem.InventoryQty, -9999, Computation.GreaterThanOrEqualTo);
                        }
                        break;
                    case CatalogItemType.Systems:
                        actualCatalogItems = actualCatalogItems.Where(ci => ci.ItemType.ToString() == "BTS");
                        foreach (CatalogItem actualCatalogItem in actualCatalogItems)
                        {
                            matchFlag &= UtilityMethods.CompareValues<int>("LeadTime", actualCatalogItem.LeadTime, 0, Computation.GreaterThanOrEqualTo);
                            matchFlag &= UtilityMethods.CompareValues<int>("InventoryQty", actualCatalogItem.InventoryQty, -9999, Computation.GreaterThanOrEqualTo);
                        }
                        break;
                    default:
                        break;
                }
            }
            return matchFlag;
        }

    }
}
