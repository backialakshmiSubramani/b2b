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
    public class B2BPreviewAssociatedCrossReferenceListPage : DCSGPageBase
    {
        IWebDriver webDriver;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public B2BPreviewAssociatedCrossReferenceListPage(IWebDriver webDriver)
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
        # region Elements

        private IWebElement ChooseAccountName
        {
            get
            {
                webDriver.WaitForElement(By.Id("ContentPageHolder_drp_CRT_Profiles"), TimeSpan.FromSeconds(30));
                return webDriver.FindElement(By.Id("ContentPageHolder_drp_CRT_Profiles"));
            }
        }
        private IWebElement SearchButton
        {
            get
            {
                return webDriver.FindElement(By.Id("ContentPageHolder_lnk_btnSearch"));
            }
        }
        public IWebElement CrossReferenceListTable
        {
            get
            {
                return webDriver.FindElement(By.Id("ContentPageHolder_CRTGridAssoList_grdVwCrossReferenceAssociationsList"));
            }
        }
        # endregion
        # region Element Actions
        public void SelectAccountName(string AccountName)
        {
            SelectElement accountName = new SelectElement(ChooseAccountName);
            accountName.SelectByValue(AccountName);
        }
        public void ClickSearchButton()
        {
            SearchButton.Click();
        }
        public string RowText()
        {
            const string rowPath = "//table[@id='ContentPageHolder_CRTGridAssoList_grdVwCrossReferenceAssociationsList']/tbody/tr[1]";
            return webDriver.FindElement(By.XPath(rowPath)).Text;
        }
        # endregion
    }
}
