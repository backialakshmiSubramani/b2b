// ***********************************************************************
// Author           : AMERICAS\Ygnashwaran_Sekar
// Created          : 12/2/2014 8:02:31 PM
//
// Last Modified By : AMERICAS\Ygnashwaran_Sekar
// Last Modified On : 12/2/2014 8:02:31 PM
// ***********************************************************************
// <copyright file="ProfileSettingsASNPage.cs" company="Dell">
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
    public class B2BProfileSettingsAsnPage : PageBase
    {
        IWebDriver webDriver;
        private IJavaScriptExecutor javaScriptExecutor;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public B2BProfileSettingsAsnPage(IWebDriver webDriver)
            : base(ref webDriver)
        {
            this.webDriver = webDriver;
            javaScriptExecutor = (IJavaScriptExecutor)webDriver;
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
        private IWebElement AsnTab
        {
            get
            {
                webDriver.WaitForElement(By.XPath("//a[contains(@href,'ASN')]"), TimeSpan.FromSeconds(60));
                return webDriver.FindElement(By.XPath("//a[contains(@href,'ASN')]"));
            }
        }

        private IWebElement EnableChannelAsnCheckbox
        {
            get
            {
                webDriver.WaitForElement(By.XPath("//input[contains(@id,'ASNEnableChannelCheck')]"), TimeSpan.FromSeconds(60));
                return webDriver.FindElement(By.XPath("//input[contains(@id,'ASNEnableChannelCheck')]"));
            }
        }

        private IWebElement UpdateButton
        {
            get
            {
                return webDriver.FindElement(By.XPath("//input[contains(@id,'Asn_Save')]"));
            }
        }

        private IWebElement UpdateSuccessMsg
        {
            get
            {
                webDriver.WaitForElement(By.XPath("//span[contains(@id,'Asn_OKmsg')]"), TimeSpan.FromSeconds(10));
                return webDriver.FindElement(By.XPath("//span[contains(@id,'Asn_OKmsg')]"));
            }
        }

        private IWebElement SelectProfileLink
        {
            get
            {
                return webDriver.FindElement(By.XPath("//a[contains(text(),'Select Profile')]"));
            }
        }

        private SelectElement DeliveryPreferenceDropdown
        {
            get
            {
                return new SelectElement(webDriver.FindElement(By.Id("ContentPageHolder_ddl_Asn_Delivery_Preference")));
            }
        }

        private IWebElement DellImageLink
        {
            get
            {
                return webDriver.FindElement(By.Id("HyplnkDellLogo"));
            }
        }

        #endregion

        #region ReUsable Methods

        public void EnableorDisableChannelAsnForProfile(string Option, string DeliveryPreference)
        {
            ////AsnTab.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", AsnTab);

            switch (Option)
            {
                case "Enable":
                    if (EnableChannelAsnCheckbox.GetAttribute("checked") != "true")
                    {
                        ////EnableChannelAsnCheckbox.Click();
                        javaScriptExecutor.ExecuteScript("arguments[0].click();", EnableChannelAsnCheckbox);
                        DeliveryPreferenceDropdown.SelectByText(DeliveryPreference);
                        ////UpdateButton.Click();
                        javaScriptExecutor.ExecuteScript("arguments[0].click();", UpdateButton);
                    }

                    break;

                case "Disable":
                    if (EnableChannelAsnCheckbox.GetAttribute("checked") == "true")
                    {
                        ////EnableChannelAsnCheckbox.Click();
                        javaScriptExecutor.ExecuteScript("arguments[0].click();", EnableChannelAsnCheckbox);
                        ////UpdateButton.Click();
                        javaScriptExecutor.ExecuteScript("arguments[0].click();", UpdateButton);
                    }

                    break;
            }
        }

        public bool UpdateSuccessMsgDisplayed()
        {
            return UpdateSuccessMsg.Displayed;
        }

        public void ClickSelectProfile()
        {
            ////SelectProfileLink.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", SelectProfileLink);
            webDriver.WaitForPageLoad(new TimeSpan(0, 0, 10));
        }

        public string GetDeliveryPreference()
        {
            return DeliveryPreferenceDropdown.SelectedOption.GetAttribute("value");
        }

        public void GoToHomePage()
        {
            javaScriptExecutor.ExecuteScript("arguments[0].click();", DellImageLink);
            webDriver.WaitForPageLoad(new TimeSpan(0, 0, 10));
        }

        #endregion
    }
}
