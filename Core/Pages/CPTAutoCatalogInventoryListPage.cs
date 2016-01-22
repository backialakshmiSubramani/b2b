// ***********************************************************************
// Author           : AMERICAS\Nethra_Pandappilav
// Created          : 4/21/2015 1:40:52 PM
//
// Last Modified By : AMERICAS\Nethra_Pandappilav
// Last Modified On : 4/21/2015 1:40:52 PM
// ***********************************************************************
// <copyright file="B2BAutoCatalogListPage.cs" company="Dell">
//     Copyright (c) Dell 2015. All rights reserved.
// </copyright>
// <summary>Provide a summary of the page class here.</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using OpenQA.Selenium;
using Dell.Adept.Core;
using Dell.Adept.UI.Web.Pages;
using Dell.Adept.UI.Web.Support;
using Dell.Adept.UI.Web.Support.Extensions.WebDriver;
using Dell.Adept.UI.Web.Support.Extensions.WebElement;
using Dell.Adept.UI.Web.Support.Locators;
using Modules.Channel.B2B.Common;
using Modules.Channel.EUDC.Core.Pages;

namespace Modules.Channel.B2B.Core.Pages
{
    /// <summary>
    /// This base class is the where all specific page classes will be derived.
    /// </summary>
    public class CPTAutoCatalogInventoryListPage : PageBase
    {
        IWebDriver webDriver;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public CPTAutoCatalogInventoryListPage(IWebDriver webDriver)
            : base(ref webDriver)
        {
            this.webDriver = webDriver;
            //populate the following variables with the appropriate value
            Name = "Channel Catalog List Page";
            Url = webDriver.Url;
            ProductUnit = "Channel";
            this.webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(30));
            this.webDriver.WaitForPageLoad(TimeSpan.FromSeconds(30));
        }

        /// <summary>
        /// Treat this like a BVT of the page. If Validate does not pass, throw exception and console.writeline a return message into Test Class
        /// </summary>
        /// <returns>validated</returns>
        public override bool Validate()
        {
            return PageHeader.Displayed;
        }

        /// <summary>
        /// determines whether or not the driver is active on this page. Must be overriden with each subclass.
        /// </summary>
        /// <returns>active</returns>
        public override bool IsActive()
        {
            return webDriver.Url.ToLower().Contains("autocataloglistpage.aspx");
        }

        #region Elements

        /// <summary>
        /// Auto Catalog List Page Header
        /// </summary>
        public IWebElement PageHeader
        {
            get { return webDriver.FindElement(By.Id("lblPageTitle")); }
        }

        /// <summary>
        /// Dropdown menu at the top right corner of the page
        /// </summary>
        public IWebElement DropdownMenuLink
        {
            get { return webDriver.FindElement(By.ClassName("btn-group")); }
        }

        /// <summary>
        /// List of links in the dropdown menu at the top right corner of the page
        /// </summary>
        public ReadOnlyCollection<IWebElement> DropdownMenuItems
        {
            get
            {
                return DropdownMenuLink.FindElements(By.XPath("ul/li/a"));
            }
        }

        /// <summary>
        /// List of Status in the upper right corner of the page
        /// </summary>
        public ReadOnlyCollection<IWebElement> StatusTable
        {
            get { return webDriver.FindElements(By.XPath("//form/table[1]/tbody/tr[1]/td[3]/table/tbody/tr[td]")); }

            //get 
            //{ 
            //    return webDriver.FindElements(By.XPath(".//*[@id='myForm']/table/tbody/tr/td[3]/table/tbody")); 
            //}
        }

        /// <summary>
        /// Customer Drop down
        /// </summary>
        public IWebElement SelectCustomer
        {
            get
            {
                return
                    webDriver.FindElement(
                        By.XPath("//select[@ng-model='customer']"));

            }
        }

        /// <summary>
        /// Identities Drop down
        /// </summary>
        public IWebElement SelectIdentity
        {
            get
            {
                return
                    webDriver.FindElement(
                        By.XPath("//select[@ng-model='Identity']"));
            }
        }

        /// <summary>
        /// Region Drop down
        /// </summary>
        public IWebElement SelectRegion
        {
            get
            {
                return
                    webDriver.FindElement(
                        By.XPath("//select[@ng-model='region']"));

            }
        }

        /// <summary>
        /// Country Drop down
        /// </summary>
        public IWebElement SelectCountry
        {
            get
            {
                return
                    webDriver.FindElement(
                        By.XPath("//select[@ng-model='country']"));
            }
        }


        /// <summary>
        /// Select status Drop down
        /// </summary>
        public IWebElement SelectStatus
        {
            get
            {
                return
                    webDriver.FindElement(
                        By.XPath("//select[@ng-model='CatalogStatusId']"));

            }
        }

        /// <summary>
        /// Std config type check box
        /// </summary>
        public IWebElement StdConfigTypeCheckbox
        {
            get
            {
                return
                    webDriver.FindElement(
                        By.XPath("//input[@ng-model='parIsStandardConfig']"));
            }
        }
        
        /// <summary>
        /// Catalog Name text box
        /// </summary>
        public IWebElement CatalogName
        {
            get { return webDriver.FindElement(By.Id("txtCatalogName")); }
        }

        /// <summary>
        /// Thread ID text box
        /// </summary>
        public IWebElement ThreadId
        {
            get { return webDriver.FindElement(By.Id("txtThreadId"), 30); }
        }

        /// <summary>
        /// Creation Date (Start) field
        /// </summary>
        public IWebElement CreationDateStart
        {
            get { return webDriver.FindElement(By.Id("dtStartDate")); }
        }

        /// <summary>
        /// Creation Date (End) field
        /// </summary>
        public IWebElement CreationDateEnd
        {
            get { return webDriver.FindElement(By.Id("dtEndDate")); }
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
        /// Catalog Type - Inventory Checkbox
        /// </summary>
        public IWebElement InventoryCheckbox
        {
            get
            {
                return webDriver.FindElement(AdeptBy.Attribute(ElementTag.input, "value", "Inventory"));
            }
        }

        /// <summary>
        /// Test harness checkbox
        /// </summary>
        public IWebElement TestHarnessCheckbox
        {
            get
            {
                return this.webDriver.FindElement(By.XPath("//input[@ng-model='parHarness']"), 30);
                //return webDriver.FindElement(By.XPath("//*[@id='myForm']/table/tbody/tr/td[1]/table/tbody/tr[4]/td[4]/input[2]"));
            }
        }

        /// <summary>
        /// DownloadButton on Auto cat List page
        /// </summary>
        public IWebElement DownloadButton
        {
            get
            {
                return webDriver.FindElement(By.XPath("//*[@id='homepage-var']/div/div[2]/div/div/div/div[1]/div/table/tbody/tr[1]/td[16]/input"));
            }
        }

        /// <summary>
        /// Show Scheduled check box
        /// </summary>
        public IWebElement ScheduledCheckbox
        {
            get { return webDriver.FindElement(AdeptBy.Attribute(ElementTag.input, "ng-model", "parScheduled")); }
        }

        /// <summary>
        /// Search Records hyperlink
        /// </summary>
        public IWebElement SearchRecordsLink
        {
            get { return webDriver.FindElement(By.Id("lnkSearch")); }
        }

        /// <summary>
        /// Clear All hyperlink
        /// </summary>
        public IWebElement ClearAllLink
        {
            get { return webDriver.FindElement(By.Id("lnkClear")); }
        }

        /// <summary>
        /// Disclaimer for Inventory
        /// </summary>
        public IWebElement InventoryDisclaimer
        {
            get { return webDriver.FindElement(By.Id("lblDisclaimer")); }
        }

        /// <summary>
        /// Auto Catalog List Page results Table 
        /// </summary>
        public ReadOnlyCollection<IWebElement> CatalogListTableRows
        {
            get { return webDriver.FindElements(By.XPath("//table[@st-safe-src='Catalogs']/tbody/tr")); }
        }

        /// <summary>
        /// Auto Catalog List Page results Table Header
        /// </summary>
        public IWebElement CatalogListTableHeader
        {
            get { return webDriver.FindElement(By.XPath("//table[@st-safe-src='Catalogs']/thead")); }
        }

        /// <summary>
        /// No of pages
        /// </summary>
        public IWebElement NoOfPages
        {
            get { return webDriver.FindElement(By.XPath("//span[@class='ng-scope']/span/span[2]/span")); }
        }

        ///<summary>
        /// Auto Catalog Part Viewer Quote Ids Link
        /// </summary>
        public IWebElement PartViewerQuoteIdsLink
        {
            get { return webDriver.FindElement(By.Id("quoteid")); }
        }

        ///<summary>
        /// Auto Catalog part Viewer Search Button
        /// </summary>
        public IWebElement PartViewerSearchButton
        {
            get
            {
                return webDriver.FindElement(By.XPath("//*[@id='homepage-var']/div/div[2]/div/div[1]/div[2]/button"));
            }
        }

        ///<summary>
        /// Part Viewer table Parent Header 
        /// </summary>
        public ReadOnlyCollection<IWebElement> PartViewerHeader
        {

            get { return webDriver.FindElements(By.XPath("//*[@id='quoteTable']/thead/tr")); }
        }

        ///<summary>
        /// Part Viewer Table 1st Row in first plus button
        /// </summary>
        public ReadOnlyCollection<IWebElement> PartViewerFirstRows
        {

            get { return webDriver.FindElements(By.XPath("//*[@id='quoteTable']/tbody[1]/tr[1]")); }
        }
        
        ///<summary>
        /// Part Viewer Page '+' First Button
        /// </summary>
        public IWebElement PartViewerPlusButton
        {
            get { return webDriver.FindElement(By.XPath("//*[@id='quoteTable']/tbody[1]/tr[1]/td[1]/img")); }
        }

        ///<summary>
        /// Part Viewer Sub Header after clicking 1st plus button
        /// </summary>
        public ReadOnlyCollection<IWebElement> PartViewerSubHeader
        {
            get { return webDriver.FindElements(By.XPath("//*[@id='quoteTable']/tbody[1]/tr[2]/td[2]/table/thead/tr")); }
        }

        ///<summary>
        /// Part Viewer Table sub Rows in first sub header
        /// </summary>
        public ReadOnlyCollection<IWebElement> PartViewerSubRows
        {

            get { return webDriver.FindElements(By.XPath("//*[@id='quoteTable']/tbody[1]/tr[2]/td[2]/table/tbody/tr")); }
        }

        ///<summary>
        /// Part Viewer '+' second Button 
        /// </summary>
        public IWebElement PartViewerSecondPlusButton
        {
            get { return webDriver.FindElement(By.XPath("//*[@id='quoteTable']/tbody[2]/tr[1]/td[1]/img")); }
        }

        ///<summary>
        /// Part Viewer Table Second Row
        /// </summary>
        public ReadOnlyCollection<IWebElement> PartViewerSecondRow
        {

            get { return webDriver.FindElements(By.XPath("//*[@id='quoteTable']/tbody[2]/tr[1]")); }
        }
      
        ///<summary>
        /// Part Viewer Sub Header after clicking second plus button
        /// </summary>
        public ReadOnlyCollection<IWebElement> PartViewerSecondSubHeader
        {
            get { return webDriver.FindElements(By.XPath("//*[@id='quoteTable']/tbody[2]/tr[2]/td[2]/table/thead/tr")); }
        }

        ///<summary>
        /// Part Viewer Table sub Rows in second sub header
        /// </summary>
        public ReadOnlyCollection<IWebElement> PartViewerSecondSubRows
        {

            get { return webDriver.FindElements(By.XPath("//*[@id='quoteTable']/tbody[2]/tr[2]/td[2]/table/tbody/tr")); }
        }

        public IWebElement NextButton
        {
            get
            {
                return webDriver.FindElement(By.Id("nextUpButton"));
            }
        }

        public IWebElement CatalogsTable
        {
            get
            {
                return webDriver.FindElement(By.XPath("//table[@st-table='Channel' and @st-safe-src='Catalogs']"));
            }
        }

        public IWebElement PrevButton
        {
            get
            {
                return webDriver.FindElement(By.Id("previousUpButton"));
            }
        }

        /// <summary>
        /// Text box in the paging section
        /// </summary>
        public IWebElement PageNumberTextbox
        {
            get { return webDriver.FindElement(By.XPath("//page-select/input")); }
        }

        /// <summary>
        /// Span element containing the elements related to paging
        /// </summary>
        public IWebElement PagingSpan
        {
            get { return webDriver.FindElement(By.XPath("//span[page-select]")); }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Status table as a dictionary with Key = Status & Value = Description
        /// </summary>
        /// <returns></returns>
        public IDictionary<string, string> GetStatusDictionary()
        {
            //var rows = StatusTable[0].FindElements(By.TagName("tr"));
            //Dictionary<string, string> statusAndDescription = new Dictionary<string, string>();
            //for (int i = 1; i < rows.Count; i++)
            //{
            //    var columns = rows[i].FindElements(By.TagName("td"));
            //    statusAndDescription.Add(columns[0].Text, columns[1].Text);
            //}
            //return statusAndDescription;

            return StatusTable.ToDictionary(element => element.FindElements(By.TagName("td"))[0].Text,
                element => element.FindElements(By.TagName("td"))[1].Text);
        }

        /// <summary>
        /// Selects the specified customer from the Select customer drop down
        /// </summary>
        /// <param name="profileName"></param>
        public void SelectTheCustomer(string profileName)
        {
            webDriver.FindElement(By.XPath("//div[@ng-model='customer']/a")).Click();
            webDriver.FindElement(By.XPath("//div[@ng-model='customer']/div/div[@class='custom-select-search']/input"))
                .SendKeys(profileName);
            webDriver.FindElement(By.XPath("//div[@ng-model='customer']/div/ul/li[1]/a[text()='" + profileName + "']"))
                .Click();
        }

        /// <summary>
        /// Selects the specified Region from the Select Region drop down
        /// </summary>
        /// <param name="profileName"></param>
        public void SelectTheRegion(string regionName)
        {
            webDriver.FindElement(By.XPath("//div[@ng-model='region']/a")).Click();
            webDriver.FindElement(By.XPath("//div[@ng-model='region']/div/div[@class='custom-select-search']/input"))
                .SendKeys(regionName);
            webDriver.FindElement(By.XPath("//div[@ng-model='region']/div/ul/li[1]/a[text()='" + regionName + "']"))
                .Click();
        }

        /// <summary>
        /// Selects the country from the country listed
        /// if not specified, selects the first country from the drop down
        /// </summary>
        /// <param name="identity"></param>
        public void SelectTheCountry(string country = "")
        {
            webDriver.FindElement(By.XPath("//div[@ng-model='country']/a")).Click();
            if (string.IsNullOrEmpty(country))
            {
                webDriver.FindElements(By.XPath("//div[@ng-model='country']/div/ul/li[1]/a"))[0].Click();
            }
            else
            {
                webDriver.FindElement(By.XPath("//input[@ng-model='search.UserName']")).SendKeys(country);
                webDriver.FindElement(By.XPath("//div[@ng-model='Identity']//div[@class='dropdown-menu ng-scope']//ul[@role='menu']/li[@class='ng-scope']/a[@role='menuitem']")).Click();

            }
        }

        /// <summary>
        /// Selects the status from the Select drop down
        /// </summary>
        /// <param name="status"></param>
        public void SelectTheStatus(string status)
        {
            webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(60));
            webDriver.FindElement(By.XPath("//div[@ng-model='CatalogStatusId']/a")).Click();
            webDriver.FindElement(By.XPath("//div[@ng-model='CatalogStatusId']/div/div[@class='custom-select-search']/input")).SendKeys(status);
            webDriver.FindElement(By.XPath("//div[@ng-model='CatalogStatusId']/div/ul/li/a[contains(text(),'" + status + "')]")).Click();
        }

        /// <summary>
        /// Selects the identity from the identities listed
        /// if not specified, selects the first identity from the drop down
        /// </summary>
        /// <param name="identity"></param>
        public void SelectTheIdentity(string identity = "")
        {
            webDriver.FindElement(By.XPath("//div[@ng-model='Identity']/a")).Click();
            if (string.IsNullOrEmpty(identity))
            {
                webDriver.FindElements(By.XPath("//div[@ng-model='Identity']/div/ul/li[1]/a"))[0].Click();
            }
            else
            {
                webDriver.FindElement(By.XPath("//input[@ng-model='search.UserName']")).SendKeys(identity);
                webDriver.FindElement(By.XPath("//div[@ng-model='Identity']//div[@class='dropdown-menu ng-scope']//ul[@role='menu']/li[@class='ng-scope']/a[@role='menuitem']")).Click();

            }
        }

        /// <summary>
        /// Verified if all the records on the current page are Inventory Feeds
        /// </summary>
        /// <returns></returns>
        public bool AreAllRowsInventory()
        {
            return
                CatalogListTableRows.All(
                    r => r.FindElements(By.TagName("td"))[2].Text.ToLowerInvariant().Equals("inventory"));
        }

        #endregion
    }
}
