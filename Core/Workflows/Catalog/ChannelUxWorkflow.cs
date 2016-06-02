using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Dell.Adept.UI.Web.Support.Extensions.WebElement;
using FluentAssertions;
using Modules.Channel.B2B.Core.Pages;
using OpenQA.Selenium;
using Modules.Channel.B2B.Common;
using System.IO;
using System.Configuration;
using CatalogTests.Common.CatalogXMLTemplates;
using Modules.Channel.Utilities;
using Microsoft.Exchange.WebServices.Data;
using Modules.Channel.B2B.CatalogXMLTemplates;
using Modules.Channel.B2B.Common;

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
        public bool ValidateCatalogXML(CatalogItemType[] catalogItemType, CatalogType catalogType, string identityName, string filePath, DateTime anyTimeAfter, ConfigRules configRules, DefaultOptions defaultOptions)
        {
            string schemaPath = Path.Combine(System.Environment.CurrentDirectory, "CatalogSchema.xsd");

            string message = XMLSchemaValidator.ValidateSchema(filePath, schemaPath);
            message.Should().Be(string.Empty, "Error: One or more tags failed scehma validation. Please check the log for complete details");

            B2BXML actualcatalogXML = XMLDeserializer<B2BXML>.DeserializeFromXmlFile(filePath);
            CatalogDetails actualCatalogDetails = actualcatalogXML.BuyerCatalog.CatalogDetails;
            CatalogHeader actualCatalogHeader = actualcatalogXML.BuyerCatalog.CatalogHeader;

            string expectedCatalogFilePath = string.Empty;

            expectedCatalogFilePath = catalogType == CatalogType.Original
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
                IEnumerable<CatalogItem> expectedCatalogItems =
                        expectedCatalogDetails.CatalogItem.Where(ci => ci.CatalogItemType == itemType);

                if (itemType.Equals(CatalogItemType.ConfigWithDefaultOptions) ||
                    itemType.Equals(CatalogItemType.ConfigWithUpsellDownsell) ||
                    itemType.Equals(CatalogItemType.Systems))
                {
                    switch (configRules)
                    {
                        case ConfigRules.DuplicateBPN:
                            expectedCatalogItems = expectedCatalogItems.Where(ci => ci.ShortName.StartsWith("Duplicate BPN"));
                            break;
                        case ConfigRules.NullBPN:
                            expectedCatalogItems = expectedCatalogItems.Where(ci => ci.ShortName.StartsWith("Null BPN"));
                            break;
                        case ConfigRules.LeadTimeOff:
                            expectedCatalogItems =
                              expectedCatalogDetails.CatalogItem.Where(ci => ci.ShortName.StartsWith("Lead Time"));
                            break;
                        case ConfigRules.LeadTimeON:
                            expectedCatalogItems =
                              expectedCatalogDetails.CatalogItem.Where(ci => ci.ShortName.StartsWith("Lead Time") && (ci.ItemType.Equals("") ||
                               ci.ItemType.Equals("BTO") || (ci.ItemType.Equals("BTS") && ci.LeadTime < 3)));
                            break;
                        default:
                            string matchingOrderCode = actualCatalogItems.Select(ci => ci.ItemOrderCode).ToList().Intersect(expectedCatalogItems.Select(ci => ci.ItemOrderCode).ToList()).First();
                            actualCatalogItems = actualCatalogItems.Where(ci => ci.ItemOrderCode == matchingOrderCode);
                            expectedCatalogItems = expectedCatalogItems.Where(ci => ci.ItemOrderCode == matchingOrderCode);

                            //expectedCatalogItems = expectedCatalogItems.Where(ci => ci.ShortName.StartsWith("STD Config"));
                            break;
                    }

                }

                else if(itemType.Equals(CatalogItemType.SNP))
                {
                    string matchingOrderCode = actualCatalogItems.Select(ci => ci.BaseSKUId).ToList().Intersect(expectedCatalogItems.Select(ci => ci.BaseSKUId).ToList()).First();
                    actualCatalogItems = actualCatalogItems.Where(ci => ci.BaseSKUId == matchingOrderCode);
                    expectedCatalogItems = expectedCatalogItems.Where(ci => ci.BaseSKUId == matchingOrderCode);
                }

                matchFlag &= ValidateCatalogItems(actualCatalogItems, expectedCatalogItems, defaultOptions);

                //itemCount += expectedCatalogItems.Count();
            }

            matchFlag &= ValidateCatalogHeader(actualCatalogHeader, expectedCatalogHeader, catalogItemType, identityName, catalogName);
            return matchFlag;
        }

        /// <summary>
        /// Validate all the tags of Catalog Header
        /// </summary>
        /// <param name="actualCatalog"></param>
        /// <param name="expectedCatalog"></param>
        /// <param name="catalogItemType"></param>
        /// <param name="identityName"></param>
        /// <param name="catalogName"></param>
        /// <returns></returns>
        public bool ValidateCatalogHeader(CatalogHeader actualCatalogHeader, CatalogHeader expectedCatalogHeader, CatalogItemType[] catalogItemType, string identityName, string catalogName)
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
        public bool ValidateCatalogItems(IEnumerable<CatalogItem> actualCatalogItems, IEnumerable<CatalogItem> expectedCatalogItems, DefaultOptions defaultOptions)
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
                }
                else
                {
                    expectedCatalogItem = expectedCatalogItems.Where(ci => ci.ItemOrderCode == actualCatalogItem.ItemOrderCode).FirstOrDefault();
                    matchFlag &= !(actualCatalogItem.PartId != "BHC:" + actualCatalogItem.QuoteId);
                }
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
                //matchFlag &= UtilityMethods.CompareValues<string>("ItemType", actualCatalogItem.ItemType, expectedCatalogItem.ItemType);
                //matchFlag &= UtilityMethods.CompareValues<string>("ItemSKUinfo", actualCatalogItem.ItemSKUinfo, expectedCatalogItem.ItemSKUinfo);
                matchFlag &= UtilityMethods.CompareValues<string>("FGAModNumber", actualCatalogItem.FGAModNumber, expectedCatalogItem.FGAModNumber);
                matchFlag &= UtilityMethods.CompareValues<int>("InventoryQty", actualCatalogItem.InventoryQty, expectedCatalogItem.InventoryQty, Computation.GreaterThanOrEqualTo);
                matchFlag &= UtilityMethods.CompareValues<string>("ListPrice", actualCatalogItem.ListPrice, expectedCatalogItem.ListPrice);
                matchFlag &= UtilityMethods.CompareValues<string>("UPC", actualCatalogItem.UPC, expectedCatalogItem.UPC);
                matchFlag &= UtilityMethods.CompareValues<string>("ManufacturerCode", actualCatalogItem.ManufacturerCode, expectedCatalogItem.ManufacturerCode);
                matchFlag &= UtilityMethods.CompareValues<string>("VPNReplacement", actualCatalogItem.VPNReplacement, expectedCatalogItem.VPNReplacement);
                matchFlag &= UtilityMethods.CompareValues<string>("VPNEOLDate", actualCatalogItem.VPNEOLDate, expectedCatalogItem.VPNEOLDate);
                matchFlag &= UtilityMethods.CompareValues<int>("PackageLength", actualCatalogItem.PackageLength, expectedCatalogItem.PackageLength);
                matchFlag &= UtilityMethods.CompareValues<int>("PackageWidth", actualCatalogItem.PackageWidth, expectedCatalogItem.PackageWidth);
                matchFlag &= UtilityMethods.CompareValues<int>("PackageHeight", actualCatalogItem.PackageHeight, expectedCatalogItem.PackageHeight);
                matchFlag &= UtilityMethods.CompareValues<int>("PalletLength", actualCatalogItem.PalletLength, expectedCatalogItem.PalletLength);
                matchFlag &= UtilityMethods.CompareValues<int>("PalletWidth", actualCatalogItem.PalletWidth, expectedCatalogItem.PalletWidth);
                matchFlag &= UtilityMethods.CompareValues<int>("PalletHeight", actualCatalogItem.PalletHeight, expectedCatalogItem.PalletHeight);
                matchFlag &= UtilityMethods.CompareValues<int>("PalletUnitsPerLayer", actualCatalogItem.PalletUnitsPerLayer, expectedCatalogItem.PalletUnitsPerLayer);
                matchFlag &= UtilityMethods.CompareValues<int>("PalletLayerPerPallet", actualCatalogItem.PalletLayerPerPallet, expectedCatalogItem.PalletLayerPerPallet);
                matchFlag &= UtilityMethods.CompareValues<int>("PalletUnitsPerPallet", actualCatalogItem.PalletUnitsPerPallet, expectedCatalogItem.PalletUnitsPerPallet);

                Console.WriteLine("PartId: " + actualCatalogItem.PartId);
                Console.WriteLine("QuoteId: " + actualCatalogItem.QuoteId);

                // Part and Quote Id
                matchFlag &= !(String.IsNullOrEmpty(actualCatalogItem.PartId));
                //Console.WriteLine("PartId is empty");
                matchFlag &= !(String.IsNullOrEmpty(actualCatalogItem.QuoteId));
                //Console.WriteLine("PartId is empty");

                // Modules
                if (defaultOptions == DefaultOptions.On)
                {
                    actualCatalogItem.Modules.Module.Count()
                        .Should()
                        .Be(expectedCatalogItem.Modules.Module.Count(),
                            "ERROR: Module count mismatch for Order code: " + actualCatalogItem.ItemOrderCode);

                    foreach (CatalogItemModule actualmodule in actualCatalogItem.Modules.Module)
                    {
                        CatalogItemModule expectedModule =
                            expectedCatalogItem.Modules.Module.Where(em => em.ModuleId == actualmodule.ModuleId)
                                .FirstOrDefault();

                        actualmodule.Options.Option.Count()
                            .Should()
                            .Be(expectedModule.Options.Option.Count(),
                                "ERROR: Option count mismatch for Module ID: " + actualmodule.ModuleId.ToString());

                        foreach (CatalogItemOption actualOption in actualmodule.Options.Option)
                        {
                            CatalogItemOption expectedOption =
                                expectedModule.Options.Option.Where(eo => eo.OptionDesc == actualOption.OptionDesc)
                                    .FirstOrDefault();

                            actualOption.OptionSkuList.OptionSku.Count()
                                .Should()
                                .Be(expectedOption.OptionSkuList.OptionSku.Count(),
                                    "ERROR: Option SKU count mismtach for Option Desc: " + actualOption.OptionDesc);

                            foreach (OptionSku actualOptionSku in actualOption.OptionSkuList.OptionSku)
                            {
                                OptionSku expectedOptionSku =
                                    expectedOption.OptionSkuList.OptionSku.Where(eo => eo.SkuId == actualOptionSku.SkuId)
                                        .FirstOrDefault();

                                matchFlag &=
                                    UtilityMethods.CompareValues<string>(
                                        "Modules.Module.Options.Option.OptionSkuList.OptionSku.SkuId",
                                        actualOptionSku.SkuId, expectedOptionSku.SkuId);
                                matchFlag &=
                                    UtilityMethods.CompareValues<string>(
                                        "Modules.Module.Options.Option.OptionSkuList.OptionSku.SkuDescription",
                                        actualOptionSku.SkuDescription, expectedOptionSku.SkuDescription);
                                matchFlag &=
                                    UtilityMethods.CompareValues<string>(
                                        "Modules.Module.Options.Option.OptionSkuList.OptionSku.OptionId",
                                        actualOptionSku.OptionId, expectedOptionSku.OptionId);
                                matchFlag &=
                                    UtilityMethods.CompareValues<decimal>(
                                        "Modules.Module.Options.Option.OptionSkuList.OptionSku.SkuPrice",
                                        actualOptionSku.SkuPrice, expectedOptionSku.SkuPrice);
                            }


                            matchFlag &= UtilityMethods.CompareValues<string>("Modules.Module.Options.Option.OptionId",
                            actualOption.OptionId.Substring(actualOption.OptionId.IndexOf("/", System.StringComparison.Ordinal)),
                            expectedOption.OptionId.Substring(expectedOption.OptionId.IndexOf("/", System.StringComparison.Ordinal)));

                            matchFlag &= UtilityMethods.CompareValues<string>(
                                "Modules.Module.Options.Option.OptionDesc", actualOption.OptionDesc,
                                expectedOption.OptionDesc);
                            matchFlag &= UtilityMethods.CompareValues<bool>("Modules.Module.Options.Option.Selected",
                                actualOption.Selected, expectedOption.Selected);
                            matchFlag &=
                                UtilityMethods.CompareValues<decimal>("Modules.Module.Options.Option.DeltaPrice",
                                    actualOption.DeltaPrice, expectedOption.DeltaPrice);
                            matchFlag &=
                                UtilityMethods.CompareValues<decimal>("Modules.Module.Options.Option.FinalPrice",
                                    actualOption.FinalPrice, expectedOption.FinalPrice);
                        }
                        matchFlag &= UtilityMethods.CompareValues<int>("Modules.Module.ModuleId", actualmodule.ModuleId,
                            expectedModule.ModuleId);
                        matchFlag &= UtilityMethods.CompareValues<string>("Modules.Module.ModuleDesc",
                            actualmodule.ModuleDesc, expectedModule.ModuleDesc);
                        matchFlag &= UtilityMethods.CompareValues<bool>("Modules.Module.Required", actualmodule.Required,
                            expectedModule.Required);
                        matchFlag &= UtilityMethods.CompareValues<bool>("Modules.Module.MultiSelect",
                            actualmodule.MultiSelect, expectedModule.MultiSelect);
                        matchFlag &= UtilityMethods.CompareValues<string>("Modules.Module.DefaultOptionId",
                            actualmodule.DefaultOptionId, expectedModule.DefaultOptionId);
                        matchFlag &= UtilityMethods.CompareValues<decimal>("Modules.Module.DefaultOptionPrice",
                            actualmodule.DefaultOptionPrice, expectedModule.DefaultOptionPrice);
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
            }

            return matchFlag;
        }

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
                b2BChannelUx.ProductionEnvRadioButton.Click();
            else if (b2BEnvironment == B2BEnvironment.Preview)
                b2BChannelUx.PreviewEnvRadioButton.Click();

            b2BChannelUx.SelectOption(b2BChannelUx.SelectCustomerProfileDiv, profileName);
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

            b2BChannelUx.WaitForFeedBackMessage(TimeSpan.FromMinutes(1));
            b2BChannelUx.FeedBackMessage.Text.ShouldBeEquivalentTo("Auto Catalog generation successfully initiated. Please check it on the Auto Catalog & Inventory List page after sometime.");
        }

        internal void ValidateP2PMessage(B2BEnvironment environment, string profileName, string identityName, CatalogType catalogType)
        {
            webDriver.Navigate().GoToUrl(ConfigurationManager.AppSettings["TestHarnessPageUrl"] + ((environment == B2BEnvironment.Production) ? "P" : "U"));
            B2BChannelUx b2BChannelUx = new B2BChannelUx(webDriver);

            if (environment == B2BEnvironment.Production)
                b2BChannelUx.ProductionEnvRadioButton.Click();
            else if (environment == B2BEnvironment.Preview)
                b2BChannelUx.PreviewEnvRadioButton.Click();

            b2BChannelUx.SelectOption(b2BChannelUx.SelectCustomerProfileDiv, profileName);
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

        internal void ValidateErrorMessageNoConfigSelected(B2BEnvironment environment, string profileName, string identityName, CatalogType catalogType)
        {
            webDriver.Navigate().GoToUrl(ConfigurationManager.AppSettings["TestHarnessPageUrl"] + ((environment == B2BEnvironment.Production) ? "P" : "U"));
            B2BChannelUx b2BChannelUx = new B2BChannelUx(webDriver);

            if (environment == B2BEnvironment.Production)
                b2BChannelUx.ProductionEnvRadioButton.Click();
            else if (environment == B2BEnvironment.Preview)
                b2BChannelUx.PreviewEnvRadioButton.Click();

            b2BChannelUx.SelectOption(b2BChannelUx.SelectCustomerProfileDiv, profileName);
            b2BChannelUx.SelectOption(b2BChannelUx.SelectProfileIdentityDiv, identityName.ToUpper());
            b2BChannelUx.SetNewRadioButton.Click();
            
            if (b2BChannelUx.STDSetNewCheckBox.Selected)
                b2BChannelUx.STDSetNewCheckBox.Click();

            if (catalogType == CatalogType.Original)
                b2BChannelUx.OriginalRadioButton.Click();
            else if (catalogType == CatalogType.Delta)
                b2BChannelUx.DeltaRadioButton.Click();
            b2BChannelUx.CreateAndDownloadButton.Click();
            
            WaitForPageRefresh();

            Console.WriteLine("Actual: " + b2BChannelUx.FeedBackMessage.Text);
            b2BChannelUx.FeedBackMessage.Text.Trim().ShouldBeEquivalentTo("Minimum one configuration is required for the catalog creation.");
        }

        internal void ValidateErrorMessageWhileCreatingDeltaCatalog(B2BEnvironment environment, string profileName, string identityName, CatalogType catalogType)
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

            b2BChannelUx.CreateButton.Click();

            WaitForPageRefresh();

            Console.WriteLine("Actual: " + b2BChannelUx.FeedBackMessage.Text);
            b2BChannelUx.FeedBackMessage.Text.Trim().ShouldBeEquivalentTo("Original Catalog doesn't exist, hence, Delta can not be generated.");
        }

        /// <summary>
        /// Search for a catalog in Auto Catalog List & Inventory page 
        /// </summary>
        /// <param name="profileName">Profile name</param>
        /// <param name="identityName">Identity name</param>
        /// <param name="anyTimeAfter">Time after which the catalog is processed</param>
        /// <param name="operation">Catalog operation i.e. 'Create' or 'Create & Publish'</param>
        public void SearchCatalog(string profileName, string identityName, DateTime anyTimeAfter, CatalogStatus catalogStatus)
        {
            CPTAutoCatalogInventoryListPage autoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            autoCatalogListPage.SelectOption(autoCatalogListPage.SelectRegionSpan, "US");
            autoCatalogListPage.SelectOption(autoCatalogListPage.SelectCustomerNameSpan, profileName);
            autoCatalogListPage.SelectOption(autoCatalogListPage.SelectIdentityNameSpan, identityName.ToUpper());
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
        public void SearchCatalog(string profileName, string identityName, DateTime anyTimeAfter, CatalogStatus catalogStatus, CatalogType catalogType)
        {
            CPTAutoCatalogInventoryListPage autoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            autoCatalogListPage.SelectOption(autoCatalogListPage.SelectRegionSpan, "US");
            autoCatalogListPage.SelectOption(autoCatalogListPage.SelectCustomerNameSpan, profileName);
            autoCatalogListPage.SelectOption(autoCatalogListPage.SelectIdentityNameSpan, identityName.ToUpper());

            if (autoCatalogListPage.OriginalCatalogCheckbox.Selected != (catalogType == CatalogType.Original))
                autoCatalogListPage.OriginalCatalogCheckbox.Click();
            else if (autoCatalogListPage.DeltaCatalogCheckbox.Selected != (catalogType == CatalogType.Delta))
                autoCatalogListPage.DeltaCatalogCheckbox.Click();

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
            autoCatalogListPage.CatalogsTable.GetCellValue(1, "Status").Should().Be(catalogStatus.ConvertToString(), "Catalog status is not as expected");
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

            autoCatalogListPage.GetDownloadButton(1).Click();
            string downloadPath = ConfigurationManager.AppSettings["CatalogDownloadPath"];

            webDriver.WaitForDownLoadToComplete(downloadPath, identityName, anyTimeAfter, TimeSpan.FromMinutes(1));
            string fileName = new DirectoryInfo(downloadPath).GetFiles().AsEnumerable()
                .Where(file => file.Name.Contains(identityName.ToUpper()) && file.CreationTime > anyTimeAfter)
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
            b2BHomePage.CRAssociationListLink.Click();

            B2BCrossReferenceAssociationPage b2BCrossReferenceAssociationPage = new B2BCrossReferenceAssociationPage(webDriver);
            b2BCrossReferenceAssociationPage.OpenCrossReferenceListPage(b2BEnvironment);
            b2BCrossReferenceAssociationPage.AccountName.SelectByValue(profileName);
            b2BCrossReferenceAssociationPage.Search.Click();
            b2BCrossReferenceAssociationPage.OpenCRTXML(profileName);

            IReadOnlyCollection<string> windowHandles = webDriver.WindowHandles;

            webDriver.SwitchTo().Window(webDriver.WindowHandles.Last());
            string crtXMLText = webDriver.FindElement(By.CssSelector("div[class='pretty-print']")).Text;

            B2BXML CatalogXML = XMLDeserializer<B2BXML>.DeserializeFromXmlFile(catalogXMLFilePath);
            CRTXML CrtXML = XMLDeserializer<CRTXML>.DeserializeFromXmlString(crtXMLText);

            List<CatalogItem> validCRTCatalogItems = CatalogXML.BuyerCatalog.CatalogDetails.CatalogItem.Where(ci => ((ci.CatalogItemType == CatalogItemType.ConfigWithDefaultOptions || ci.CatalogItemType == CatalogItemType.ConfigWithUpsellDownsell || ci.CatalogItemType == CatalogItemType.Systems)
                && !string.IsNullOrEmpty(ci.ManufacturerPartNumber)) || (ci.CatalogItemType == CatalogItemType.SNP && !string.IsNullOrEmpty(ci.BaseSKUId))).ToList();

            CrtXML.CrossReference.CRTValues.CRTValue.Count().Should().Be(validCRTCatalogItems.Count, "CRT XML Count does not match with catalog item count");

            bool matchflag = true;

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
                matchflag &= UtilityMethods.CompareValues<string>("Price", CrtXML.CrossReference.CRTValues.CRTValue.Where(crt => crt.Id == catalogItem.ManufacturerPartNumber).First().Item.Where(item => item.Id == "price").First().Data, catalogItem.ListPrice);
                matchflag &= UtilityMethods.CompareValues<string>("Buyer Code Type", CrtXML.CrossReference.CRTValues.CRTValue.Where(crt => crt.Id == catalogItem.ManufacturerPartNumber).First().Item.Where(item => item.Id == "buyer_code_type").First().Data, "B2B Quote");
            }

            return matchflag;
        }
    }
}
