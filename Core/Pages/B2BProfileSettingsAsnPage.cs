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
    public class B2BProfileSettingsAsnPage : DCSGPageBase
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

        private SelectElement DeliveryPreferenceList
        {
            get
            {
                return
                    new SelectElement(
                        webDriver.FindElement(By.XPath("//select[contains(@id,'Asn_Delivery_Preference')]")));
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
                        DeliveryPreferenceList.SelectByText(DeliveryPreference);
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


        #endregion
    }
}
