// ***********************************************************************
// Author           : AMERICAS\Gaurav_Bhardwaj3
// Created          : 11/28/2014 3:03:31 PM
//
// Last Modified By : AMERICAS\Gaurav_Bhardwaj3
// Last Modified On : 11/28/2014 3:03:31 PM
// ***********************************************************************
// <copyright file="CatalogAndPricingPage.cs" company="Dell">
//     Copyright (c) Dell 2014. All rights reserved.
// </copyright>
// <summary>Provide a summary of the page class here.</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

//Adept Framework 
using Dell.Adept.Core;
using Dell.Adept.UI;
using Dell.Adept.UI.Web;
using Dell.Adept.UI.Web.Pages;
using Dell.Adept.UI.Web.Support.Extensions.WebDriver;
using Dell.Adept.UI.Web.Support.Extensions.WebElement;
using Dell.Adept.UI.Web.Support.Locators;
using Dell.Adept.UI.Web.Support;

namespace Modules.Channel.B2B.Core.Pages
{
    /// <summary>
    /// This base class is the where all specific page classes will be derived.
    /// </summary>
    public class OstCatalogAndPricingPage : PageBase
    {
        IWebDriver webDriver;
        IJavaScriptExecutor javaScriptExecutor;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public OstCatalogAndPricingPage(IWebDriver webDriver)
            : base(ref webDriver)
        {
            this.webDriver = webDriver;
            javaScriptExecutor = (IJavaScriptExecutor)this.webDriver;
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
        public IWebElement OstLabel
        {
            get
            {
               return webDriver.FindElement(By.Id("ctl00_topHeaderControl_lblSiteHeading"));
            }
        }

        /// <summary>
        /// Find WebElement of AffinityID .
        /// </summary>
        public IWebElement AffinityId
        {
            get
            {
                return webDriver.FindElement(By.Id("ctl00_ContentPageHolder_txt_AffAccountID"));
            }
        }

        /// <summary>
        /// Find WebElement of Update Button .
        /// </summary>
        private IWebElement UpdateButton
        {
            get
            {
                return webDriver.FindElement(By.Id("ctl00_ContentPageHolder_uc_PublishAndUpdateTop_img_Update"));
            }
        }

        /// <summary>
        /// Find WebElement of Error, which comes when we try to update Affinity ID with value '0' .
        /// </summary>
        public IWebElement ErrorLabel
        {
            get
            {
               return webDriver.FindElement(By.Id("ctl00_ContentPageHolder_lbl_Error"));
            }
        }

        private IWebElement AddressWizardLink
        {
            get
            {
                this.webDriver.WaitForElementDisplayed(By.LinkText("Address Wizard"), TimeSpan.FromSeconds(30));
                return webDriver.FindElement(By.LinkText("Address Wizard"));
            }
        }

        public IWebElement AffinityAccountName
        {
            get
            {
                return webDriver.FindElement(By.Id("ctl00_ContentPageHolder_lblAffAccount"));
            }
        }

        private IWebElement FeaturesSetupLink
        {
            get
            {
                return webDriver.FindElement(By.LinkText("Features Setup"));
            }
        }

        #endregion

        #region Elements Actions

        /// <summary>
        /// Pass alphabets as AffinityID .
        /// </summary>
        public void AlphabetAffinityNegative(string accountId)
        {
            AffinityId.SendKeys(accountId);
        }

        /// <summary>
        /// Pass Alpha Numeric value as AffinityID .
        /// </summary>
        public void AlphaNumericAffinityNegative(string accountId)
        {
            AffinityId.Clear();
            AffinityId.SendKeys(accountId);
        }

        /// <summary>
        /// Pass Zero as AffinityID .
        /// </summary>
        public void ZeroAffinityNegative(string num)
        {
            AffinityId.Clear();
            AffinityId.SendKeys(num);
        }

        /// <summary>
        /// Pass Special Character as AffinityID .
        /// </summary>
        public void SpecialCharAffinityNegative(string accountId)
        {
            AffinityId.SendKeys(accountId);
        }

        public void NumericAffinityPositive(string numeric)
        {
            AffinityId.Clear();
            AffinityId.SendKeys(numeric);
        }

        /// <summary>
        /// Click on update button after passing any numeric value .
        /// </summary>
        public void UpdateAffinityId()
        {
            ////UpdateButton.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", UpdateButton);
            webDriver.WaitForElementDisplayed(By.Id("ctl00_ContentPageHolder_lbl_Error"), TimeSpan.FromSeconds(20));
        }

        public void GoToAdressWizardPage()
        {
            ////AddressWizardLink.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", AddressWizardLink);
            webDriver.WaitForPageLoad(new TimeSpan(0, 0, 10));
        }

        public void GoToFeatureSetupPage()
        {
            ////FeaturesSetupLink.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", FeaturesSetupLink);
            webDriver.WaitForPageLoad(new TimeSpan(0, 0, 60));
        }

        #endregion
    }
}
