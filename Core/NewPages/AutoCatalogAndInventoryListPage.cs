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
using Dell.Adept.UI.Web.Support.Extensions.WebDriver;
using Dell.Adept.UI.Web.Support.Extensions.WebElement;
using Dell.Adept.UI.Web.Support.Locators;
using Modules.Channel.B2B.Common;
using OpenQA.Selenium;
using System;
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
        IWebDriver webDriver;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public AutoCatalogAndInventoryListPage(IWebDriver webDriver) : base(ref webDriver)
        {
            this.webDriver = webDriver;
            //populate the following variables with the appropriate value
            //Name = "";
            //Url = "";
            //ProductUnit = "";

        }

        public IWebElement SelectCustomerNameSpan
        {
            get
            {
                return webDriver.FindElement(By.XPath("//span[contains(text(),'Select Customer Profile')]"));
            }
        }

        public IWebElement SelectIdentityNameSpan
        {
            get
            {
                return webDriver.FindElement(By.XPath("//span[contains(text(),'Select Profile Identity')]"));
            }
        }

        public IWebElement SelectRegionSpan
        {
            get
            {
                return webDriver.FindElement(By.XPath("//span[contains(text(),'Select Region')]"));
            }
        }

        /// <summary>
        /// Delta catalog check box
        /// </summary>
        public IWebElement DeltaCatalogCheckbox
        {
            get { return webDriver.FindElement(AdeptBy.Attribute(ElementTag.input, "value", "Delta")); }
        }

        /// <summary>
        /// Original Catalog check box
        /// </summary>
        public IWebElement OriginalCatalogCheckbox
        {
            get
            {
                return webDriver.FindElement(AdeptBy.Attribute(ElementTag.input, "value", "Original"));
            }
        }
        
        /// <summary>
        /// Selects the status from the Select drop down
        /// </summary>
        /// <param name="status"></param>
        public void SelectCatalogStatus(string status)
        {
            webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(60));
            webDriver.FindElement(By.XPath("//div[@ng-model='CatalogStatusId']/a")).Click();
            webDriver.FindElement(By.XPath("//div[@ng-model='CatalogStatusId']/div/div[@class='custom-select-search']/input")).SendKeys(status);
            webDriver.FindElement(By.XPath("//div[@ng-model='CatalogStatusId']/div/ul/li/a[contains(text(),'" + status + "')]")).Click();
        }

        public IWebElement CatalogsTable
        {
            get
            {
                return webDriver.FindElement(By.XPath("//table[@st-table='Channel' and @st-safe-src='Catalogs']"));
            }
        }

        /// <summary>
        /// Search Records hyperlink
        /// </summary>
        public IWebElement SearchRecordsLink
        {
            get { return webDriver.FindElement(By.Id("lnkSearch")); }
        }

        public IWebElement GetDownloadButton(int rowIndex)
        {
            return CatalogsTable.FindElement(By.CssSelector("tbody>tr:nth-of-type(" + rowIndex + ")>td[title=' download']>input[type='image']"));
        }

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
            webDriver.Navigate().GoToUrl(ConfigurationReader.GetValue("AutoCatalogListPageUrl") + ((b2BEnvironment == B2BEnvironment.Production) ? "P" : "U"));
        }

        public void SearchCatalog(string profileName, string identityName, DateTime anyTimeAfter, CatalogStatus catalogStatus, CatalogType catalogType)
        {
            SelectOptionFromDropDown(SelectRegionSpan, "US");
            SelectOptionFromDropDown(SelectCustomerNameSpan, profileName);
            SelectOptionFromDropDown(SelectIdentityNameSpan, identityName.ToUpper());

            SelectOriginalOrDeltaCheckBox(catalogType);          
            SelectCatalogStatus(UtilityMethods.ConvertToString(catalogStatus));
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

        /// <summary>
        /// Download catalog from Auto Catalog List & Inventory page
        /// </summary>
        /// <param name="identityName">Identity Name</param>
        /// <param name="anyTimeAfter">Time after which the catalog is processed</param>
        /// <returns>File name for the downloaded catalog</returns>
        public string DownloadCatalog(string identityName, DateTime anyTimeAfter)
        {
            UtilityMethods.ClickElement(webDriver, GetDownloadButton(1));
            string downloadPath = ConfigurationManager.AppSettings["CatalogDownloadPath"];

            webDriver.WaitForDownLoadToComplete(downloadPath, identityName, anyTimeAfter, TimeSpan.FromMinutes(1));
            string fileName = new DirectoryInfo(downloadPath).GetFiles().AsEnumerable()
                .Where(file => file.Name.Contains(identityName.ToUpper()) && file.CreationTime > anyTimeAfter)
                .FirstOrDefault().FullName;

            return fileName;
        }

        private void SelectOptionFromDropDown(IWebElement webElement, string optionText)
        {
            string xPath = "../following-sibling::div/child::ul/child::li/a[(text()='" + optionText + "')]";
            UtilityMethods.SelectOptionFromDDL(webDriver, webElement, xPath);
        }

        private void WaitForCatalogInSearchResult(DateTime createdTime, CatalogStatus catalogStatus)
        {
            DateTime lastStatusDate;
            double timeOutInSecs = CatalogTimeOuts.CatalogSearchTimeOut.TotalSeconds;
            CatalogStatus status = catalogStatus;

            while (timeOutInSecs > 0)
            {
                lastStatusDate = Convert.ToDateTime(CatalogsTable.GetCellValue(1, "Last Status Date"), System.Globalization.CultureInfo.InvariantCulture);
                if (CatalogsTable.GetCellValue(1, "Status") != null)
                    //status = (CatalogStatus)Enum.Parse(typeof(CatalogStatus), CatalogsTable.GetCellValue(1, "Status"));
                    status = UtilityMethods.ConvertToEnum<CatalogStatus>(CatalogsTable.GetCellValue(1, "Status"));


                //if (lastStatusDate.AddMinutes(1) > createdTime && (status == catalogStatus || status == CatalogStatus.Failed))
                if (lastStatusDate.AddMinutes(1) > createdTime && (status == catalogStatus || status == CatalogStatus.Failed || status == CatalogStatus.FailedInstant))
                    break;
                else
                {
                    SearchRecordsLink.Click();
                    Thread.Sleep(TimeSpan.FromSeconds(5));
                    timeOutInSecs -= 5;
                }
            }
        }

        private void SelectOriginalOrDeltaCheckBox(CatalogType catalogType)
        {
            if (OriginalCatalogCheckbox.Selected != (catalogType == CatalogType.Original))
                OriginalCatalogCheckbox.Click();
            else if (DeltaCatalogCheckbox.Selected != (catalogType == CatalogType.Delta))
                DeltaCatalogCheckbox.Click();
        }
    }
}
