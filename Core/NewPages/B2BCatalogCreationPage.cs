// ***********************************************************************
// Author           : AMERICAS\Shaukat_Saleem
// Created          : 9/27/2016 4:52:31 PM
//
// Last Modified By : AMERICAS\Shaukat_Saleem
// Last Modified On : 9/27/2016 4:52:31 PM
// ***********************************************************************
// <copyright file="B2BCatalogCreationPage.cs" company="Dell">
//     Copyright (c) Dell 2016. All rights reserved.
// </copyright>
// <summary>Provide a summary of the page class here.</summary>
// ***********************************************************************
using Dell.Adept.UI.Web.Pages;
using Modules.Channel.B2B.Common;
using OpenQA.Selenium;
using System;
using System.Threading;

namespace Modules.Channel.B2B.Core.NewPages
{
    /// <summary>
    /// This base class is the where all specific page classes will be derived.
    /// </summary>
    public class B2BCatalogCreationPage : PageBase
    {
        IWebDriver webDriver;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public B2BCatalogCreationPage(IWebDriver webDriver) : base(ref webDriver)
        {
            this.webDriver = webDriver;
            //populate the following variables with the appropriate value
            //Name = "";
            //Url = "";
            //ProductUnit = "";
        }

        public void SwitchBrowser(BrowserName browser)
        {
            webDriver.SwitchBrowser(browser);
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

        public IWebElement UseExistingB2BAutoScheduleRadioButton
        {
            get
            {
                return webDriver.FindElement(By.CssSelector("input[type='radio'][id='rdAutoBHC'][name='AutoBHC']"));
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

        public IWebElement CreateButton
        {
            get
            {
                return webDriver.FindElement(By.CssSelector("button[id='btnCreateCatalog']"));
            }
        }

        public IWebElement FeedBackMessage
        {
            get
            {
                return webDriver.FindElement(By.CssSelector("div[class='alert ng-binding'][ng-class='alert.type']"));
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

        public void GoToCreateInstantCatalogPage(B2BEnvironment b2BEnvironment)
        {
            webDriver.Navigate().GoToUrl(ConfigurationReader.GetValue("TestHarnessPageUrl") + ((b2BEnvironment == B2BEnvironment.Production) ? "P" : "U"));
        }

        public string PublishCatalog(B2BEnvironment b2BEnvironment, string profileName, string identityName, CatalogType catalogType)
        {
            SelectOptionFromDropDown(SelectCustomerProfileDiv, profileName);
            SelectOptionFromDropDown(SelectProfileIdentityDiv, identityName.ToUpper());

            if (!UseExistingB2BAutoScheduleRadioButton.Selected)
                    UseExistingB2BAutoScheduleRadioButton.Click();

            SelectCatalogTypeOrginalOrDelta(catalogType);

            CreateButton.Click();

            WaitForFeedBackMessage(TimeSpan.FromMinutes(2));
            return FeedBackMessage.Text;
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

        public void SelectEnvironmentProdOrPrev(B2BEnvironment b2BEnvironment)
        {
            if (b2BEnvironment == B2BEnvironment.Production)
            {
                UtilityMethods.ClickElement(webDriver, ProductionEnvRadioButton);
            }
            else if (b2BEnvironment == B2BEnvironment.Preview)
            {
                UtilityMethods.ClickElement(webDriver, PreviewEnvRadioButton);
            }
        }

        private void SelectOptionFromDropDown(IWebElement webElement, string optionText)
        {
            webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromMinutes(1));
            string xPath = "div/ul/child::li/a/div/strong[text()='" + optionText + "']";
            UtilityMethods.SelectOptionFromDDL(webDriver, webElement, xPath);           
        }

        private void SelectCatalogTypeOrginalOrDelta(CatalogType catalogType)
        {
            if (catalogType == CatalogType.Original)
                OriginalRadioButton.Click();
            else if (catalogType == CatalogType.Delta)
                DeltaRadioButton.Click();
        }

        
    }
}
