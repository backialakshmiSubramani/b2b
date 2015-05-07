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
            return SelectCustomer.Displayed;
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
            get { return webDriver.FindElement(By.XPath("//table[@id='mytable']/tbody/tr/td/h3/span")); }
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
                        By.XPath("//table[@id='mytable']/tbody/tr[1]/td[2]/div/div/select[@ng-model='profiles']"));
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
                        By.XPath("//table[@id='mytable']/tbody/tr[1]/td[4]/div/div/select[@ng-model='Identitys']"));
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
        /// Show Scheduled check box
        /// </summary>
        public IWebElement ShowScheduled
        {
            get { return webDriver.FindElement(AdeptBy.Attribute(ElementTag.input, "ng-model", "Scheduled")); }
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

        #endregion
    }
}
