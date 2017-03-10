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
using OpenQA.Selenium.Support.UI;
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

        private IWebElement EnableBHCCatalogAutoGeneration
        {
            get
            {
                return webDriver.FindElement(By.Id("chk_BC_BuyerCatalogCheck"));
            }
        }

        private IWebElement AutomatedBhcCatalogProcessingRules
        {
            get
            {
                return webDriver.FindElement(By.XPath("//*[@id='autobhccatalogprocessingrulesplus']/a/img"));
            }
        }

        /// <summary>
        /// 'Click to Run Once' button under 'Inventory Feed - Processing Rules' section
        /// Click to Run Once Button - to perform catalog Inventory Feed manually
        /// </summary>
        private IWebElement ClickToRunOnceButton
        {
            get
            {
                return webDriver.FindElement(By.Id("ContentPageHolder_btnClicktoRunOnce"), new TimeSpan(0, 0, 10));
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
                    _updateButton = webDriver.FindElement(By.Id("ContentPageHolder_lnk_BC_Save"), new TimeSpan(0, 0, 60));
                }
                return _updateButton;
            }
        }

        private IWebElement InternalEmailAddresses
        {
            get
            {
                return webDriver.FindElement(By.Id("txtInterEmail"), new TimeSpan(0, 0, 60));
            }
        }

        private IWebElement ExternalEmailAddresses => webDriver.FindElement(By.Id("txtCustEmail"), new TimeSpan(0, 0, 60));

        private string DaysInterval
        {
            get
            {
                IWebElement comboBox = webDriver.FindElement(By.Id("ContentPageHolder_dd_AutomatedATSIntervaldays"));
                SelectElement selectedValue = new SelectElement(comboBox);
                return selectedValue.SelectedOption.Text;
            }
        }

        private string HoursInterval
        {
            get
            {
                IWebElement comboBox = webDriver.FindElement(By.Id("ContentPageHolder_dd_AutomatedATSIntervalhr"));
                SelectElement selectedValue = new SelectElement(comboBox);
                return selectedValue.SelectedOption.Text;
            }
        }

        private string MinutesInterval
        {
            get
            {
                IWebElement comboBox = webDriver.FindElement(By.Id("ContentPageHolder_dd_AutomatedATSIntervalmn"));
                SelectElement selectedValue = new SelectElement(comboBox);
                return selectedValue.SelectedOption.Text;
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

        public void ExpandAutomatedBHCCatalogSection()
        {
            //Expand Auto BHC Section
            AutomatedBhcCatalogProcessingRules.Click();
            if (!EnableBHCCatalogAutoGeneration.Selected)
            {
                EnableBHCCatalogAutoGeneration.Click();
            }
        }

        public void SelectAllIdentities()
        {
            foreach (var item in CheckedIdentityList)
            {
                if (!item.FindElements(By.TagName("input"))[0].Selected)
                {
                    item.FindElements(By.TagName("input"))[0].Click();
                }
            }
        }

        public bool UpdateBuyerCatalog()
        {
            UpdateButton.Click();
            return string.Equals(ConfirmationLabel.Text.Trim(), "Buyer Catalog details saved successfully.");
        }

        public bool ClickToRunOnce()
        {
            Console.WriteLine("Clicking on ClickToRunOnce Button..");
            ClickToRunOnceButton.Click();
            PageUtility.WaitForPageRefresh(webDriver);
            Console.WriteLine("Done!");
            Console.WriteLine("Inventory Feed Request Status : {0}", ConfirmationLabel.Text);
            return string.Equals(ConfirmationLabel.Text.Trim(), "Inventory feed request initiated.");
        }

        public void UpdateInternalEmailAddresses(string internalEmailAddress)
        {
            InternalEmailAddresses.Set(internalEmailAddress);
        }

        public void UpdateExternalEmailAddresses(string externalEmailAddress)
        {
            ExternalEmailAddresses.Set(externalEmailAddress);
        }

        public void RetrieveRefreshInterval(out string daysRefreshInterval,out string hoursRefreshInterval, out string minutesRefreshInterval)
        {
            daysRefreshInterval = DaysInterval;
            hoursRefreshInterval = HoursInterval;
            minutesRefreshInterval = MinutesInterval;
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
