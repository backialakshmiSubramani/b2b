// ***********************************************************************
// Author           : AMERICAS\Nethra_Pandappilav
// Created          : 12/31/2014 10:51:05 AM
//
// Last Modified By : AMERICAS\Nethra_Pandappilav
// Last Modified On : 12/31/2014 10:51:05 AM
// ***********************************************************************
// <copyright file="OstFeatureSetupPage.cs" company="Dell">
//     Copyright (c) Dell 2014. All rights reserved.
// </copyright>
// <summary>Provide a summary of the page class here.</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;

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
    using OpenQA.Selenium.Support.UI;

    /// <summary>
    /// This base class is the where all specific page classes will be derived.
    /// </summary>
    public class OstFeatureSetupPage : PageBase
    {
        IWebDriver webDriver;

        private IJavaScriptExecutor javaScriptExecutor;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public OstFeatureSetupPage(IWebDriver webDriver)
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

        private IWebElement PartnerCustomerTabCheckbox
        {
            get
            {
                return webDriver.FindElement(By.Name("MastHeadAndFooter.DefaultMastheadSettings.EnablePartnerCustomers"));
            }
        }

        private IWebElement PagePreviewButton
        {
            get
            {
                return webDriver.FindElement(By.Id("ctl00_topHeaderControl_tbContrHeader_tbpnlPgSetting_imgbtnPgPrvw"));
            }
        }

        private SelectElement UserRolesDropdown
        {
            get
            {
                return
                    new SelectElement(
                        webDriver.FindElement(By.Id("ctl00_topHeaderControl_tbContrHeader_tbpnlPgSetting_ddlRolesPrvw")));
            }
        }

        private IWebElement PreviewButton
        {
            get
            {
                return webDriver.FindElement(By.Id("ctl00_topHeaderControl_tbContrHeader_tbpnlPgSetting_imgbtnPgPrevw"));
            }
        }

        private IWebElement UpdateButton
        {
            get
            {
                return webDriver.FindElement(By.Id("updatesettings2"));
                ////return webDriver.FindElement(By.Id("updatesettings1"));
            }
        }

        private IWebElement FeaturesSetupLink
        {
            get
            {
                return webDriver.FindElement(By.LinkText("Features Setup"));
            }
        }

        private IWebElement UpdateSuccessfulMessage
        {
            get
            {
                return webDriver.FindElement(By.XPath("//label[.='Settings Updated Successfully']"));
            }
        }

        public void UnCheckPartnerCustomerTabCheckboxIfAlreadyChecked()
        {
            this.webDriver.SwitchTo().Frame(0);

            if (!this.PartnerCustomerTabCheckbox.Selected)
            {
                return;
            }

            // Uncheck the checkbox and click update if it is already checked.
            ////PartnerCustomerTabCheckbox.Click();
            this.javaScriptExecutor.ExecuteScript("arguments[0].click();", this.PartnerCustomerTabCheckbox);
            ////UpdateButton.Click();
            this.javaScriptExecutor.ExecuteScript("arguments[0].click();", this.UpdateButton);
            ////this.webDriver.SwitchTo().DefaultContent();
            this.webDriver.SwitchTo().Window(webDriver.CurrentWindowHandle);
            ////FeaturesSetupLink.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", FeaturesSetupLink);
            webDriver.WaitForPageLoad(new TimeSpan(0, 0, 30));
            this.webDriver.SwitchTo().Frame(0);
        }

        public void CheckPartnerCustomerTabCheckbox()
        {
            ////PartnerCustomerTabCheckbox.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", PartnerCustomerTabCheckbox);
        }

        public bool ClickUpdate()
        {
            ////UpdateButton.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", UpdateButton);
            webDriver.WaitForPageLoad(new TimeSpan(0, 0, 30));
            return this.UpdateSuccessfulMessage.IsElementVisible();
        }

        public void PreviewPage()
        {
            ////this.webDriver.SwitchTo().DefaultContent();
            this.webDriver.SwitchTo().Window(webDriver.CurrentWindowHandle);
            ////PagePreviewButton.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", PagePreviewButton);
            UserRolesDropdown.SelectByText("SiteAdmin");
            ////PreviewButton.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", PreviewButton);
        }

        #endregion
    }
}
