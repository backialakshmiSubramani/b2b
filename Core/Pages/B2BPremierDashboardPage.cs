// ***********************************************************************
// Author           : AMERICAS\Gaurav_Bhardwaj3
// Created          : 12/8/2014 2:34:17 PM
//
// Last Modified By : AMERICAS\Gaurav_Bhardwaj3
// Last Modified On : 12/8/2014 2:34:17 PM
// ***********************************************************************
// <copyright file="DashBoardPage.cs" company="Dell">
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
    /// <summary>
    /// This base class is the where all specific page classes will be derived.
    /// </summary>
    public class B2BPremierDashboardPage : PageBase
    {
        IWebDriver webDriver;

        private IJavaScriptExecutor javaScriptExecutor;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public B2BPremierDashboardPage(IWebDriver webDriver)
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

        #region Element
        private IWebElement Title
        {
            get
            {
                return webDriver.FindElement(By.XPath("//div[@class='uif_mhFooterWrap clearfix']/h1"));
            }
        }

        private IWebElement ShopElement
        {
            get
            {
                return webDriver.FindElement(By.XPath("//li[@id='ShopMenu']/div/a"));
            }
        }

        private IWebElement StandardConfigurationElement
        {
            get
            {
                return webDriver.FindElement(By.XPath("//li[@id='ShopMenu1']/div/a"));
            }
        }

        private IWebElement AccountMenuLink
        {
            get
            {
                return webDriver.FindElement(By.LinkText("Account"));
            }
        }

        private IWebElement MyCustomersLinkUnderAccountMenu
        {
            get
            {
                return webDriver.FindElement(By.LinkText("My Customers"));
            }
        }

        #endregion

        #region Element Actions

        public void WaitForTitle()
        {
            webDriver.WaitForPageLoad(TimeSpan.FromSeconds(120));
        }

        public string ReturnTitle()
        {
            return Title.Text;
        }

        public void OpenShop()
        {
            ////ShopElement.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", ShopElement);
        }

        public void ClickStandardConfiguration()
        {
            ////StandardConfigurationElement.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", StandardConfigurationElement);
            webDriver.WaitForPageLoad(TimeSpan.FromSeconds(60));
        }

        public bool CheckIfMyCustomersLinkIsAvailable()
        {
            ////AccountMenuLink.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", AccountMenuLink);
            try
            {
                return MyCustomersLinkUnderAccountMenu.IsElementVisible();
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("My Customers link not found");
                return false;
            }
        }

        #endregion
    }
}


