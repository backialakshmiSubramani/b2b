// ***********************************************************************
// Author           : AMERICAS\Vinay_Chand
// Created          : 12/9/2014 1:28:52 PM
//
// Last Modified By : AMERICAS\Vinay_Chand
// Last Modified On : 12/9/2014 1:28:52 PM
// ***********************************************************************
// <copyright file="B2BPreviewAssociatedCrossReferenceList.cs" company="Dell">
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
    public class B2BPreviewAssociatedCrossReferenceListPage : PageBase
    {
        IWebDriver webDriver;
        private IJavaScriptExecutor javaScriptExecutor;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public B2BPreviewAssociatedCrossReferenceListPage(IWebDriver webDriver)
            : base(ref webDriver)
        {
            this.webDriver = webDriver;
            javaScriptExecutor = (IJavaScriptExecutor)this.webDriver;
            Name = "B2B Preview Associated Cross Reference Lis";
            Url = webDriver.Url;
            ProductUnit = "Channel";
            webDriver.WaitForPageLoad(new TimeSpan(0, 0, 10));
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
                    _pageTitleHeader = webDriver.FindElement(By.Id("ucBreadCrumb_lblPageTitle"), new TimeSpan(0, 0, 10));
                return _pageTitleHeader;
            }
        }

        private SelectElement AccountName
        {
            get
            {
                webDriver.WaitForElement(By.Id("ContentPageHolder_drp_CRT_Profiles"), TimeSpan.FromSeconds(30));
                return new SelectElement(webDriver.FindElement(By.Id("ContentPageHolder_drp_CRT_Profiles")));
            }
        }

        private IWebElement SearchButton
        {
            get
            {
                return webDriver.FindElement(By.Id("ContentPageHolder_lnk_btnSearch"));
            }
        }

        private IWebElement CrossReferenceListTable
        {
            get
            {
                return webDriver.FindElement(By.Id("ContentPageHolder_CRTGridAssoList_grdVwCrossReferenceAssociationsList"));
            }
        }

        #endregion

        #region Element Actions

        public void SelectAccountName(string AccountName)
        {
            this.AccountName.SelectByValue(AccountName);
        }

        public void ClickSearchButton()
        {
            ////SearchButton.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", SearchButton);
        }

        public string RowText()
        {
            const string rowPath = "//table[@id='ContentPageHolder_CRTGridAssoList_grdVwCrossReferenceAssociationsList']/tbody/tr[1]";
            return webDriver.FindElement(By.XPath(rowPath)).Text;
        }

        #endregion
    }
}
