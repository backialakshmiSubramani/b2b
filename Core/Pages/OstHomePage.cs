// ***********************************************************************
// Author           : AMERICAS\Nethra_Pandappilav
// Created          : 12/3/2014 2:01:56 PM
//
// Last Modified By : AMERICAS\Nethra_Pandappilav
// Last Modified On : 12/3/2014 2:01:56 PM
// ***********************************************************************
// <copyright file="OstHomePage.cs" company="Dell">
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


namespace Modules.Channel.B2B.Core.Pages
{
    /// <summary>
    /// This base class is the where all specific page classes will be derived.
    /// </summary>
    public class OstHomePage : DCSGPageBase
    {
        IWebDriver webDriver;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public OstHomePage(IWebDriver webDriver)
            : base(ref webDriver)
        {
            this.webDriver = webDriver;
            //populate the following variables with the appropriate value
            //Name = "";
            //Url = "";
            //ProductUnit = "";

        }

        /// <summary>
        /// Gets the account id.
        /// </summary>
        private IWebElement AccountId
        {
            get
            {
                return webDriver.FindElement(By.Id("ctl00_topHeaderControl_tbContrHeader_tbpnlPgSetting_txtbxAutoComplete"));
            }
        }

        /// <summary>
        /// Gets the go button.
        /// </summary>
        private IWebElement GoButton
        {
            get
            {
                return webDriver.FindElement(By.Id("ctl00_topHeaderControl_tbContrHeader_tbpnlPgSetting_imgbtnGo"));
            }
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

        /// <summary>
        /// Clicks on Go button and navigates to Catalog and Pricing Page
        /// </summary>
        /// <param name="accountId"></param>
        public void GoToCatalogAndPricingPage(string accountId)
        {
            webDriver.WaitForPageLoad(TimeSpan.FromSeconds(20));
            AccountId.SendKeys(accountId);
            GoButton.Click();
            webDriver.WaitForElementDisplayed(By.Id("ctl00_brdcrbControl_lbl_PageMigrationinfo"), TimeSpan.FromSeconds(30));
        }
    }
}
