// ***********************************************************************
// Author           : AMERICAS\Nethra_Pandappilav
// Created          : 12/10/2014 5:50:13 PM
//
// Last Modified By : AMERICAS\Nethra_Pandappilav
// Last Modified On : 12/10/2014 5:50:13 PM
// ***********************************************************************
// <copyright file="B2BBuyerCatalogListPage.cs" company="Dell">
//     Copyright (c) Dell 2014. All rights reserved.
// </copyright>
// <summary>Provide a summary of the page class here.</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;
using DCSG.ADEPT.Framework;
using DCSG.ADEPT.Framework.Core;
using DCSG.ADEPT.Framework.Core.Extensions.WebDriver;
using DCSG.ADEPT.Framework.Core.Extensions.WebElement;
using DCSG.ADEPT.Framework.Core.Extensions.Locators;
using DCSG.ADEPT.Framework.Core.Page;
using OpenQA.Selenium.Support.UI;
using System.Collections;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.SharePoint.Client.WebParts;

namespace Modules.Channel.B2B.Core.Pages
{
    using System.Collections.ObjectModel;

    using OpenQA.Selenium.Interactions;

    /// <summary>
    /// This base class is the where all specific page classes will be derived.
    /// </summary>
    public class B2BBuyerCatalogListPage : DCSGPageBase
    {
        IWebDriver webDriver;
        private IJavaScriptExecutor javaScriptExecutor;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public B2BBuyerCatalogListPage(IWebDriver webDriver)
            : base(ref webDriver)
        {
            this.webDriver = webDriver;
            javaScriptExecutor = (IJavaScriptExecutor)this.webDriver;
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
            return SearchCatalogLink.IsElementVisible();
        }

        /// <summary>
        /// determines whether or not the driver is active on this page. Must be overriden with each subclass.
        /// </summary>
        /// <returns>active</returns>
        public override bool IsActive()
        {
            return webDriver.Url.ToLower().Contains("buyercataloglist.aspx");
        }

        #region Elements

        private SelectElement SelectCustomer
        {
            get
            {
                webDriver.WaitForElementDisplayed(By.Id("ContentPageHolder_cboCustomer"), TimeSpan.FromSeconds(30));
                return new SelectElement(webDriver.FindElement(By.Id("ContentPageHolder_cboCustomer")));
            }
        }

        private IWebElement SearchCatalogLink
        {
            get
            {
                return webDriver.FindElement(By.Id("ContentPageHolder_lnkSearch"));
            }
        }

        private ReadOnlyCollection<IWebElement> CatalogListRows
        {
            get
            {
                return webDriver.FindElements(By.XPath("//table[@id='G_ContentPageHolderxgrdCatalogListxgrdCatList']/tbody/tr"));
            }
        }

        #endregion

        /// <summary>
        /// Call this method to search for a Buyer Catalog providing the profile name and identity
        /// </summary>
        /// <param name="profileName">Customer/Profile name</param>
        public void SearchForBuyerCatalog(string profileName)
        {
            if (!SelectCustomer.SelectedOption.Text.Equals(profileName))
            {
                SelectCustomer.SelectByText(profileName);
            }

            ////SearchCatalogLink.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", SearchCatalogLink);
            webDriver.WaitForPageLoad(new TimeSpan(0, 0, 10));
        }

        /// <summary>
        /// Checks if the status of the catalog is 'Available'
        /// If so, clicks the catalog name
        /// </summary>
        /// <param name="threadId">Thread Id associated with the catalog</param>
        /// <returns>true if the status is available and if the link is clicked</returns>
        public bool CheckCatalogAvailabilityAndAct(string threadId)
        {
            // Returns the row with the thread ID
            var rowWithCatalogName = CatalogListRows.FirstOrDefault(e => e.FindElement(By.XPath("//td[8]")).Text.Contains(threadId));

            // Checks if the catalog status is 'Available'
            if (!rowWithCatalogName.FindElements(By.TagName("td"))[2].Text.Contains("Available"))
            {
                return false;
            }

            // Click on the corresponding link to view the catalog
            ////rowWithCatalogName.FindElement(By.XPath("//td[2]/nobr/a")).Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", rowWithCatalogName.FindElement(By.XPath("//td[2]/nobr/a")));
            webDriver.WaitForPageLoad(new TimeSpan(0, 0, 10));

            return true;
        }
    }
}
