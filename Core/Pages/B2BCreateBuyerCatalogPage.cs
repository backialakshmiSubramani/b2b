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
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using System.Collections.ObjectModel;
using System.Linq;
using Modules.Channel.B2B.Core.Workflows.Common;

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
    public class B2BCreateBuyerCatalogPage : PageBase
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
            return GenerateCatalogLink.IsElementVisible();
        }

        /// <summary>
        /// determines whether or not the driver is active on this page. Must be overriden with each subclass.
        /// </summary>
        /// <returns>active</returns>
        public override bool IsActive()
        {
            return webDriver.Url.ToLower().Contains("createbuyercatalog.aspx");
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

        private ReadOnlyCollection<IWebElement> ConfigurationTypes
        {
            get
            {
                return webDriver.FindElements(By.Id("mytable"))[1].FindElements(By.XPath("//input[@type='checkbox']"));
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

        public string GenerateCatalog(
            Workflow workflow,
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
            System.Threading.Thread.Sleep(10000);
            ValidityEnd.SendKeys(validityEnd);
            EmailAddress.SendKeys(emailAddress);

            if (workflow == Workflow.Eudc)
            {
                if (!string.IsNullOrEmpty(configurationType))
                {
                    Console.WriteLine("Configuration Type not provided");
                }

                ClearConfigurationTypes();

                if (configurationType.Equals("Standard Configurations"))
                {
                    ////ConfigurationTypes.ElementAt(0).Click();
                    javaScriptExecutor.ExecuteScript("arguments[0].click();", ConfigurationTypes.ElementAt(0));
                }
                else if (configurationType.Equals("SNP"))
                {
                    ////ConfigurationTypes.ElementAt(2).Click();
                    javaScriptExecutor.ExecuteScript("arguments[0].click();", ConfigurationTypes.ElementAt(2));
                }
            }

            if (workflow == Workflow.Asn)
            {
                if (!ConfigurationTypes.ElementAt(0).Selected)
                {
                    ////ConfigurationTypes.ElementAt(0).Click();
                    javaScriptExecutor.ExecuteScript("arguments[0].click();", ConfigurationTypes.ElementAt(0));
                }

                if (!ConfigurationTypes.ElementAt(7).Selected)
                {
                    ////ConfigurationTypes.ElementAt(7).Click();
                    javaScriptExecutor.ExecuteScript("arguments[0].click();", ConfigurationTypes.ElementAt(7));
                }

                if (!ConfigurationTypes.ElementAt(2).Selected)
                {
                    ////ConfigurationTypes.ElementAt(2).Click();
                    javaScriptExecutor.ExecuteScript("arguments[0].click();", ConfigurationTypes.ElementAt(2));
                }

                if (!ConfigurationTypes.ElementAt(3).Selected)
                {
                    ////ConfigurationTypes.ElementAt(3).Click();
                    javaScriptExecutor.ExecuteScript("arguments[0].click();", ConfigurationTypes.ElementAt(3));
                }

                if (!ConfigurationTypes.ElementAt(6).Selected)
                {
                    ////ConfigurationTypes.ElementAt(6).Click();
                    javaScriptExecutor.ExecuteScript("arguments[0].click();", ConfigurationTypes.ElementAt(6));
                }
            }

            ////GenerateCatalogLink.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", GenerateCatalogLink);
            webDriver.WaitForElementDisplayed(By.Id("ContentPageHolder_lbl_TY_ThreadId"), new TimeSpan(0, 0, 10));

            return ThreadId.Text;
        }

        private void ClearConfigurationTypes()
        {
            foreach (var e in this.ConfigurationTypes.Where(e => e.Selected))
            {
                ////e.Click();
                this.javaScriptExecutor.ExecuteScript("arguments[0].click();", e);
            }
        }

        public void GoToBuyerCatalogListPage()
        {
            ////BuyerCatalogListLink.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", BuyerCatalogListLink);
            webDriver.WaitForPageLoad(new TimeSpan(0, 0, 10));
        }
    }
}
