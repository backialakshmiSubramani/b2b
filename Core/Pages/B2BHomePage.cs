// ***********************************************************************
// Author           : AMERICAS\Ygnashwaran_Sekar
// Created          : 12/1/2014 5:19:54 PM
//
// Last Modified By : AMERICAS\Ygnashwaran_Sekar
// Last Modified On : 12/1/2014 5:19:54 PM
// ***********************************************************************
// <copyright file="B2B.cs" company="Dell">
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
using System.Linq;


namespace Modules.Channel.B2B.Core.Pages
{
    /// <summary>
    /// This base class is the where all specific page classes will be derived.
    /// </summary>
    public class B2BHomePage : DCSGPageBase
    {
        IWebDriver webDriver;
        private IJavaScriptExecutor javaScriptExecutor;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public B2BHomePage(IWebDriver webDriver)
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
            return !(webDriver).ToString().Contains("(null)");
        }

        #region Elements

        private SelectElement EnvironmentList
        {
            get
            {
                webDriver.WaitForElement(By.Id("ucLeftMenu_ddlEnv"), new TimeSpan(0, 0, 10));
                return new SelectElement(webDriver.FindElement(By.Id("ucLeftMenu_ddlEnv")));
            }
        }

        private IWebElement GoButton
        {
            get
            {
                return webDriver.FindElement(By.Id("ucLeftMenu_lnkGo"));
            }
        }

        private IWebElement B2BProfileListLink
        {
            get
            {
                webDriver.WaitForElement(By.XPath("//a[contains(text(),'B2B Profile List')]"), new TimeSpan(0, 0, 10));
                return webDriver.FindElement(By.XPath("//a[contains(text(),'B2B Profile List')]"));
            }
        }

        private IWebElement CrossReferenceListLink
        {
            get
            {
                webDriver.WaitForElement(By.XPath("//a[contains(text(),'Cross Reference List')]"), new TimeSpan(0, 0, 10));
                return webDriver.FindElement(By.XPath("//a[contains(text(),'Cross Reference List')]"));
            }
        }

        private IWebElement CrAssociationlist
        {

            get
            {
                webDriver.WaitForElement(By.XPath("//a[contains(text(),'CR Association List')]"), new TimeSpan(0, 0, 10));
                return webDriver.FindElement(By.XPath("//a[contains(text(),'CR Association List')]"));

            }
        }

        private IWebElement BuyerCatalogLink
        {
            get
            {
                return webDriver.FindElement(By.XPath("//a[normalize-space(.)='Buyer Catalog']"));
            }
        }

        private IWebElement QaTools3
        {
            get
            {
                return webDriver.FindElement(By.XPath("//a[normalize-space(.)='QA Tools 3.0']"), TimeSpan.FromSeconds(10));
            }
        }

        private IWebElement BuyerCatalogAdminLink
        {
            get
            {
                return webDriver.FindElement(By.XPath("//a[normalize-space(.)='Buyer Catalog Admin']"));
            }
        }

        private IWebElement ManageUserLink
        {
            get
            {
                return webDriver.FindElement(By.XPath("//a[contains(text(),'Manage Users')]"), TimeSpan.FromSeconds(10));
            }
        }

        private IWebElement CifCatalogLink
        {
            get
            {
                webDriver.WaitForElement(By.XPath("//a[contains(text(),'CIF Catalog')]"), TimeSpan.FromSeconds(10));
                return webDriver.FindElement(By.XPath("//a[contains(text(),'CIF Catalog')]"));
            }
        }

        private IWebElement LogReport
        {
            get
            {
                return webDriver.FindElement(By.LinkText("Log Report"));
            }
        }

        #endregion

        #region Element Actions

        public void ClickB2BProfileList()
        {
            ////B2BProfileListLink.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", B2BProfileListLink);
        }

        public void ClickCrossReferenceListLink()
        {
            ////CrossReferenceListLink.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", CrossReferenceListLink);
            webDriver.WaitForPageLoad(new TimeSpan(0, 0, 10));
        }

        public void ClickCrAssociationList()
        {
            ////CrAssociationlist.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", CrAssociationlist);
            webDriver.WaitForPageLoad(new TimeSpan(0, 0, 10));
        }

        public void ClickQaTools3()
        {
            ////QaTools3.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", QaTools3);

            webDriver.WaitForPageLoad(new TimeSpan(0, 0, 10));
            String newWindow = webDriver.WindowHandles.LastOrDefault();
            webDriver.SwitchTo().Window(newWindow);
        }

        public void ClickLogReport()
        {
            ////LogReport.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", LogReport);
            webDriver.WaitForElementDisplayed(By.Id("ucBreadCrumb_lblPageTitle"), new TimeSpan(0, 0, 10));
        }

        public void ClickOnBuyerCatalogLink()
        {
            ////BuyerCatalogLink.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", BuyerCatalogLink);
            webDriver.WaitForPageLoad(new TimeSpan(0, 0, 10));
        }

        public void ClickBuyerCatalogAdmin()
        {
            BuyerCatalogAdminLink.Click();
        }

        public void ClickManageUser()
        {
            ManageUserLink.Click();
        }

        public void ClickCifCatalog()
        {
            CifCatalogLink.Click();
        }

        #endregion

        #region ReUsable Methods
        public void SelectEnvironment(string EnvironmentValue)
        {
            EnvironmentList.SelectByText(EnvironmentValue);
            ////GoButton.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", GoButton);
            webDriver.WaitForPageLoad(new TimeSpan(0, 0, 10));
        }
        #endregion

    }
}
