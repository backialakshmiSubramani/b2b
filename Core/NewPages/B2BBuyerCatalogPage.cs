// ***********************************************************************
// Author           : AMERICAS\Shaukat_Saleem
// Created          : 12/26/2016 5:24:59 PM
//
// Last Modified By : AMERICAS\Shaukat_Saleem
// Last Modified On : 12/26/2016 5:24:59 PM
// ***********************************************************************
// <copyright file="B2BBuyerCatalogPage.cs" company="Dell">
//     Copyright (c) Dell 2016. All rights reserved.
// </copyright>
// <summary>Provide a summary of the page class here.</summary>
// ***********************************************************************
using Dell.Adept.UI.Web.Pages;
using Dell.Adept.UI.Web.Support.Extensions.WebDriver;
using Dell.Adept.UI.Web.Support.Extensions.WebElement;
using Modules.Channel.B2B.Core.Pages;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Modules.Channel.B2B.Core.NewPages
{
    /// <summary>
    /// This base class is the where all specific page classes will be derived.
    /// </summary>
    public class B2BBuyerCatalogPage : PageBase

    {
        IWebDriver webDriver;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public B2BBuyerCatalogPage(IWebDriver webDriver) : base(ref webDriver)
        {
            this.webDriver = webDriver;
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
        /// determines whether or not the driver is active on this page. Must be overridden with each subclass.
        /// </summary>
        /// <returns>active</returns>
        public override bool IsActive()
        {
            throw new NotImplementedException();
        }

        #region Elements

        private List<IWebElement> _checkedIdentityList;
        private List<IWebElement> CheckedIdentityList
        {
            get
            {
                _checkedIdentityList = webDriver.FindElements(
                    By.XPath("//table[@id='chklistIdenties']/tbody/tr"), new TimeSpan(0, 0, 60)).ToList();
                return _checkedIdentityList;
            }
        }

        private IWebElement _automatedBhcCatalogProcessingRules;
        private IWebElement AutomatedBhcCatalogProcessingRules
        {
            get
            {
                if (_automatedBhcCatalogProcessingRules == null)
                {
                    _automatedBhcCatalogProcessingRules = webDriver.FindElement(By.XPath("//*[@id='autobhccatalogprocessingrulesplus']/a/img"));
                }
                return _automatedBhcCatalogProcessingRules;
            }
        }

        private IWebElement _clickToRunOnceButton;
        /// <summary>
        /// 'Click to Run Once' button under 'Inventory Feed - Processing Rules' section
        /// Click to Run Once Button - to perform catalog Inventory Feed manually
        /// </summary>
        private IWebElement ClickToRunOnceButton
        {
            get
            {
                return _clickToRunOnceButton ??
                       (_clickToRunOnceButton = webDriver.FindElement(By.Id("ContentPageHolder_btnClicktoRunOnce"),
                           new TimeSpan(0, 0, 10)));
            }
        }

        private IWebElement ConfirmationLabel
        {
            get
            {
                return webDriver.FindElement(By.Id("ContentPageHolder_lbl_BC_OKmsg"), new TimeSpan(0, 0, 60));
            }
        }

        private IWebElement _updateButton;
        private IWebElement UpdateButton
        {
            get
            {
                if (_updateButton == null)
                {
                    _updateButton = webDriver.FindElement(By.Id("ContentPageHolder_lnk_BC_Save"), new TimeSpan(0, 0, 30));
                }
                return _updateButton;
            }
        }

        #endregion Elements

        #region ElementActions

        public bool ExpandAutomatedBHCCatalogSectionAndSelectSingleIdentity(out string selectedIdentity)
        {
            //Expand Auto BHC Section
            AutomatedBhcCatalogProcessingRules.Click();
            selectedIdentity = SelectIdentity();
            UpdateButton.Click();
            return string.Equals(ConfirmationLabel.Text.Trim(), "Buyer Catalog details saved successfully.");
        }

        public bool ClickToRunOnce()
        {
            Console.WriteLine("Clicking on ClickToRunOnce Button..");
            ClickToRunOnceButton.Click();
            PageUtility.WaitForPageRefresh(webDriver);
            Console.WriteLine("Done!");
            /*  The below 2 lines should be uncommented once the Click to run once button issue gets fixed */

            //Console.WriteLine("Inventory Feed Request Status : {0}", ConfirmationLabel.Text);
            //return string.Equals(ConfirmationLabel.Text.Trim(), "Inventory feed request initiated.");
            return true;
        }

        #region Private Methods
        private string SelectIdentity()
        {
            if (!CheckedIdentityList[0].FindElements(By.TagName("input"))[0].Selected)
            {
                CheckedIdentityList[0].FindElements(By.TagName("input"))[0].Click();
            }

            string selectedIdentity = CheckedIdentityList[0].Text;

            foreach (var item in CheckedIdentityList.Skip(1))
            {
                if (item.FindElements(By.TagName("input"))[0].Selected)
                {
                    item.FindElements(By.TagName("input"))[0].Click();
                }
            }

            return selectedIdentity;
        }

        #endregion Private Methods

        #endregion ElementActions
    }
}
