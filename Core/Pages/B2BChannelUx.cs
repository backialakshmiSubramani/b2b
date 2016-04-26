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
                //Click To Publish Catalog
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
                return webDriver.FindElement(By.CssSelector("button[id='btnCreateAndDownLoadCatalog']"));
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
            webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromMinutes(1));
            webElement.Click();
            IWebElement textElement = webElement.FindElement(By.XPath("div/ul/child::li/a/div/strong[text()='" + optionText + "']"));
            textElement.Click();

            //webElement.Click();
            //IWebElement text = webElement.FindElement(By.CssSelector("input[type='text']"));
            //text.SendKeys(optionText);
            ////IWebElement optionEle = webElement.FindElement(By.CssSelector("ul[role='menu'] li:nth-of-type(1)"));
            //IWebElement optionEle = webElement.FindElement(By.XPath("//ul[@role='menu']/li[1]/a[div/strong[contains(text(),'" + optionText + "')]]"));
            //optionEle.Click();
        }

        #endregion
    }
}
