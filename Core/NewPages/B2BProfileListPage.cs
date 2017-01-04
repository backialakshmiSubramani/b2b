// ***********************************************************************
// Author           : AMERICAS\Shaukat_Saleem
// Created          : 12/23/2016 5:44:21 PM
//
// Last Modified By : AMERICAS\Shaukat_Saleem
// Last Modified On : 12/23/2016 5:44:21 PM
// ***********************************************************************
// <copyright file="B2BProfileListPage.cs" company="Dell">
//     Copyright (c) Dell 2016. All rights reserved.
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
    public class B2BProfileListPage : PageBase

    {
        IWebDriver webDriver;
        private IJavaScriptExecutor javaScriptExecutor;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public B2BProfileListPage(IWebDriver webDriver) : base(ref webDriver)
        {
            this.webDriver = webDriver;
            javaScriptExecutor = (IJavaScriptExecutor)webDriver;
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

        private SelectElement SearchCriteriaList
        {
            get
            {
                webDriver.WaitForElement(By.Id("ContentPageHolder_ddlSearchType"), TimeSpan.FromSeconds(30));
                return new SelectElement(webDriver.FindElement(By.Id("ContentPageHolder_ddlSearchType")));

            }
        }

        private IWebElement SearchTextField
        {
            get
            {
                return webDriver.FindElement(By.Id("ContentPageHolder_txtBoxSearchText"));
            }
        }

        private IWebElement SearchLink
        {
            get
            {
                return webDriver.FindElement(By.LinkText("Search"));
            }

        }

        #endregion Elements

        #region Element Actions

        public void SearchProfile(string SearchCriteria, string ProfileName)
        {
            if (SearchCriteria != null)
            {
                if (!SearchCriteriaList.SelectedOption.Text.Equals(SearchCriteria))
                {
                    SearchCriteriaList.SelectByText(SearchCriteria);
                }
            }

            SearchTextField.Set(ProfileName);
            javaScriptExecutor.ExecuteScript("arguments[0].click();", SearchLink);
            PageUtility.WaitForPageRefresh(webDriver);
        }

        public void ClickSearchedProfile(string linkText)
        {
            IWebElement profileLink = webDriver.FindElement(By.XPath("//a[contains(@id,'CustomerName')][translate(text(),'abcdefghijklmnopqrstuvwxyz','ABCDEFGHIJKLMNOPQRSTUVWXYZ')='" + linkText + "']"));
            profileLink.Click();
            PageUtility.WaitForPageRefresh(webDriver);
        }

        #endregion Element Actions
    }
}
