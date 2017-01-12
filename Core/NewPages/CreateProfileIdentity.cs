// ***********************************************************************
// Author           : AMERICAS\Shaukat_Saleem
// Created          : 1/6/2017 11:35:33 AM
//
// Last Modified By : AMERICAS\Shaukat_Saleem
// Last Modified On : 1/6/2017 11:35:33 AM
// ***********************************************************************
// <copyright file="CreateProfileIdentity.cs" company="Dell">
//     Copyright (c) Dell 2017. All rights reserved.
// </copyright>
// <summary>Provide a summary of the page class here.</summary>
// ***********************************************************************
using Dell.Adept.UI.Web.Pages;
using Dell.Adept.UI.Web.Support.Extensions.WebDriver;
using Dell.Adept.UI.Web.Support.Extensions.WebElement;
using Modules.Channel.B2B.Core.Pages;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;


namespace Modules.Channel.B2B.Core.NewPages
{
    /// <summary>
    /// This base class is the where all specific page classes will be derived.
    /// </summary>
    public class CreateProfileIdentity : PageBase

    {
        IWebDriver webDriver;
        private IJavaScriptExecutor javaScriptExecutor;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public CreateProfileIdentity(IWebDriver webDriver) : base(ref webDriver)
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
            throw new NotImplementedException();
        }

        #region Elements

        private IWebElement IdentityNameField
        {
            get
            {
                return webDriver.FindElement(By.Id("ContentPageHolder_txtIdentityName"));
            }
        }

        private IWebElement CustomerSetField
        {
            get
            {
                return webDriver.FindElement(By.Id("ContentPageHolder_CustomerAccessGroup_txt_CAG_CustomerSet"));
            }
        }

        private IWebElement SearchLink
        {
            get
            {
                return webDriver.FindElement(By.Id("ContentPageHolder_CustomerAccessGroup_lnk_CAG_Search"));
            }
        }

        private SelectElement SelectAccessGroup
        {
            get
            {
                webDriver.WaitForElement(By.Id("ContentPageHolder_CustomerAccessGroup_drp_CAG_SelectAccessGroup"),
                    TimeSpan.FromSeconds(30));
                return new SelectElement(webDriver.FindElement(By.Id("ContentPageHolder_CustomerAccessGroup_drp_CAG_SelectAccessGroup")));
            }
        }

        private IWebElement CreateThisIdentityLink
        {
            get
            {
                return webDriver.FindElement(By.Id("ContentPageHolder_lnkCreate"));
            }
        }

        private IWebElement ConfirmationLabel
        {
            get
            {
                return webDriver.FindElement(By.Id("ContentPageHolder_lbl_PR_OKmsg"), new TimeSpan(0, 0, 60));
            }
        }

        #endregion Elements

        #region Element Actions

        public bool CreateNewProfileIdentity(string identityName, string customerSet, string accessGroup)
        {
            IdentityNameField.Set(identityName);
            CustomerSetField.Set(customerSet);
            SearchLink.Click();
            PageUtility.WaitForPageRefresh(webDriver);

            if (SelectAccessGroup.SelectedOption.Text.Equals("--Select an Item--"))
            {
                SelectAccessGroup.SelectByValue(accessGroup);
            }

            javaScriptExecutor.ExecuteScript("arguments[0].click();", CreateThisIdentityLink);
            PageUtility.WaitForPageRefresh(webDriver);
            return string.Equals(ConfirmationLabel.Text.Trim(), "Profile Identity created successfully.");
        }

        #endregion Element Action
    }
}
