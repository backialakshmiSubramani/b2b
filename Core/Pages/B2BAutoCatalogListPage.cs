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


namespace Modules.Channel.B2B.Core.Pages
{
    /// <summary>
    /// This base class is the where all specific page classes will be derived.
    /// </summary>
    public class B2BAutoCatalogListPage : PageBase
    {
        IWebDriver webDriver;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public B2BAutoCatalogListPage(IWebDriver webDriver)
            : base(ref webDriver)
        {
            this.webDriver = webDriver;
            //populate the following variables with the appropriate value
            Name = "Channel Catalog List Page";
            Url = webDriver.Url;
            ProductUnit = "Channel";

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
            get { return webDriver.FindElement(By.XPath("//table[@id='mytable']/tbody/tr/td/h1/span")); }
        }

        /// <summary>
        /// List of Status in the upper right corner of the page
        /// </summary>
        public ReadOnlyCollection<IWebElement> StatusTable
        {
            get { return webDriver.FindElements(By.XPath("//form/table[1]/tbody/tr[1]/td[3]/table/tbody/tr[td]")); }
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
            get { return webDriver.FindElement(By.Id("txtThreadId")); }
        }

        /// <summary>
        /// Creation Date (Start) field
        /// </summary>
        public IWebElement CreationDateStart
        {
            get { return webDriver.FindElement(By.Name("dtStartDate")); }
        }

        /// <summary>
        /// Creation Date (End) field
        /// </summary>
        public IWebElement CreationDateEnd
        {
            get { return webDriver.FindElement(By.Name("dtEndDate")); }
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
        /// Test harness checkbox
        /// </summary>
        public IWebElement TestHarnessCheckbox
        {
            get
            {
                return webDriver.FindElement(By.XPath("//*[@id='myForm']/table/tbody/tr/td[1]/table/tbody/tr[4]/td[4]/input[2]"));
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
        /// Search Catalog hyperlink
        /// </summary>
        public IWebElement SearchCatalogLink
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
        /// Auto Catalog List Page results Table 
        /// </summary>
        public ReadOnlyCollection<IWebElement> CatalogListTableRows
        {
            //table[@st-safe-src='Catalogs']/tbody/tr
            get { return webDriver.FindElements(By.XPath("//table[@st-safe-src='Catalogs']/tbody/tr")); }
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
        
        #endregion

        #region Helper Methods

        /// <summary>
        /// Status table as a dictionary with Key = Status & Value = Description
        /// </summary>
        /// <returns></returns>
        public IDictionary<string, string> GetStatusDictionary()
        {
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

        #endregion

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
    }
}
