// ***********************************************************************
// Author           : AMERICAS\Nethra_Pandappilav
// Created          : 1/6/2015 5:10:46 PM
//
// Last Modified By : AMERICAS\Nethra_Pandappilav
// Last Modified On : 1/6/2015 5:10:46 PM
// ***********************************************************************
// <copyright file="B2BQuoteViewerPage.cs" company="Dell">
//     Copyright (c) Dell 2015. All rights reserved.
// </copyright>
// <summary>Provide a summary of the page class here.</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;
using System.Linq;

//Adept Framework 
using Dell.Adept.Core;
using Dell.Adept.UI;
using Dell.Adept.UI.Web;
using Dell.Adept.UI.Web.Pages;
using Dell.Adept.UI.Web.Support.Extensions.WebDriver;
using Dell.Adept.UI.Web.Support.Extensions.WebElement;
using Dell.Adept.UI.Web.Support.Locators;
using Dell.Adept.UI.Web.Support;
using Modules.Channel.B2B.Common;

namespace Modules.Channel.B2B.Core.Pages
{
    /// <summary>
    /// This base class is the where all specific page classes will be derived.
    /// </summary>
    public class B2BQuoteViewerPage : PageBase
    {
        IWebDriver webDriver;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public B2BQuoteViewerPage(IWebDriver webDriver)
            : base(ref webDriver)
        {
            this.webDriver = webDriver;
            //populate the following variables with the appropriate value
            //Name = "";
            //Url = "";
            //ProductUnit = "";

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

        #region Elements

        private IWebElement FirstItemRow
        {
            get
            {
                return webDriver.FindElement(By.XPath("//table[@id='ContentPageHolder_dg_QV_Items']/tbody/tr[2]"));
            }
        }

        private IEnumerable<IWebElement> QuoteViewerTableRowList
        {
            get
            {
                return webDriver.FindElement(By.Id("ContentPageHolder_dg_QV_Items")).FindElements(By.TagName("tr")).Skip(1);
            }
        }

        ///<summary>
        /// Auto Catalog part Viewer history Checkbox
        /// </summary>
        public IWebElement HistoryCheckbox
        {
            get
            {
                return webDriver.FindElement(By.CssSelector("input[type='checkbox'][ng-model='isHistoryEnabled']"));
            }
        }

        ///<summary>
        /// Auto Catalog part Viewer Instant Checkbox
        /// </summary>
        public IWebElement InstantCheckbox
        {
            get
            {
                return webDriver.FindElement(By.CssSelector("input[type='checkbox'][ng-model='isInstant']"));
            }
        }

        ///<summary>
        /// Auto Catalog Part Viewer Quote Ids Link
        /// </summary>
        public IWebElement MPNTextBox
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
                return webDriver.FindElement(By.CssSelector("button[type='button'][class='btn btn-primary']"));
            }
        }

        public IWebElement SelectRegionSpan
        {
            get
            {
                return webDriver.FindElement(By.XPath("//span[contains(text(),'Select Region')]"));
            }
        }

        public IWebElement SelectCountrySpan
        {
            get
            {
                return webDriver.FindElement(By.XPath("//a[@class='dropdown-toggle']/span[@class='ng-binding']"));
            }
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

        public IWebElement SelectStatusSpan
        {
            get { return webDriver.FindElement(By.XPath("//span[contains(text(),'2 items selected ..')]")); }
        }

        public IWebElement QuoteHistoryTable
        {
            get
            {
                return webDriver.FindElement(By.XPath("//table[@id='quoteHistoryTable' and @st-safe-src='quoteHistoryDetails']"));
            }
        }
        public IWebElement ExportToExcelButton
        {
            get
            {
                return webDriver.FindElement(By.XPath("//button[contains(text(),'Export to Excel')]"));
            }
        }
        #endregion

        #region Reusable Methods
        public void SelectOption(IWebElement webElement, string optionText)
        {
            webElement.Click();
            IWebElement textElement = webElement.FindElement(By.XPath("../following-sibling::div/child::ul/child::li/a[translate(text(),'abcdefghijklmnopqrstuvwxyz','ABCDEFGHIJKLMNOPQRSTUVWXYZ')='" + optionText + "']"));
            textElement.Click();
        }
        public bool CheckItemDetails(string description, string quantity, string unitPrice)
        {
            foreach (var t in this.QuoteViewerTableRowList)
            {
                if (t.FindElements(By.TagName("td"))[2].Text.Trim().ToLower().Contains(description.ToLower()))
                {
                    return t.FindElements(By.TagName("td"))[6].Text.Trim().Contains(quantity)
                           && t.FindElements(By.TagName("td"))[8].Text.Trim().Split(' ').Last().Contains(unitPrice);
                }
            }

            return false;
        }

        /// <summary>
        /// Selects the status from the Status list
        /// </summary>
        /// <param name="Status[i]"></param>
        public void SelectTheStatus(string[] Status = null)
        {
            string stat = string.Empty;
            var statusDropDown = webDriver.FindElement(By.XPath("//div[@class='dropdown custom-select']/a[@class='dropdown-toggle']/span[contains(text(),'2 items selected ..')]"));
            UtilityMethods.ClickElement(webDriver, statusDropDown);
            foreach (string s in Status)
            {
                var elem = webDriver.FindElement(By.XPath("//div[@class='dropdown-menu status']//ul[@role='menu']/li[@class='ng-binding ng-scope']/input[@ng-model='checked']"));
                var catalogStatusInput = webDriver.FindElement(By.XPath("//input[@ng-model='statusSearch.CatalogStatus']"));
                if (s == "Created")
                {
                    stat = UtilityMethods.ConvertToString<CatalogStatus>(CatalogStatus.Created);
                    catalogStatusInput.Clear();
                    catalogStatusInput.SendKeys(stat);
                    elem = webDriver.FindElement(By.XPath("//div[@class='dropdown-menu status']//ul[@role='menu']/li[@class='ng-binding ng-scope']/input[@ng-model='checked']"));
                    if (!elem.Selected)
                        elem.Click();
                }
                else if (s == "Created-Warning")
                {
                    stat = UtilityMethods.ConvertToString<CatalogStatus>(CatalogStatus.CreatedWarning);
                    catalogStatusInput.Clear();
                    catalogStatusInput.SendKeys(stat);
                    elem = webDriver.FindElement(By.XPath("//div[@class='dropdown-menu status']//ul[@role='menu']/li[@class='ng-binding ng-scope']/input[@ng-model='checked']"));
                    if (!elem.Selected)
                        elem.Click();
                }
                else if (s == "Published")
                {
                    stat = UtilityMethods.ConvertToString<CatalogStatus>(CatalogStatus.Published);
                    catalogStatusInput.SendKeys(stat);
                    elem = webDriver.FindElement(By.XPath("//div[@class='dropdown-menu status']//ul[@role='menu']/li[@class='ng-binding ng-scope']/input[@ng-model='checked']"));
                    if (!elem.Selected)
                        elem.Click();
                }
                else if (s == "Published-Warning")
                {
                    stat = UtilityMethods.ConvertToString<CatalogStatus>(CatalogStatus.PublishedWarning);
                    catalogStatusInput.Clear(); catalogStatusInput.SendKeys(stat);
                    elem = webDriver.FindElement(By.XPath("//div[@class='dropdown-menu status']//ul[@role='menu']/li[@class='ng-binding ng-scope']/input[@ng-model='checked']"));
                    if (!elem.Selected)
                        elem.Click();
                }
                else if (s == "Expired")
                {
                    stat = UtilityMethods.ConvertToString<CatalogStatus>(CatalogStatus.Expired);
                    catalogStatusInput.Clear(); catalogStatusInput.SendKeys(stat);
                    elem = webDriver.FindElement(By.XPath("//div[@class='dropdown-menu status']//ul[@role='menu']/li[@class='ng-binding ng-scope']/input[@ng-model='checked']"));
                    if (!elem.Selected)
                        elem.Click();
                }
                else if (s == "Created-Instant")
                {
                    stat = UtilityMethods.ConvertToString<CatalogStatus>(CatalogStatus.CreatedInstant);
                    catalogStatusInput.Clear(); catalogStatusInput.SendKeys(stat);
                    elem = webDriver.FindElement(By.XPath("//div[@class='dropdown-menu status']//ul[@role='menu']/li[@class='ng-binding ng-scope']/input[@ng-model='checked']"));
                    if (!elem.Selected)
                        elem.Click();
                }
                else
                {
                    stat = UtilityMethods.ConvertToString<CatalogStatus>(CatalogStatus.CreatedWarningInstant);
                    catalogStatusInput.Clear(); catalogStatusInput.SendKeys(stat);
                    elem = webDriver.FindElement(By.XPath("//div[@class='dropdown-menu status']//ul[@role='menu']/li[@class='ng-binding ng-scope']/input[@ng-model='checked']"));
                    if (!elem.Selected)
                        elem.Click();
                }
            }
            UtilityMethods.ClickElement(webDriver, statusDropDown);
        }

        /// <summary>
        /// Get all the statuses list in Status dropdown
        /// </summary>
        /// <param name="Status"></param>
        /// <returns></returns>
        public IList<string> GetCatalogStatusFromStatusDropdown(string[] Status = null)
        {
            UtilityMethods.ClickElement(webDriver, SelectStatusSpan);
            IList<IWebElement> allStatusesList = webDriver.FindElements(By.XPath("//div[@class='dropdown-menu status']//ul[@role='menu']/li[@class='ng-binding ng-scope']"));
            return allStatusesList.Select(c => c.Text).ToList();
        }

        /// <summary>
        /// Selects the country from the country listed
        /// if not specified, selects the first country from the drop down
        /// </summary>
        /// <param name="identity"></param>
        public void SelectTheCountry(string country = "")
        {
            if (country == "UK")
                country = UtilityMethods.ConvertToString<CountryName>(CountryName.UK);
            else if (country == "US")
                country = UtilityMethods.ConvertToString<CountryName>(CountryName.US);
            else if (country == "CA")
                country = UtilityMethods.ConvertToString<CountryName>(CountryName.CA);
            else if (country == "FR")
                country = UtilityMethods.ConvertToString<CountryName>(CountryName.FR);
            else if (country == "DE")
                country = UtilityMethods.ConvertToString<CountryName>(CountryName.DE);
            else if (country == "NL")
                country = UtilityMethods.ConvertToString<CountryName>(CountryName.NL);
            webDriver.FindElement(By.XPath("//div[@class='dropdown custom-select']/a")).Click();
            webDriver.FindElement(By.XPath("//input[@ng-model='search.Name']")).SendKeys(country);
            webDriver.FindElement(By.XPath("//div[@class='dropdown-menu countries']//ul[@role='menu']/li[@class='ng-binding ng-scope']/input[@ng-model='checked']")).Click();
        }

        #endregion
    }
}
