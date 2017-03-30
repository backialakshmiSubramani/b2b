// ***********************************************************************
// Author           : AMERICAS\Tawan_Jyot_Kaur_Bhat
// Created          : 7/27/2015 1:40:52 PM
//
// Last Modified By : AMERICAS\Tawan_Jyot_Kaur_Bhat
// Last Modified On : 7/27/2015 1:40:52 PM
// ***********************************************************************
// <copyright file="B2BChannelUx.cs" company="Dell">
//     Copyright (c) Dell 2015. All rights reserved.
// </copyright>
// <summary>Provide a summary of the page class here.</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using OpenQA.Selenium;
using Dell.Adept.Core;
using Dell.Adept.UI.Web.Pages;
using Dell.Adept.UI.Web.Support;
using Dell.Adept.UI.Web.Support.Extensions.WebDriver;
using Dell.Adept.UI.Web.Support.Extensions.WebElement;
using Dell.Adept.UI.Web.Support.Locators;
using OpenQA.Selenium.Support.UI;
using System.Threading;
using Modules.Channel.B2B.Common;
using System.Configuration;

namespace Modules.Channel.B2B.Core.Pages
{
    /// <summary>
    /// This base class is the where all specific page classes will be derived.
    /// </summary>
    public class B2BChannelUx : PageBase
    {
        IWebDriver webDriver;
        private IJavaScriptExecutor javaScriptExecutor;
        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public B2BChannelUx(IWebDriver webDriver)
            : base(ref webDriver)
        {
            this.webDriver = webDriver;
            javaScriptExecutor = (IJavaScriptExecutor)this.webDriver;
            //populate the following variables with the appropriate value
            Name = "Channel Catalog Ux Page";
            Url = webDriver.Url;
            ProductUnit = "Channel";
            this.webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromMinutes(5));
        }

        /// <summary>
        /// Treat this like a BVT of the page. If Validate does not pass, throw exception and console.writeline a return message into Test Class
        /// </summary>
        /// <returns>validated</returns>
        public override bool Validate()
        {
            return PageHeader.Displayed;
        }

        /// <summary>
        /// determines whether or not the driver is active on this page. Must be overriden with each subclass.
        /// </summary>
        /// <returns>active</returns>
        public override bool IsActive()
        {
            return webDriver.Url.ToLower().Contains("ChannelCatalogUX");
        }

        #region Elements

        /// <summary>
        /// Channel ux page Header
        /// </summary>
        public IWebElement PageHeader
        {
            get { return webDriver.FindElement(By.XPath("//*[@id='homepage-var']/div/div[2]/h2")); }
        }

        /// <summary>
        /// Channel ux Environemt dropdown
        /// </summary>
        private SelectElement EnvironmentList
        {
            get
            {
                webDriver.WaitForElement(
                    By.XPath("//*[@id='homepage-var']/div/div[2]/div/table[1]/tbody/tr/td[2]/select"),
                    new TimeSpan(0, 0, 30));
                return
                    new SelectElement(
                        webDriver.FindElement(
                            By.XPath("//*[@id='homepage-var']/div/div[2]/div/table[1]/tbody/tr/td[2]/select")));
            }
        }

        /// <summary>
        /// Channel UX Home Right side menu
        /// </summary>
        public IWebElement Rightside_Menu
        {
            get { return webDriver.FindElement(By.XPath("//div/a[@class='btn btn-link dropdown-toggle text-white top-offset-mini']")); }
        }

        /// <summary>
        /// Channel UX Home right side dropdown value -'Home'
        /// </summary>
        public IWebElement Rightside_dropdown_Home
        {
            get { return webDriver.FindElement(By.XPath("//div[@class='btn-group open']/ul/li[1]")); }
        }

        #region Publish to Processor

        public IWebElement SelectCustomerProfileDiv
        {
            get
            {
                return webDriver.FindElement(By.CssSelector("div[ng-model='publishToProcessorProfileList']"));
            }
        }

        public IWebElement SelectProfileIdentityDiv
        {
            get
            {
                return webDriver.FindElement(By.CssSelector("div[ng-model='publishToProcessorPerson']"));
            }
        }

        public IWebElement ProductionEnvRadioButton
        {
            get
            {
                return webDriver.FindElement(By.CssSelector(("input[id='publishToProcessorENV'][value='Production']")));
            }
        }

        public IWebElement PreviewEnvRadioButton
        {
            get
            {
                return webDriver.FindElement(By.CssSelector(("input[id='publishToProcessorENV'][value='Preview']")));
            }
        }

        public IWebElement OriginalRadioButton
        {
            get
            {
                return webDriver.FindElement(By.CssSelector(("input[id='CatalogType'][value='O']")));
            }
        }

        public IWebElement DeltaRadioButton
        {
            get
            {
                return webDriver.FindElement(By.CssSelector(("input[id='CatalogType'][value='D']")));
            }
        }

        public IWebElement ClickToPublishButton
        {
            get
            {
                return webDriver.FindElement(By.XPath("//button[text()='Click To Publish Catalog']"));
            }
        }

        public IWebElement UseExistingB2BAutoScheduleRadioButton
        {
            get
            {
                return webDriver.FindElement(By.CssSelector("input[type='radio'][id='rdAutoBHC'][name='AutoBHC']"));
            }
        }

        public IWebElement SetNewRadioButton
        {
            get
            {
                return webDriver.FindElement(By.CssSelector("input[type='radio'][id='rdSetNew'][name='AutoBHC']"));
            }
        }

        public IWebElement STDSetNewCheckBox
        {
            get
            {
                return webDriver.FindElement(By.CssSelector("input[type='checkbox'][id='chkSTD']"));
            }
        }

        public IWebElement SNPSetNewCheckBox
        {
            get
            {
                return webDriver.FindElement(By.CssSelector("input[type='checkbox'][id='chkSNP']"));
            }
        }

        public IWebElement SYSSetNewCheckBox
        {
            get
            {
                return webDriver.FindElement(By.CssSelector("input[type='checkbox'][id='chkSYS']"));
            }
        }

        public IWebElement CreateButton
        {
            get
            {
                return webDriver.FindElement(By.CssSelector("button[id='btnCreateCatalog']"));
            }
        }

        public IWebElement CreateAndDownloadButton
        {
            get
            {
                return webDriver.FindElement(By.CssSelector("button[id='btnCreateAndDownLoad']"));
            }
        }

        public IWebElement FeedBackMessage
        {
            get
            {
                return webDriver.FindElement(By.CssSelector("div[class='alert ng-binding'][ng-class='alert.type']"));
            }
        }
        public IWebElement ValidationMessage
        {
            get
            {
                return webDriver.FindElement(By.CssSelector("div[ng-show='alert.show']"));
            }
        }
        public IWebElement AutoCatalogInventoryListLink
        {
            get
            {
                return webDriver.FindElement(By.PartialLinkText("Auto Catalog & Inventory List"));
            }
        }
        public IWebElement AutoCatalogPartViewerLInk
        {
            get
            {
                return webDriver.FindElement(By.PartialLinkText("Auto Catalog Part Viewer"));
            }
        }

        public IWebElement AutoPackagingUploadLInk
        {
            get
            {
                return webDriver.FindElement(By.PartialLinkText("Packaging Upload"));
            }
        }

        public IWebElement AutoCreateInstantCatalogLInk
        {
            get
            {
                return webDriver.FindElement(By.PartialLinkText("Create Instant Catalog"));
            }
        }

        #endregion

        #endregion

        #region ReUsable Methods
        /// <summary>
        /// Selects the environment
        /// </summary>
        /// <param name="environmentValue"></param>
        public void SelectEnvironment(string environmentValue)
        {
            EnvironmentList.SelectByText(environmentValue);
            Console.WriteLine("B2B environment selected is: ** {0} **", environmentValue);
        }

        public void SelectOption(IWebElement webElement, string optionText)
        {
            optionText = optionText.ToUpper();
            webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromMinutes(1));
            UtilityMethods.ClickElement(webDriver, webElement);
            IWebElement textElement = webElement.FindElement(By.XPath("div/ul/child::li/a/div/strong[translate(text(),'abcdefghijklmnopqrstuvwxyz','ABCDEFGHIJKLMNOPQRSTUVWXYZ')='" + optionText + "']"));
            UtilityMethods.ClickElement(webDriver, textElement);
        }

        public void OpenCreateInstantCatalogPage(B2BEnvironment b2BEnvironment)
        {
            webDriver.Navigate().GoToUrl(ConfigurationReader.GetValue("TestHarnessPageUrl") + ((b2BEnvironment == B2BEnvironment.Production) ? "P" : "U"));
        }

        public void OpenAutoCatalogAndInventoryListPage(B2BEnvironment b2BEnvironment)
        {
            webDriver.Navigate().GoToUrl(ConfigurationReader.GetValue("AutoCatalogListPageUrl") + ((b2BEnvironment == B2BEnvironment.Production) ? "P" : "U"));
        }

        public void OpenAutoPartViewerPage(B2BEnvironment b2BEnvironment)
        {
            webDriver.Navigate().GoToUrl(ConfigurationReader.GetValue("AutoPartViewerUrl") + ((b2BEnvironment == B2BEnvironment.Production) ? "P" : "U"));
        }

        public void OpenAutoPackageUploadPage(B2BEnvironment b2BEnvironment)
        {
            webDriver.Navigate().GoToUrl(ConfigurationReader.GetValue("AutoPackageUploadUrl") + ((b2BEnvironment == B2BEnvironment.Production) ? "P" : "U"));
        }

        public void WaitForFeedBackMessage(TimeSpan timeSpan)
        {
            double timeOutInSeconds = timeSpan.TotalSeconds;
            while (string.IsNullOrEmpty(FeedBackMessage.Text) && timeOutInSeconds > 0)
            {
                Thread.Sleep(2000);
                timeOutInSeconds -= 2;
            }
        }
        #endregion
    }
}
