// ***********************************************************************
// Author           : AMERICAS\Nethra_Pandappilav
// Created          : 12/3/2014 5:14:12 PM
//
// Last Modified By : AMERICAS\Nethra_Pandappilav
// Last Modified On : 12/3/2014 5:14:12 PM
// ***********************************************************************
// <copyright file="B2BProfileSettingsGeneralPage.cs" company="Dell">
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
using DCSG.ADEPT.Framework.Core.Page;
using OpenQA.Selenium.Support.UI;


namespace Modules.Channel.B2B.Core.Pages
{
    /// <summary>
    /// This base class is the where all specific page classes will be derived.
    /// </summary>
    public class B2BProfileSettingsGeneralPage : DCSGPageBase
    {
        IWebDriver webDriver;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public B2BProfileSettingsGeneralPage(IWebDriver webDriver)
            : base(ref webDriver)
        {
            this.webDriver = webDriver;
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

        private IWebElement UserIdText
        {
            get
            {
                webDriver.WaitForElement(By.XPath("//input[contains(@id,'txtProfileName')]"), TimeSpan.FromSeconds(60));
                return webDriver.FindElement(By.XPath("//input[contains(@id,'txtProfileName')]"));
            }
        }

        private IWebElement CustomerNameText
        {
            get
            {
                return webDriver.FindElement(By.XPath("//input[contains(@id,'txtDescription')]"));
            }
        }

        private IWebElement IdentityNameText
        {
            get
            {
                return webDriver.FindElement(By.XPath("//input[contains(@id,'txtIdentityName')]"));
            }
        }

        private IWebElement CustomerSetText
        {
            get
            {
                return webDriver.FindElement(By.XPath("//input[contains(@id,'CustomerAccessGroup')]"));
            }
        }

        private IWebElement SearchLink
        {
            get
            {
                return webDriver.FindElement(By.LinkText("Search"));
            }
        }

        private IWebElement SelectValidAccessGroupMsg
        {
            get
            {
                webDriver.WaitForElement(By.XPath("//li[contains(text(),'Select valid AccessGroup')]"), TimeSpan.FromSeconds(60));
                return webDriver.FindElement(By.XPath("//li[contains(text(),'Select valid AccessGroup')]"));
            }
        }


        private SelectElement AccessGroupList
        {
            get
            {
                return new SelectElement(webDriver.FindElement(By.XPath("//select[contains(@id,'SelectAccessGroup')]")));
            }
        }

        private IWebElement CreateNewProfileButton
        {
            get
            {
                return webDriver.FindElement(By.Id("ContentPageHolder_btnCreateProfile"));
            }
        }

        private IWebElement Header
        {
            get
            {
                return webDriver.FindElement(By.XPath("//span[@id='ucBreadCrumb_lblPageTitle']"));
            }
        }


        #endregion

        #region Element Action
        public void EnterUserId(string UserId)
        {

            UserIdText.Set(UserId);
        }

        public void EnterCustomerName(string CustomerName)
        {
            CustomerNameText.Set(CustomerName);
        }

        public void EnterIdentityName(string IdentityName)
        {
            IdentityNameText.Set(IdentityName);
        }
        #endregion

        public void EnterCustomerSet(string CustomerSet)
        {
            CustomerSetText.Set(CustomerSet);
        }

        public void ClickSearch()
        {
            SearchLink.Click();
        }

        public bool SelectAccessGroupMsgDisplayed()
        {
            return SelectValidAccessGroupMsg.Displayed;
        }


        public void EnterAccessGroup(string AccessGroupValue)
        {
            AccessGroupList.SelectByText(AccessGroupValue);
        }

        public void ClickCreateNewProfile()
        {
            CreateNewProfileButton.Click();
        }

    }

}
