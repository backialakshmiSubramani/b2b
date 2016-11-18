// ***********************************************************************
// Author           : AMERICAS\Shaukat_Saleem
// Created          : 10/13/2016 3:12:50 PM
//
// Last Modified By : AMERICAS\Shaukat_Saleem
// Last Modified On : 10/13/2016 3:12:50 PM
// ***********************************************************************
// <copyright file="OstFlowPage.cs" company="Dell">
//     Copyright (c) Dell 2016. All rights reserved.
// </copyright>
// <summary>Provide a summary of the page class here.</summary>
// ***********************************************************************
using Dell.Adept.UI.Web.Pages;
using Dell.Adept.UI.Web.Support.Extensions.WebDriver;
using Dell.Adept.UI.Web.Support.Extensions.WebElement;
using Modules.Channel.B2B.Common;
using OpenQA.Selenium;
using System;

namespace Modules.Channel.B2B.Core.NewPages
{
    /// <summary>
    /// This base class is the where all specific page classes will be derived.
    /// </summary>
    public class OSTMainPage : PageBase

    {
        IWebDriver webDriver;
        private IJavaScriptExecutor javaScriptExecutor;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public OSTMainPage(IWebDriver webDriver) : base(ref webDriver)
        {
            this.webDriver = webDriver;
            javaScriptExecutor = (IJavaScriptExecutor)this.webDriver;
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
                webDriver.WaitForElementDisplayed(By.Id("ctl00_topHeaderControl_tbContrHeader_tbpnlPgSetting_txtbxAutoComplete"), new TimeSpan(0, 0, 30));
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

        public IWebElement StandardConfigurationsLink
        {
            get
            {
                return webDriver.FindElement(By.LinkText("Standard Configurations"));
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

        public void OpenOSTHomePage()
        {
            webDriver.Navigate().GoToUrl(ConfigurationReader.GetValue("OSTHomePageUrl"));
        }

        /// <summary>
        /// Clicks on Go button and navigates to Catalog and Pricing Page
        /// </summary>
        /// <param name="accountId"></param>
        public void SearchStoreInOST(string accountId)
        {
            webDriver.WaitForPageLoad(TimeSpan.FromSeconds(30));
            AccountId.SendKeys(accountId);
            ////GoButton.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", GoButton);
            webDriver.WaitForElementDisplayed(By.Id("ctl00_brdcrbControl_lbl_PageMigrationinfo"), TimeSpan.FromSeconds(60));
        }

        public void GotoStandardConfigPage()
        {
            StandardConfigurationsLink.SendKeys(Keys.Enter);
            webDriver.WaitForPageLoadNew(TimeSpan.FromSeconds(60));          
        }
    }
}
