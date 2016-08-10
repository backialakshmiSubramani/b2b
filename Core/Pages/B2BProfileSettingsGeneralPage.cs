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
    public class B2BProfileSettingsGeneralPage : PageBase
    {
        IWebDriver webDriver;
        private IJavaScriptExecutor javaScriptExecutor;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public B2BProfileSettingsGeneralPage(IWebDriver webDriver)
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

        public IWebElement SearchLink
        {
            get
            {
                return WebDriver.FindElement(By.XPath("//a[text()='Search ']"));
            }
        }

        private IWebElement SelectValidAccessGroupMsg
        {
            get
            {
                //webDriver.WaitForElement(By.XPath("//li[contains(text(),'Select valid AccessGroup')]"), TimeSpan.FromSeconds(60));
                //return webDriver.FindElement(By.XPath("//li[contains(text(),'Select valid AccessGroup')]"));
                webDriver.WaitForElement(By.XPath("//option[contains(text(),'Select an Item')]"), TimeSpan.FromSeconds(20));
                webDriver.WaitForPageLoad(new TimeSpan(0, 0, 10));
                return webDriver.FindElement(By.XPath("//option[contains(text(),'Select an Item')]"));
            }
        }


        private SelectElement AccessGroupList
        {
            get
            {
                //return new SelectElement(webDriver.FindElement(By.XPath("//select[contains(@id,'SelectAccessGroup')]")));
                //return new SelectElement(webDriver.FindElement(By.XPath("//option[contains(@id,'SelectAccessGroup')]")));
                return new SelectElement(webDriver.FindElement(By.Id("ContentPageHolder_CustomerAccessGroup_drp_CAG_SelectAccessGroup")));
            }
        }

        public IWebElement CreateNewProfileButton
        {
            get
            {
                return webDriver.FindElement(By.Id("ContentPageHolder_btnCreateProfile"));
            }
        }

        private IWebElement AffinityAccountId
        {
            get
            {
                return webDriver.FindElement(By.Id("ContentPageHolder_lblAffinityId"));
            }
        }

        private IWebElement Header
        {
            get
            {
                return webDriver.FindElement(By.XPath("//span[@id='ucBreadCrumb_lblPageTitle']"));
            }
        }

        private IWebElement AsnTabLink
        {
            get
            {
                return webDriver.FindElement(By.Id("ContentPageHolder_ProfileHeader_hyp_PH_ASN"));
            }
        }

        private IWebElement _testProfileCheckbox;
        public IWebElement TestProfileCheckbox
        {
            get
            {
                if (_testProfileCheckbox == null)
                {
                    _testProfileCheckbox = webDriver.FindElement(By.Id("ContentPageHolder_chkIsTestProfile"));
                }
                return _testProfileCheckbox;
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
            ////SearchLink.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", SearchLink);
        }

        public bool SelectAccessGroupMsgDisplayed()
        {
            SelectValidAccessGroupMsg.WaitForElementDisplayed(TimeSpan.FromSeconds(60));
            return SelectValidAccessGroupMsg.Displayed;
        }

        public void EnterAccessGroup(string AccessGroupValue)
        {
            AccessGroupValue = AccessGroupValue.Split('[')[1];
            int index = 1;
            for (; index < AccessGroupList.Options.Count; index++)
            {
                if (AccessGroupList.Options[index].Text.Split('[')[1] == AccessGroupValue)
                    break;
            }
            AccessGroupList.SelectByIndex(index);
            //AccessGroupList.SelectByText(AccessGroupValue);
        }


        public void ClickCreateNewProfile()
        {
            ////CreateNewProfileButton.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", CreateNewProfileButton);
        }

        public string FindAffinityAccountId()
        {
            return AffinityAccountId.Text;
        }

        public void GoToAsnTab()
        {
            javaScriptExecutor.ExecuteScript("arguments[0].click();", AsnTabLink);
            webDriver.WaitForPageLoad(new TimeSpan(0, 0, 10));
        }
    }
}
