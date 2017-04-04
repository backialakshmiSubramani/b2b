// ***********************************************************************
// Author           : AMERICAS\Shaukat_Saleem
// Created          : 9/30/2016 4:04:12 PM
//
// Last Modified By : AMERICAS\Shaukat_Saleem
// Last Modified On : 9/30/2016 4:04:12 PM
// ***********************************************************************
// <copyright file="AutoCatalogAndInventoryListPage.cs" company="Dell">
//     Copyright (c) Dell 2016. All rights reserved.
// </copyright>
// <summary>Provide a summary of the page class here.</summary>
// ***********************************************************************
using Dell.Adept.UI.Web.Pages;
using Dell.Adept.UI.Web.Support;
using Dell.Adept.UI.Web.Support.Extensions.WebElement;
using Dell.Adept.UI.Web.Support.Locators;
using Modules.Channel.B2B.Common;
using Modules.Channel.B2B.Core.Pages;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;

namespace Modules.Channel.B2B.Core.NewPages
{
    /// <summary>
    /// This base class is the where all specific page classes will be derived.
    /// </summary>
    public class AutoCatalogAndInventoryListPage : PageBase
    {
        private readonly IWebDriver _webDriver;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public AutoCatalogAndInventoryListPage(IWebDriver webDriver) : base(ref webDriver)
        {
            _webDriver = webDriver;
        }

        private IWebElement SelectCustomerNameSpan => _webDriver.FindElement(By.XPath("//span[contains(text(),'Select Customer Profile')]"));

        private IWebElement SelectIdentityNameSpan => _webDriver.FindElement(By.XPath("//span[contains(text(),'Select Profile Identity')]"));

        private IWebElement SelectRegionSpan => _webDriver.FindElement(By.XPath("//span[contains(text(),'Select Region')]"));

        /// <summary>
        /// Delta catalog check box
        /// </summary>
        private IWebElement DeltaCatalogCheckbox => _webDriver.FindElement(AdeptBy.Attribute(ElementTag.input, "value", "Delta"));

        /// <summary>
        /// Original Catalog check box
        /// </summary>
        private IWebElement OriginalCatalogCheckbox => _webDriver.FindElement(AdeptBy.Attribute(ElementTag.input, "value", "Original"));

        /// <summary>
        /// Selects the status from the Select drop down
        /// </summary>
        /// <param name="status"></param>
        private void SelectCatalogStatus(string status)
        {
            _webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(60));
            _webDriver.FindElement(By.XPath("//div[@ng-model='CatalogStatusId']/a")).Click();
            _webDriver.FindElement(By.XPath("//div[@ng-model='CatalogStatusId']/div/div[@class='custom-select-search']/input")).SendKeys(status);
            _webDriver.FindElement(By.XPath("//div[@ng-model='CatalogStatusId']/div/ul/li/a[contains(text(),'" + status + "')]")).Click();
        }

        private IWebElement CatalogsTable => _webDriver.FindElement(By.XPath("//table[@st-table='Channel' and @st-safe-src='Catalogs']"));

        private IEnumerable<IWebElement> CatalogListTableRows => _webDriver.FindElements(By.XPath("//table[@st-safe-src='Catalogs']/tbody/tr"));

        private IWebElement PagingSpan => CatalogsTable.FindElement(By.XPath("thead/tr[1]/td"));

        /// <summary>
        /// Text box in the paging section
        /// </summary>
        private IWebElement PageNumberTextbox => _webDriver.FindElement(By.XPath("//page-select/input"));

        private IWebElement NextButton => _webDriver.FindElement(By.Id("nextUpButton"));

        private IWebElement ClearAllLink => _webDriver.FindElement(By.Id("lnkClear"));

        /// <summary>
        /// Catalog Type - Inventory Checkbox
        /// </summary>
        private IWebElement InventoryCheckbox => _webDriver.FindElement(AdeptBy.Attribute(ElementTag.input, "value", "Inventory"));

        /// <summary>
        /// Search Records hyperlink
        /// </summary>
        private IWebElement SearchRecordsLink => _webDriver.FindElement(By.Id("lnkSearch"));

        private IWebElement GetDownloadButton(int rowIndex)
        {
            return CatalogsTable.FindElement(By.CssSelector("tbody>tr:nth-of-type(" + rowIndex + ")>td[title=' download']>input[type='image']"));
        }

        /// <summary>
        /// Automated Catalog List Page
        /// </summary>
        private IWebElement AutoCatalogInventoryListPageLink => _webDriver.FindElement(By.XPath("//a[text()=' Auto Catalog & Inventory List']"));

        /// <summary>
        /// Treat this like a BVT of the page. If Validate does not pass, throw exception and console.writeline a return message into Test Class
        /// </summary>
        /// <returns>validated</returns>
        public override bool Validate()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// determines whether or not the driver is active on this page. Must be overriden with each subclass.
        /// </summary>
        /// <returns>active</returns>
        public override bool IsActive()
        {
            throw new NotImplementedException();
        }

        public void GoToAutoCatalogAndInventoryListPage(B2BEnvironment b2BEnvironment)
        {
            _webDriver.Navigate().GoToUrl(ConfigurationReader.GetValue("AutoCatalogListPageUrl") + ((b2BEnvironment == B2BEnvironment.Production) ? "P" : "U"));
        }

        public void GoToAutoCatalogAndInventoryPage()
        {
            AutoCatalogInventoryListPageLink.Click();
            PageUtility.WaitForPageRefresh(_webDriver);
            _webDriver.SwitchTo().Window(WebDriver.WindowHandles.Last());
        }

        public void SwitchToPreviousTab()
        {
            _webDriver.SwitchTo().Window(_webDriver.WindowHandles.First());
        }

        public void SwitchToNextTab()
        {
            _webDriver.SwitchTo().Window(WebDriver.WindowHandles.Last());
        }

        public void SearchCatalog(string profileName, string identityName, DateTime anyTimeAfter, CatalogStatus catalogStatus, CatalogType catalogType)
        {
            SelectOptionFromDropDown(SelectRegionSpan, "US");
            SelectOptionFromDropDown(SelectCustomerNameSpan, profileName);
            SelectOptionFromDropDown(SelectIdentityNameSpan, identityName.ToUpper());

            SelectOriginalOrDeltaCheckBox(catalogType);
            SelectCatalogStatus(catalogStatus.ConvertToString());
            SearchRecordsLink.Click();
            CatalogsTable.WaitForElementVisible(TimeSpan.FromSeconds(30));
            WaitForCatalogInSearchResult(anyTimeAfter.ConvertToUtcTimeZone(), catalogStatus);
        }

        public void RetrieveCatalogSearchResult(out string lastStatusDate, out string type, out string status)
        {
            lastStatusDate = CatalogsTable.GetCellValue(1, "Last Status Date");
            type = CatalogsTable.GetCellValue(1, "Type");
            status = CatalogsTable.GetCellValue(1, "Status");
        }

        public void ValidateInventoryFeedSearchResult(out string lastStatusDate, out string type, out string status)
        {
            lastStatusDate = CatalogsTable.GetCellValueForInventory(1, "Last Status Date");
            type = CatalogsTable.GetCellValueForInventory(1, "Type");
            status = CatalogsTable.GetCellValueForInventory(1, "Status");
        }

        public void GetInventoryFileName(out string inventoryName, out string lastStatusDate)
        {
            inventoryName = CatalogsTable.GetCellValueForInventory(1, "Catalog/Inventory Name");
            lastStatusDate = CatalogsTable.GetCellValueForInventory(1, "Last Status Date");
        }

        /// <summary>
        /// Download catalog from Auto Catalog List & Inventory page
        /// </summary>
        /// <param name="identityName">Identity Name</param>
        /// <param name="anyTimeAfter">Time after which the catalog is processed</param>
        /// <returns>File name for the downloaded catalog</returns>
        public string DownloadCatalog(string identityName, DateTime anyTimeAfter)
        {
            UtilityMethods.ClickElement(_webDriver, GetDownloadButton(1));
            string downloadPath = ConfigurationManager.AppSettings["CatalogDownloadPath"];

            _webDriver.WaitForDownLoadToComplete(downloadPath, identityName, anyTimeAfter, TimeSpan.FromMinutes(1));
            string fileName = new DirectoryInfo(downloadPath).GetFiles().AsEnumerable()
                .Where(file => file.Name.Contains(identityName.ToUpper()) && file.CreationTime > anyTimeAfter)
                .FirstOrDefault().FullName;

            return fileName;
        }

        public void SearchInventoryFeedRecords(string profileName, string identityName, Region region, DateTime anyTimeAfter, CatalogStatus catalogStatus)
        {
            ClearAllLink.Click();
            SelectOptionFromDropDown(SelectRegionSpan, "US");
            SelectOptionFromDropDown(SelectCustomerNameSpan, profileName);
            SelectOptionFromDropDown(SelectIdentityNameSpan, identityName.ToUpper());
            SelectCatalogStatus(catalogStatus.ConvertToString());
            InventoryCheckbox.Click();
            SearchRecordsLink.Click();
            CatalogsTable.WaitForElementVisible(TimeSpan.FromSeconds(30));
            WaitForInventoryInSearchResult(anyTimeAfter.ConvertToUtcTimeZone(), catalogStatus);
        }

        /// <summary>
        /// Use this method to know if the paging is enabled
        /// </summary>
        /// <returns></returns>
        public bool IsPagingEnabled()
        {
            try
            {
                const string checktext = "Page of";
                var text = PagingSpan.Text;
                if (string.IsNullOrEmpty(PagingSpan.Text) || !text.Contains(checktext))
                {
                    Console.WriteLine("Paging not enabled");
                    return false;
                }
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("Paging not enabled");
                return false;
            }

            return true;
        }

        public void DisplayInventoryFeedRecords(string profileName, string identityName, CatalogStatus catalogStatus)
        {
            ClearAllLink.Click();
            SelectOptionFromDropDown(SelectRegionSpan, "US");
            SelectOptionFromDropDown(SelectCustomerNameSpan, profileName);
            SelectOptionFromDropDown(SelectIdentityNameSpan, identityName.ToUpper());
            SelectCatalogStatus(catalogStatus.ConvertToString());
            InventoryCheckbox.Click();
            SearchRecordsLink.Click();
            CatalogsTable.WaitForElementVisible(TimeSpan.FromSeconds(30));
        }

        public List<string> GetInventoryFeeds()
        {
            // ToDo: filter out feeds based on refresh interval, for last 24 hrs if it is less than 12hrs, else latest 2 feeds
            return CatalogListTableRows.Select(row => row.FindElements(By.TagName("td"))[6].Text).ToList();
        }

        public void GoToNextPage()
        {
            NextButton.Click();
        }

        public string GetPageNumberFromTextbox()
        {
            return PageNumberTextbox.GetAttribute("value");
        }

        public string GetPagingSpan()
        {
            return PagingSpan.Text.Split(' ').Last();
        }

        #region Private Methods

        private void SelectOptionFromDropDown(IWebElement webElement, string optionText)
        {
            var xPath = "../following-sibling::div/child::ul/child::li/a[(text()='" + optionText + "')]";
            UtilityMethods.SelectOptionFromDDL(_webDriver, webElement, xPath);
        }

        private void WaitForCatalogInSearchResult(DateTime createdTime, CatalogStatus catalogStatus)
        {
            var timeOutInSecs = CatalogTimeOuts.CatalogSearchTimeOut.TotalSeconds;
            var status = catalogStatus;

            while (timeOutInSecs > 0)
            {
                var lastStatusDate = Convert.ToDateTime(CatalogsTable.GetCellValue(1, "Last Status Date"), System.Globalization.CultureInfo.InvariantCulture);
                if (CatalogsTable.GetCellValue(1, "Status") != null)
                    status = CatalogsTable.GetCellValue(1, "Status").ConvertToEnum<CatalogStatus>();

                if (lastStatusDate.AddMinutes(1) > createdTime && (status == catalogStatus || status == CatalogStatus.Failed || status == CatalogStatus.FailedInstant))
                    break;
                SearchRecordsLink.Click();
                Thread.Sleep(TimeSpan.FromSeconds(5));
                timeOutInSecs -= 5;
            }
        }

        private void WaitForInventoryInSearchResult(DateTime createdTime, CatalogStatus catalogStatus)
        {
            var timeOutInSecs = CatalogTimeOuts.CatalogSearchTimeOut.TotalSeconds;
            var status = catalogStatus;

            while (timeOutInSecs > 0)
            {
                var lastStatusDate = Convert.ToDateTime(CatalogsTable.GetCellValueForInventory(1, "Last Status Date"), System.Globalization.CultureInfo.InvariantCulture);
                if (CatalogsTable.GetCellValueForInventory(1, "Status") != null)
                    status = CatalogsTable.GetCellValueForInventory(1, "Status").ConvertToEnum<CatalogStatus>();

                if (lastStatusDate.AddMinutes(1) > createdTime && (status == catalogStatus || status == CatalogStatus.Failed || status == CatalogStatus.FailedInstant))
                    break;
                SearchRecordsLink.Click();
                Thread.Sleep(TimeSpan.FromSeconds(5));
                timeOutInSecs -= 5;
            }
        }

        private void SelectOriginalOrDeltaCheckBox(CatalogType catalogType)
        {
            if (OriginalCatalogCheckbox.Selected != (catalogType == CatalogType.Original))
                OriginalCatalogCheckbox.Click();
            else if (DeltaCatalogCheckbox.Selected != (catalogType == CatalogType.Delta))
                DeltaCatalogCheckbox.Click();
        }

        #endregion Private Methods
    }
}
