// ***********************************************************************
// Author           : AMERICAS\Ygnashwaran_Sekar
// Created          : 12/16/2014 2:28:21 PM
//
// Last Modified By : AMERICAS\Ygnashwaran_Sekar
// Last Modified On : 12/16/2014 2:28:21 PM
// ***********************************************************************
// <copyright file="B2BBuyerCatalogRequestPage.cs" company="Dell">
//     Copyright (c) Dell 2014. All rights reserved.
// </copyright>
// <summary>Provide a summary of the page class here.</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

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
    public class B2BBuyerCatalogRequestPage : PageBase
    {
        IWebDriver webDriver;
        private IJavaScriptExecutor javaScriptExecutor;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public B2BBuyerCatalogRequestPage(IWebDriver webDriver)
            : base(ref webDriver)
        {
            this.webDriver = webDriver;
            javaScriptExecutor = (IJavaScriptExecutor)this.webDriver;
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

        # region Elements

        private IWebElement RecipientEmailIdText
        {
            get
            {
                webDriver.WaitForElement(By.Id("ContentPageHolder_txt_BCR_RequestEmail"), TimeSpan.FromSeconds(10));
                return webDriver.FindElement(By.Id("ContentPageHolder_txt_BCR_RequestEmail"));
            }
        }

        private IWebElement SelectCustomerList
        {
            get
            {
                webDriver.WaitForElement(By.Id("ContentPageHolder_drp_BCR_SelectCustomer"), TimeSpan.FromSeconds(10));
                return webDriver.FindElement(By.Id("ContentPageHolder_drp_BCR_SelectCustomer"));
            }
        }

        private IWebElement SelectRegionList
        {
            get
            {
                webDriver.WaitForElement(By.Id("ContentPageHolder_drp_BCR_SelectRegion"), TimeSpan.FromSeconds(10));
                return webDriver.FindElement(By.Id("ContentPageHolder_drp_BCR_SelectRegion"));
            }
        }

        private IWebElement SelectProfileList
        {
            get
            {
                webDriver.WaitForElement(By.Id("ContentPageHolder_drp_BCR_Identity"), TimeSpan.FromSeconds(10));
                return webDriver.FindElement(By.Id("ContentPageHolder_drp_BCR_Identity"));
            }
        }

        private IWebElement RequestCatalogLink
        {
            get
            {
                webDriver.WaitForElement(By.LinkText("Request Catalog"), TimeSpan.FromSeconds(10));
                return webDriver.FindElement(By.LinkText("Request Catalog"));
            }
        }

        private IWebElement ThreadId
        {
            get
            {
                 webDriver.WaitForElement(By.Id("ContentPageHolder_lbl_TY_ThreadId"), TimeSpan.FromSeconds(30));
                 return webDriver.FindElement(By.Id("ContentPageHolder_lbl_TY_ThreadId"));
            }
        }

        # endregion


        # region Resuable Methods

        public string RequestCifCatalog(string email, string profileName, string region)
        {
            RecipientEmailIdText.SendKeys(email);
            SelectElement selectCustomer = new SelectElement(SelectCustomerList);
            selectCustomer.SelectByText(profileName);
            SelectElement selectProfileIdentity = new SelectElement(SelectProfileList);
            selectProfileIdentity.SelectByText(profileName.ToUpper());
            SelectElement regionList = new SelectElement(SelectRegionList);
            regionList.SelectByText(region);
            ////RequestCatalogLink.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", RequestCatalogLink);
            Console.WriteLine("Generated Thread Id is - {0}", ThreadId.Text);
            return ThreadId.Text;
        }
        # endregion

    }
}
