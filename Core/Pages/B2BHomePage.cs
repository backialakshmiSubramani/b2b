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
using System.Linq;

//Adept Framework 
using Dell.Adept.Core;
using Dell.Adept.UI;
using Dell.Adept.UI.Web;
using Dell.Adept.UI.Web.Pages;
using Dell.Adept.UI.Web.Support.Extensions.WebDriver;
using Dell.Adept.UI.Web.Support.Extensions.WebElement;
using Dell.Adept.UI.Web.Support.Locators;
using Dell.Adept.UI.Web.Support;
using Modules.Channel.EUDC.Core.Pages;

namespace Modules.Channel.B2B.Core.Pages
{
    /// <summary>
    /// This base class is the where all specific page classes will be derived.
    /// </summary>
    public class B2BHomePage : PageBase
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
            Name = "B2B Home Page";
            //Url = webDriver.Url;
            ProductUnit = "Channel";
            //this.webDriver.WaitForPageLoad(new TimeSpan(0, 0, 10));
            PageUtility.WaitForPageRefresh(webDriver);
        }

        /// <summary>
        /// Treat this like a BVT of the page. If Validate does not pass, throw exception and console.writeline a return message into Test Class
        /// </summary>
        /// <returns>validated</returns>
        public override bool Validate()
        {
            return PageTitleHeader.Displayed;
        }

        /// <summary>
        /// determines whether or not the driver is active on this page. Must be overriden with each subclass.
        /// </summary>
        /// <returns>active</returns>
        public override bool IsActive()
        {
            return webDriver.Url.Contains("/B2BToolsCE");
        }

        #region Elements

        private IWebElement _pageTitleHeader;
        private IWebElement PageTitleHeader
        {
            get
            {
                if (_pageTitleHeader == null)
                    _pageTitleHeader = webDriver.FindElement(By.Id("lblHeaderText"), new TimeSpan(0, 0, 10));
                return _pageTitleHeader;
            }
        }
        private SelectElement EnvironmentList
        {
            get
            {
                webDriver.WaitForElement(By.Id("ucLeftMenu_ddlEnv"), new TimeSpan(0, 0, 30));
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

        public IWebElement B2BProfileListLink
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

        private IWebElement ASNLink
        {
            get
            {
                webDriver.WaitForElement(By.XPath("//a[contains(text(),' Channel ASN')]"), TimeSpan.FromSeconds(10));
                return webDriver.FindElement(By.XPath("//a[contains(text(),' Channel ASN')]"));
            }
        }

        private IWebElement LogReport
        {
            get
            {
                return webDriver.FindElement(By.LinkText("Log Report"));
            }
        }

        /// <summary>
        /// Link to go to Channel Catalog UX - Packaging data file upload page
        /// </summary>
        public IWebElement ChannelCatalogUxLink
        {
            get
            {
                return webDriver.FindElement(By.LinkText("Packaging Upload"));
            }
        }

        /// <summary>
        /// Automated Catalog List Page
        /// </summary>
        public IWebElement AutoCatalogInventoryListPageLink
        {
            get
            {
                //return webDriver.FindElement(By.LinkText("Auto Catalog & Inventory List"));
                return webDriver.FindElement(By.LinkText("Auto Catalog List"));
            }
        }

        /// <summary>
        /// Auto Catalog Part Viewer
        /// </summary>
        public IWebElement AutoCatalogPartViewerLink
        {
            get { return webDriver.FindElement(By.LinkText("Auto Catalog Part Viewer")); }
        }


        #endregion

        #region Element Actions

        public void ClickB2BProfileList()
        {
            ////B2BProfileListLink.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", B2BProfileListLink);
            //webDriver.WaitForPageLoad(new TimeSpan(0, 0, 20));
            PageUtility.WaitForPageRefresh(webDriver);
        }

        public void ClickCrossReferenceListLink()
        {
            ////CrossReferenceListLink.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", CrossReferenceListLink);
            //webDriver.WaitForPageLoad(new TimeSpan(0, 0, 10));
            PageUtility.WaitForPageRefresh(webDriver);
        }

        public void ClickCrAssociationList()
        {
            ////CrAssociationlist.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", CrAssociationlist);
            //webDriver.WaitForPageLoad(new TimeSpan(0, 0, 10));
            PageUtility.WaitForPageRefresh(webDriver);
        }

        public void ClickQaTools3()
        {
            ////QaTools3.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", QaTools3);

            //webDriver.WaitForPageLoad(new TimeSpan(0, 0, 10));
            PageUtility.WaitForPageRefresh(webDriver);
            var newWindow = webDriver.WindowHandles.LastOrDefault();
            webDriver.SwitchTo().Window(newWindow);
            Console.WriteLine("Url after switching is: {0}", webDriver.Url);
            webDriver.Manage().Window.Maximize();
        }

        public void ClickLogReport()
        {
            ////LogReport.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", LogReport);
            //webDriver.WaitForPageLoad(new TimeSpan(0, 0, 10));
            PageUtility.WaitForPageRefresh(webDriver);
        }

        public void ClickOnBuyerCatalogLink()
        {
            ////BuyerCatalogLink.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", BuyerCatalogLink);
            //webDriver.WaitForPageLoad(new TimeSpan(0, 0, 10));
            PageUtility.WaitForPageRefresh(webDriver);
        }

        public void ClickBuyerCatalogAdmin()
        {
            ////BuyerCatalogAdminLink.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", BuyerCatalogAdminLink);
        }

        public void ClickManageUser()
        {
            ////ManageUserLink.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", ManageUserLink);
        }

        public void ClickCifCatalog()
        {
            ////CifCatalogLink.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", CifCatalogLink);
        }

        public void ClickChannelPlatformLink()
        {
            ////CifASNLink.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", ASNLink);
        }

        #endregion

        #region ReUsable Methods
        public void SelectEnvironment(string environmentValue)
        {
            EnvironmentList.SelectByText(environmentValue);
            Console.WriteLine("B2B environment selected is: ** {0} **", environmentValue);
            javaScriptExecutor.ExecuteScript("arguments[0].click();", GoButton);
            PageUtility.WaitForPageRefresh(webDriver);
        }

        public void OpenPackageUploadPage()
        {
            ChannelCatalogUxLink.WaitForElementVisible(TimeSpan.FromSeconds(10));
            ChannelCatalogUxLink.Click();
            //webDriver.WaitForPageLoad(TimeSpan.FromSeconds(20));
            PageUtility.WaitForPageRefresh(webDriver);
            webDriver.SwitchTo().Window(webDriver.WindowHandles.LastOrDefault());
        }

        /// <summary>
        /// Use this method to navigate to the CPT Auto Catalog & Inventory List Page
        /// </summary>
        public void OpenAutoCatalogInventoryListPage()
        {
            AutoCatalogInventoryListPageLink.Click();
            PageUtility.WaitForPageRefresh(webDriver);
            webDriver.SwitchTo().Window(webDriver.WindowHandles.LastOrDefault());
            PageUtility.WaitForPageRefresh(webDriver);
        }

        #endregion
    }
}
