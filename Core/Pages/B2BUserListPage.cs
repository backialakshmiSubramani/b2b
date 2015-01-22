// ***********************************************************************
// Author           : AMERICAS\Ygnashwaran_Sekar
// Created          : 12/12/2014 8:10:18 PM
//
// Last Modified By : AMERICAS\Ygnashwaran_Sekar
// Last Modified On : 12/12/2014 8:10:18 PM
// ***********************************************************************
// <copyright file="B2BUserListPage.cs" company="Dell">
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


namespace Modules.Channel.B2B.Core.Pages
{
    /// <summary>
    /// This base class is the where all specific page classes will be derived.
    /// </summary>
    public class B2BUserListPage : DCSGPageBase
    {
        IWebDriver webDriver;
        private IJavaScriptExecutor javaScriptExecutor;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public B2BUserListPage(IWebDriver webDriver)
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

        private IWebElement ManageUsersTab
        {
            get
            {
                webDriver.WaitForElementDisplayed(By.Id("__tab_ContentPageHolder_tabUsers_tabManageUsers"), TimeSpan.FromSeconds(10));
                return webDriver.FindElement(By.Id("__tab_ContentPageHolder_tabUsers_tabManageUsers"));
            }
        }

        private IWebElement UserNameText
        {
            get
            {
                webDriver.WaitForElementDisplayed(By.Id("ContentPageHolder_tabUsers_tabManageUsers_txtUserName"), TimeSpan.FromSeconds(10));
                return webDriver.FindElement(By.Id("ContentPageHolder_tabUsers_tabManageUsers_txtUserName"));
            }
        }

        private IWebElement UserTypeList
        {
            get
            {
                return webDriver.FindElement(By.Id("ContentPageHolder_tabUsers_tabManageUsers_ddlUserType"));
            }
        }

        private IWebElement SearchBtn
        {
            get
            {
                return webDriver.FindElement(By.Id("ContentPageHolder_tabUsers_tabManageUsers_lnkBtnSearch"));
            }
        }

        private IWebElement UpdateUserLink
        {
            get
            {
                webDriver.WaitForElementDisplayed(By.Id("ContentPageHolder_tabUsers_tabManageUsers_lnkManageUserUpdate"), TimeSpan.FromSeconds(10));
                return webDriver.FindElement(By.Id("ContentPageHolder_tabUsers_tabManageUsers_lnkManageUserUpdate"));
            }
        }

        private IWebElement UpdateSuccessMsg
        {
            get
            {
                webDriver.WaitForElementDisplayed(By.Id("ContentPageHolder_tabUsers_tabManageUsers_lblManageUsersOKMessage"), TimeSpan.FromSeconds(10));
                return webDriver.FindElement(By.Id("ContentPageHolder_tabUsers_tabManageUsers_lblManageUsersOKMessage"));
            }
        }

        private IWebElement UserTypeValue
        {
            get
            {
                webDriver.WaitForElementDisplayed(By.Id("ContentPageHolder_tabUsers_tabManageUsers_gvUserLists_lblUserType_0"), TimeSpan.FromSeconds(5));
                return webDriver.FindElement(By.Id("ContentPageHolder_tabUsers_tabManageUsers_gvUserLists_lblUserType_0"));
            }
        }
        #endregion

        #region ReUsable Methods

        public bool ChangeUserAccess(string UserName, string UserType)
        {
            ////ManageUsersTab.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", ManageUsersTab);
            UserNameText.Set(UserName);
            ////SearchBtn.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", SearchBtn);
            webDriver.WaitForPageLoad(TimeSpan.FromSeconds(5));
            ////webDriver.FindElement(By.XPath("//a[contains(text(),'" + UserName + "')]")).Click(); // Searched Username is dynamic
            javaScriptExecutor.ExecuteScript(
                "arguments[0].click();",
                webDriver.FindElement(By.XPath("//a[contains(text(),'" + UserName + "')]"))); // Searched Username is dynamic
            webDriver.WaitForPageLoad(TimeSpan.FromSeconds(5));
            SelectElement userType = new SelectElement(UserTypeList);
            userType.SelectByText(UserType);
            ////UpdateUserLink.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", UpdateUserLink);
            if (UpdateSuccessMsg.Displayed)
            {
                UserNameText.Set(UserName);
                ////SearchBtn.Click();
                javaScriptExecutor.ExecuteScript("arguments[0].click();", SearchBtn);
                webDriver.WaitForPageLoad(TimeSpan.FromSeconds(20));
                if (UserTypeValue.Text.Trim() == UserType)
                {
                    Console.WriteLine("User Type is changed Successfully");
                    return true;
                }
                else
                {
                    Console.WriteLine("User Type is not changed Successfully");
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        #endregion


    }
}
