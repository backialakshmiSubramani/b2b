// ***********************************************************************
// Author           : AMERICAS\Nethra_Pandappilav
// Created          : 12/4/2014 4:34:34 PM
//
// Last Modified By : AMERICAS\Nethra_Pandappilav
// Last Modified On : 12/4/2014 4:34:34 PM
// ***********************************************************************
// <copyright file="B2BCreateBuyerCatalogPage.cs" company="Dell">
//     Copyright (c) Dell 2014. All rights reserved.
// </copyright>
// <summary>Provide a summary of the page class here.</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;
using DCSG.ADEPT.Framework;
using DCSG.ADEPT.Framework.Core;
using DCSG.ADEPT.Framework.Core.Extensions.WebDriver;
using DCSG.ADEPT.Framework.Core.Extensions.WebElement;
using DCSG.ADEPT.Framework.Core.Extensions.Locators;
using OpenQA.Selenium.Support.UI;
using DCSG.ADEPT.Framework.Core.Page;
using OpenQA.Selenium.Interactions;

namespace Modules.Channel.B2B.Core.Pages
{
    /// <summary>
    /// This base class is the where all specific page classes will be derived.
    /// </summary>
    public class B2BCreateBuyerCatalogPage : DCSGPageBase
    {
        IWebDriver webDriver;
        private IJavaScriptExecutor javaScriptExecutor;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public B2BCreateBuyerCatalogPage(IWebDriver webDriver)
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
        private SelectElement SelectCustomer
        {
            get
            {
                return new SelectElement(webDriver.FindElement(By.Id("ContentPageHolder_drp_CBC_Customer")));
            }
        }

        private SelectElement IdentityName
        {
            get
            {
                return new SelectElement(webDriver.FindElement(By.Id("ContentPageHolder_drp_CBC_Identity")));
            }
        }

        private IWebElement ValidityEnd
        {
            get
            {
                return webDriver.FindElement(By.Id("ContentPageHolder_txt_ValidityEnd"));
            }
        }

        private IWebElement EmailAddress
        {
            get
            {
                return webDriver.FindElement(By.Id("ContentPageHolder_txt_BCR_EmailNotification"));
            }
        }

        private IWebElement StandardConfigurationCheckbox
        {
            get
            {
                return webDriver.FindElement(By.Id("ContentPageHolder_chkStandardConfigurations"));
            }
        }

        private IWebElement SnpCheckbox
        {
            get
            {
                return webDriver.FindElement(By.Id("ContentPageHolder_chkSNP"));
            }
        }

        private IWebElement GenerateCatalogLink
        {
            get
            {
                return this.webDriver.FindElement(By.Id("ContentPageHolder_lnkbtnGenerateCatalog"));
            }
        }

        private IWebElement ThreadId
        {
            get
            {
                webDriver.WaitForElementDisplayed(By.Id("ContentPageHolder_lbl_TY_ThreadId"), new TimeSpan(0, 0, 10));
                return this.webDriver.FindElement(By.Id("ContentPageHolder_lbl_TY_ThreadId"));
            }
        }

        private IWebElement BuyerCatalogListLink
        {
            get
            {
                return webDriver.FindElement(By.Id("ContentPageHolder_lnkbtnCatalogList"));
            }
        }

        #endregion

        #region Element Actions

        public string GenerateCatalog(
            string profileName,
            string identityName,
            string validityEnd,
            string emailAddress,
            string configurationType)
        {
            SelectCustomer.SelectByText(profileName);
            webDriver.WaitForElementDisplayed(
                By.XPath("//select[@id='ContentPageHolder_drp_CBC_Identity']/option[text()='" + identityName + "']"),
                TimeSpan.FromSeconds(10));
            IdentityName.SelectByText(identityName);
            System.Threading.Thread.Sleep(5000);
            ValidityEnd.SendKeys(validityEnd);
            EmailAddress.SendKeys(emailAddress);

            if (!string.IsNullOrEmpty(configurationType))
            {
                if (configurationType.Equals("Standard Configurations"))
                {
                    if (!StandardConfigurationCheckbox.Selected)
                    {
                        ////StandardConfigurationCheckbox.Click();
                        javaScriptExecutor.ExecuteScript("arguments[0].click();", StandardConfigurationCheckbox);
                    }

                    if (SnpCheckbox.Selected)
                    {
                        ////SnpCheckbox.Click();
                        javaScriptExecutor.ExecuteScript("arguments[0].click();", SnpCheckbox);
                    }
                }
                else if (configurationType.Equals("SNP"))
                {
                    if (!SnpCheckbox.Selected)
                    {
                        ////SnpCheckbox.Click();
                        javaScriptExecutor.ExecuteScript("arguments[0].click();", SnpCheckbox);
                    }

                    if (StandardConfigurationCheckbox.Selected)
                    {
                        ////StandardConfigurationCheckbox.Click();
                        javaScriptExecutor.ExecuteScript("arguments[0].click();", StandardConfigurationCheckbox);
                    }
                }
            }

            ////GenerateCatalogLink.Click();

            javaScriptExecutor.ExecuteScript("arguments[0].click();", GenerateCatalogLink);

            return ThreadId.Text;
        }

        public void GoToBuyerCatalogListPage()
        {
            ////BuyerCatalogListLink.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", BuyerCatalogListLink);
            webDriver.WaitForPageLoad(new TimeSpan(0, 0, 10));
        }

        #endregion
    }
}
